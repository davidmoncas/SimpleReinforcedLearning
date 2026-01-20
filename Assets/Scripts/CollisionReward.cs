using UnityEngine;

public class CollisionReward : MonoBehaviour
{
    [field: SerializeField] public int Reward { get; private set; }
    [field: SerializeField] public StatsCounter.CollisionType collisionType { get; private set; }
}
