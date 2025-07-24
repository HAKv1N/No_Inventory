using Mirror;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Value")]
    [SerializeField] private float _speed;
    [SerializeField] private float _sensitivity;

    [Header("Objects")]
    [SerializeField] private Transform playerHead;

    private CharacterController characterController;
    [HideInInspector] public Transform playerCamera;
    private Vector3 moveVector;
    private float rotationX;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>().GetComponent<Transform>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InitializePlayer();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        Move();
        FirstPerson();
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveVector = transform.forward * v + transform.right * h;

        characterController.Move(moveVector.normalized * _speed * Time.deltaTime);
    }

    private void FirstPerson()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80, 80);

        characterController.transform.Rotate(0, mouseX, 0);
        playerHead.localRotation = Quaternion.Euler(rotationX, 0, 0);
    }

    private void InitializePlayer()
    {
        if (isLocalPlayer) return;

        Destroy(playerCamera.gameObject);
    }
}