using UnityEngine;

public class FrameLimiter : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;

    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
