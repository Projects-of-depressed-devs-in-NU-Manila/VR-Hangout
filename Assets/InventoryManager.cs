using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public struct Inventory
{
    public List<InventoryItem> items;
}

public struct InventoryItem
{
    public int qty;
    public string object_id;
    public string player_id;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public GameObject inventoryItemPrefab;

    public InventoryItem equippedItem;
    public GameObject equippedItemPanel;

    public Inventory inventory;
    public GameObject inventoryUIPanel;
    public event Action<Inventory> inventoryChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        equippedItem = new InventoryItem();
        StartCoroutine(GetInventory());
    }

    IEnumerator GetInventory()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(APIUrl.url + $"inventory?player_id={PlayerContext.Instance.playerId}"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Fetched inventory");
                Inventory inventory = JsonHelper.FromJson<Inventory>(request.downloadHandler.text);
                this.inventory = inventory;
                inventoryChanged?.Invoke(this.inventory);
            }
        }
        UpdateInventoryUI(this.inventory);
    }

    public void SetEquippedItem(InventoryItem inventoryItem)
    {
        equippedItem = inventoryItem;
        if (inventoryItem.object_id == "" || inventoryItem.object_id == null)
        {

            equippedItemPanel.SetActive(false);
            equippedItemPanel.transform.GetChild(0).GetComponent<RawImage>().texture = null;
            equippedItemPanel.SetActive(true);
            equippedItemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = "0";
            return;
        }

        GameObject obj = WorldObjectUtils.LoadPrefab(inventoryItem.object_id);
        Texture2D texture = RuntimePreviewGenerator.GenerateModelPreview(obj.transform);
        equippedItemPanel.transform.GetChild(0).GetComponent<RawImage>().texture = texture;
        equippedItemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = equippedItem.qty.ToString();
    }

    public void AddInventoryIetm(InventoryItem inventoryItem)
    {
        StartCoroutine(AddInventoryItemCoroutine(inventoryItem));
    }

    public void RemoveInventoryItem(InventoryItem inventoryItem)
    {
        StartCoroutine(RemoveInventoryItemCoroutine(inventoryItem));
    }

    IEnumerator AddInventoryItemCoroutine(InventoryItem inventoryItem)
    {
        string url = $"{APIUrl.url}inventory";
        string json = JsonHelper.ToJson(inventoryItem);

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
            Debug.Log("Response: " + request.downloadHandler.text);

        StartCoroutine(GetInventory());
    }

    IEnumerator RemoveInventoryItemCoroutine(InventoryItem inventoryItem)
    {
        string url = $"{APIUrl.url}inventory";
        string json = JsonHelper.ToJson(inventoryItem);

        var request = new UnityWebRequest(url, "DELETE");
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
            Debug.Log("Response: " + request.downloadHandler.text);

        StartCoroutine(GetInventory());
    }

    void UpdateInventoryUI(Inventory inventory)
    {
        foreach (Transform child in inventoryUIPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem inventoryItem in inventory.items)
        {
            GameObject inventoryUI = Instantiate(inventoryItemPrefab, inventoryUIPanel.transform);
            GameObject obj = WorldObjectUtils.LoadPrefab(inventoryItem.object_id);
            Texture2D texture = RuntimePreviewGenerator.GenerateModelPreview(obj.transform);
            inventoryUI.transform.GetChild(0).GetComponent<RawImage>().texture = texture;
            inventoryUI.transform.GetChild(1).GetComponent<TMP_Text>().text = inventoryItem.qty.ToString();
            inventoryUI.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { SetEquippedItem(inventoryItem); });
        }
    }

}
