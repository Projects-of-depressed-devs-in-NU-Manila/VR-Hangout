using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public RectTransform fishIcon;
    public RectTransform trackBounds;

    public float moveSpeed = 300f; 
    public float directionChangeInterval = 1f; 

    private float direction = 1f;
    private float timer = 0f;

    void Start()
    {
        direction = Random.Range(0, 2) == 0 ? -1f : 1f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= directionChangeInterval)
        {
            direction = Random.Range(-1f, 1f);
            timer = 0f;
        }

        Vector2 pos = fishIcon.anchoredPosition;
        pos.y += direction * moveSpeed * Time.deltaTime;

        float halfFishHeight = fishIcon.rect.height / 2f;
        float minY = -trackBounds.rect.height / 2f + halfFishHeight;
        float maxY = trackBounds.rect.height / 2f - halfFishHeight;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        fishIcon.anchoredPosition = pos;
    }
}
