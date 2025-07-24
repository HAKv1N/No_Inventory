using Mirror;
using UnityEngine;

public class ItemInfo : NetworkBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private ItemSettings itemSettings;
}

[System.Serializable]
public class ItemSettings
{
    public int _itemID;
    public GameObject _itemPrefab;
}