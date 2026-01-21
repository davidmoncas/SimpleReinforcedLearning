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
    [SerializeField] private StatsCounter statsCounter;
    [SerializeField] private RobotStateController robotStateController;
    [SerializeField] private bool waitForAnimations;
    [SerializeField] private PlaceObjectsController placeObjectsController;

    private bool episodeEnding;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition);
        sensor.AddObservation((Vector2)goal.localPosition - (Vector2)transform.localPosition);
        foreach (var obstacle in obstacles)
        {
            sensor.AddObservation((Vector2)obstacle.localPosition - (Vector2)transform.localPosition);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;
        actions[0] = MapAxisToDiscreteAction((int)Input.GetAxisRaw("Horizontal"));
        actions[1] = MapAxisToDiscreteAction((int)Input.GetAxisRaw("Vertical"));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if(robotStateController.State != RobotState.Moving)
            return;

        float moveX = MapDiscreteActionToAxis(actions.DiscreteActions[0]);
        float moveY = MapDiscreteActionToAxis(actions.DiscreteActions[1]);

        transform.localPosition += movementSpeed * Time.deltaTime * new Vector3(moveX, moveY);
    }

    public override void OnEpisodeBegin()
    {
        placeObjectsController.PlaceObjects2D();
        episodeEnding = false;
        robotStateController.SetState(RobotState.Moving);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (robotStateController.State != RobotState.Moving)
            return;

        if (other.TryGetComponent<CollisionReward>(out var reward))
        {
            SetReward(reward.Reward);
            statsCounter.OnCollisionHappened(reward.CollisionType);
            HandleCollisionAsync(reward.CollisionType).Forget();
        }
    }

    private async UniTask HandleCollisionAsync(StatsCounter.CollisionType collisionType)
    {
        if (episodeEnding)
            return;

        episodeEnding = true;

        robotStateController.SetState(
            collisionType == StatsCounter.CollisionType.Target
                ? RobotState.Winning
                : RobotState.Dying
        );

        if (!waitForAnimations)
        {
            EndEpisode();
            return;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: true);
        EndEpisode();
    }

    private int MapDiscreteActionToAxis(int input)
    {
        return input switch
        {
            0 => -1,
            1 => 0,
            _ => 1,
        };
    }

    private int MapAxisToDiscreteAction(int input)
    {
        return input switch
        {
            -1 => 0,
            0 => 1,
            _ => -1,
        };
    }
}
