using TMPro;
using UnityEngine;


public class StatsCounter : MonoBehaviour
{
    public enum CollisionType { Target, Wall, Obstacle }

    [SerializeField] private TextMeshProUGUI wonText, obstacleText, wallText , totalText;
    private int wonCount, obstacleCount, wallCount;

    void Start()
    {
        UpdateTexts();
    }

    public void OnCollisionHappened(CollisionType collisionType)
    {
        switch (collisionType)
        {
            case CollisionType.Target:
                wonCount++;
                break;
            case CollisionType.Wall:
                wallCount++;
                break;
            case CollisionType.Obstacle:
                obstacleCount++;
                break;
        }

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        wonText.text = "Won<br>" + wonCount;
        obstacleText.text = "Obstacles<br>" + obstacleCount;
        wallText.text = "Wall<br>" + wallCount;
        totalText.text = "Total: " + (wonCount + obstacleCount + wallCount);
    }

}
