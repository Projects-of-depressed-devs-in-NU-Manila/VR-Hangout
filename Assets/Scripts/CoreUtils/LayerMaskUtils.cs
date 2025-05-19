using UnityEngine;

public static class LayerMaskUtils{
    public static void SetLayerRecursively(UnityEngine.GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }
}
