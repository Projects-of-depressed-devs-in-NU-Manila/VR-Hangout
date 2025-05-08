using UnityEngine;

public class PlayerContext : MonoBehaviour
{
    public static PlayerContext Instance;
    [SerializeField] public string playerId = "1000";

    void Awake()
    {
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
}
