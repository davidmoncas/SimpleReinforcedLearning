using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceObjectsController : MonoBehaviour
{
    [SerializeField] private float minSeparation;
    [SerializeField] private Transform minPosition, maxPosition;
    [SerializeField] private Transform[] obstacles;
    [SerializeField] private Transform goal;
    [SerializeField] private Transform agent;

    private readonly List<Transform> allObjects = new();
    private readonly List<Vector2> placedPositions = new();

    void OnEnable()
    {
        allObjects.Add(agent.transform);
        allObjects.Add(goal.transform);
        allObjects.AddRange(obstacles.Select(o => o.transform));
    }

    public void PlaceObjects2D()
    {
        placedPositions.Clear();

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

            obj.localPosition = new Vector3(newPos.x, newPos.y, obj.localPosition.z);
            placedPositions.Add(newPos);
        }
    }

    Vector2 GetRandomPosition2D()
    {
        return new Vector2(
            Random.Range(minPosition.localPosition.x, maxPosition.localPosition.x),
            Random.Range(minPosition.localPosition.y, maxPosition.localPosition.y)
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