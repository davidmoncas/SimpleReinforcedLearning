using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform goal;
    [SerializeField] private Transform[] obstacles;
    [SerializeField] private float movementSpeed;
    private float savedSpeed;
    [SerializeField] private float minSeparation;
    [SerializeField] private Transform minPosition, maxPosition;
    [SerializeField] private StatsCounter statsCounter;

    [SerializeField] private RobotStateController robotStateController;
    [SerializeField] private bool WaitForAnimations;
    [SerializeField] private bool accelerateTime;

    private bool UseTimeScale
    {
        get { return accelerateTime; }
        set
        {
            accelerateTime = value;
            Academy.Instance.AutomaticSteppingEnabled = accelerateTime;
            if(!accelerateTime)
            {
                Time.timeScale = 1f; // Reset time scale to normal
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.position);
        sensor.AddObservation((Vector2)goal.position - (Vector2)transform.localPosition);
        sensor.AddObservation((Vector2)obstacles[0].position - (Vector2)transform.localPosition);
    }

    protected override void Awake()
    {
        base.Awake();
        savedSpeed = movementSpeed;
    }

    public async void HandleAnimations(StatsCounter.CollisionType collisionType)
    {
        if (WaitForAnimations)
        {
            movementSpeed = 0;
            if (collisionType == StatsCounter.CollisionType.Target)
            {
                robotStateController.SetState(RobotState.Winning);
                await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: true);
            }
            else if (collisionType == StatsCounter.CollisionType.Obstacle || collisionType == StatsCounter.CollisionType.Wall)
            {
                robotStateController.SetState(RobotState.Dying);
                await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: true);
            }
        }
        EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float MoveX = actions.ContinuousActions[0];
        float MoveY = actions.ContinuousActions[1];

        transform.localPosition += movementSpeed * Time.deltaTime * new Vector3(MoveX, MoveY);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (movementSpeed == 0)
            return;

        if (other.TryGetComponent<CollisionReward>(out var reward))
        {
            SetReward(reward.Reward);
            statsCounter.OnCollisionHappened(reward.collisionType);
            HandleAnimations(reward.collisionType);
        }
    }

    public override void OnEpisodeBegin()
    {
        PlaceObjects2D();
        movementSpeed = savedSpeed;
        robotStateController.SetState(RobotState.Moving);
    }

    public void PlaceObjects2D()
    {
        List<Transform> allObjects = new List<Transform> { this.transform, goal.transform };
        allObjects.AddRange(obstacles.Select(o => o.transform));

        List<Vector2> placedPositions = new List<Vector2>();

        foreach (Transform obj in allObjects)
        {
            Vector2 newPos;
            int attempts = 0;
            const int maxAttempts = 100;

            do
            {
                newPos = GetRandomPosition2D();
                attempts++;
            }
            while (!IsValidPosition2D(newPos, placedPositions) && attempts < maxAttempts);

            obj.position = new Vector3(newPos.x, newPos.y, obj.position.z); // preserve z
            placedPositions.Add(newPos);
        }
    }

    Vector2 GetRandomPosition2D()
    {
        return new Vector2(
            Random.Range(minPosition.position.x, maxPosition.position.x),
            Random.Range(minPosition.position.y, maxPosition.position.y)
        );
    }

    bool IsValidPosition2D(Vector2 pos, List<Vector2> others)
    {
        foreach (var other in others)
        {
            if (Vector2.Distance(pos, other) < minSeparation)
                return false;
        }
        return true;
    }

}
