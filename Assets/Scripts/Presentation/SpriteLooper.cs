using UnityEngine;

public class SpriteLooper : MonoBehaviour
{
    [SerializeField] float animationSpeed;
    float offset;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] sprites;

    private void Awake()
    {
        offset = Random.Range(0f, 2f);
    }

    private void Update()
    {
        if (sprites.Length == 0) return;

        offset += Time.deltaTime * animationSpeed;
        int index = Mathf.FloorToInt(offset) % sprites.Length;
        spriteRenderer.sprite = sprites[index];
    }
}
