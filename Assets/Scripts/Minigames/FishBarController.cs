using UnityEngine;

public class FishBarController : MonoBehaviour
{
    public RectTransform greenBar; 
    public RectTransform trackBounds;

    public float floatSpeed = 150f;
    public float gravity = 300f; 
    private float velocity = 0f;

    void Update()
    {
        bool isHolding = Input.GetKey(KeyCode.Space); // or use GetMouseButton(0) for mouse

        if (isHolding)
            velocity = floatSpeed;
        else
            velocity -= gravity * Time.deltaTime;

        Vector2 pos = greenBar.anchoredPosition;
        pos.y += velocity * Time.deltaTime;

        float halfBarHeight = greenBar.rect.height / 2f;
        float minY = -trackBounds.rect.height / 2f + halfBarHeight;
        float maxY = trackBounds.rect.height / 2f - halfBarHeight;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        greenBar.anchoredPosition = pos;
    }
}
