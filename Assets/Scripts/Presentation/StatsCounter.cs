using System.Collections;
using TMPro;
using UnityEngine;


public class StatsCounter : MonoBehaviour
{
    public enum CollisionType { Target, Wall, Obstacle }

    [SerializeField] private TextMeshProUGUI wonText, obstacleText, wallText, totalText;
    [SerializeField] private RectTransform wonTextContainer, obstacleTextContainer, wallTextContainer, totalTextContainer;
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
                StartCoroutine(AnimateContainerCoroutine(wonTextContainer));
                break;
            case CollisionType.Wall:
                wallCount++;
                StartCoroutine(AnimateContainerCoroutine(wallTextContainer));
                break;
            case CollisionType.Obstacle:
                obstacleCount++;
                StartCoroutine(AnimateContainerCoroutine(obstacleTextContainer));
                break;
        }

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        wonText.text = "Won<br>" + wonCount;
        obstacleText.text = "Obstacles<br>" + obstacleCount;
        wallText.text = "Border<br>" + wallCount;
        totalText.text = "Total: " + (wonCount + obstacleCount + wallCount);
    }

    private IEnumerator AnimateContainerCoroutine(RectTransform container)
    {
        float durationInSeconds = 0.25f;
        Vector3 initialScale = container.localScale;
        int steps = 30;
        float sizeChange = 0.4f;
        for (int i = 0; i < steps; i++)
        {
            float t = i / (float)steps * sizeChange;
            container.localScale = (1 + t) * initialScale;
            yield return new WaitForSeconds(durationInSeconds / steps);
        }
        for (int i = steps; i > -1; i--)
        {
            float t = i / (float)steps * sizeChange;
            container.localScale = (1 + t) * initialScale;
            yield return new WaitForSeconds(durationInSeconds / steps);
        }
        container.localScale = initialScale;
    }

}
