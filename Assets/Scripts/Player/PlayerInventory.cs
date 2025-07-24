using Mirror;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    [Header("Value")]
    [SerializeField] private float _rangeItem;
    [SerializeField] private float _powerDrop;

    [Header("Objects")]
    [SerializeField] private Transform hand;
    [SerializeField] private LayerMask playerMask;

    private PlayerController playerController;
    
    [SyncVar(hook = nameof(ChangedItem))]
    private GameObject currentItem;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        CheckItem();
        DropItem();
    }

    private void CheckItem()
    {
        Transform cameraTransform = playerController.playerCamera;

        Ray rayItem = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit itemHit;

        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * _rangeItem, Color.red);

        if (Physics.Raycast(rayItem, out itemHit, _rangeItem, ~playerMask))
        {
            if (itemHit.collider.CompareTag("Item") && Input.GetKeyDown(KeyCode.E))
            {
                CmdAddItemToHand(itemHit.collider.gameObject);

                return;
            }
        }
    }

    [Command]
    private void CmdAddItemToHand(GameObject item)
    {
        if (currentItem != null) return;

        currentItem = item;

        RpcAddItemToHand(item);
    }

    [ClientRpc]
    private void RpcAddItemToHand(GameObject item)
    {
        item.transform.SetParent(hand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        item.GetComponent<Rigidbody>().isKinematic = true;
        item.GetComponent<Collider>().enabled = false;
    }

    private void DropItem()
    {
        if (Input.GetKeyDown(KeyCode.Q) && currentItem != null)
        {
            CmdDropItem();

            return;
        }
    }

    [Command]
    private void CmdDropItem()
    {
        Vector3 throwDirection = transform.forward * _powerDrop + transform.up * 0.2f;

        RpcDropItem(currentItem, throwDirection);

        currentItem = null;
    }

    [ClientRpc]
    private void RpcDropItem(GameObject item, Vector3 throwDirection)
    {
        item.transform.SetParent(null);

        Rigidbody itemRB = item.GetComponent<Rigidbody>();
        itemRB.isKinematic = false;
        itemRB.AddForce(throwDirection, ForceMode.Impulse);

        item.GetComponent<Collider>().enabled = true;
    }

    private void ChangedItem(GameObject oldItem, GameObject newItem)
    {
        
    }
}