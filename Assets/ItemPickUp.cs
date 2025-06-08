using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public string object_id = "";
    public int qty = 1;

    void Start()
    {
        if (object_id == "")
        {
            Destroy(gameObject);
        }
        
        GameObject prefab = WorldObjectUtils.LoadPrefab(object_id);
        GameObject obj = Instantiate(prefab, transform);
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("collided");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("player");
            InventoryItem inventoryItem = new InventoryItem();
            inventoryItem.qty = qty;
            inventoryItem.object_id = object_id;
            inventoryItem.player_id = PlayerContext.Instance.playerId;
            InventoryManager.Instance.AddInventoryIetm(inventoryItem);
            Destroy(gameObject);
        }
    }

}
