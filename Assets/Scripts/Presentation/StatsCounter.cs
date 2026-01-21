using System.Collections;
using TMPro;
using UnityEngine;

public class StatsCounter : MonoBehaviour
{
    public enum CollisionType { Target, Wall, Obstacle }

    [SerializeField] private TextMeshProUGUI wonText, obstacleText, wallText, totalText;
    [SerializeField] private RectTransform wonTextContainer, obstacleTextContainer, wallTextContainer, totalTextContainer;
    private int wonCount, obstacleCount, wallCount;
    private Coroutine sizeChangeCoroutine;
    private RectTransform sizeChangingObject;

    void Start()
    {
        UpdateTexts();
    }

    public void OnCollisionHappened(CollisionType collisionType)
    {
        if (sizeChangeCoroutine != null)
        {
            StopCoroutine(sizeChangeCoroutine);
            sizeChangingObject.localScale = Vector3.one;
        }

        switch (collisionType)
        {
            case CollisionType.Target:
                wonCount++;
                sizeChangeCoroutine = StartCoroutine(AnimateContainerCoroutine(wonTextContainer));
                break;
            case CollisionType.Wall:
                wallCount++;
                sizeChangeCoroutine = StartCoroutine(AnimateContainerCoroutine(wallTextContainer));
                break;
            case CollisionType.Obstacle:
                obstacleCount++;
                sizeChangeCoroutine = StartCoroutine(AnimateContainerCoroutine(obstacleTextContainer));
                break;
        }

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        float total = wonCount + obstacleCount + wallCount;
        total = Mathf.Max(total, 1f);
        wonText.text = $"Won<br> {wonCount} ({(wonCount / total * 100).ToString("F0")}%)";
        obstacleText.text = $"Obstacles<br> {obstacleCount} ({(obstacleCount / total * 100).ToString("F0")}%)";
        wallText.text = $"Border<br> {wallCount} ({(wallCount / total * 100).ToString("F0")}%)";
        totalText.text = $"Total: {total}";
    }

    private IEnumerator AnimateContainerCoroutine(RectTransform container)
    {
        float durationInSeconds = 0.25f;
        sizeChangingObject = container;
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
