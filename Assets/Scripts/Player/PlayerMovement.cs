using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;
    public Transform cameraPivot;
    public GameObject fishingRod;

    private Rigidbody rb;
    private bool canMove;
    private float verticalLookRotation = 0f;
    private bool isGrounded;
    private Vector2 input;
    private bool isFishing = false;
    [SerializeField] private GameObject fishGame;
    [SerializeField] private GameObject character;
    private bool insideFishingRegion;
    private Animator animator;
    private string currentScene;

    private bool isLooking = false;
    

    void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        OnSceneChanged(new Scene(), new Scene());
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.freezeRotation = true;
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {

        currentScene = SceneManager.GetActiveScene().name;
        ObjectPlacingController objController = character.GetComponent<ObjectPlacingController>();
        PlayerTransformSyncer playerSync = character.GetComponent<PlayerTransformSyncer>();
        InventoryManager inventory = character.GetComponent<InventoryManager>();

        if (currentScene == "Avatar")
        {
            isLooking = false;
            objController.enabled = false;
            playerSync.enabled = false;
            inventory.enabled = false;
        }
        else if (currentScene == "MainMenu")
        {
            isLooking = false;
            objController.enabled = false;
            playerSync.enabled = false;
            inventory.enabled = false;
        }
        else if (currentScene == "LoginPage")
        {
            isLooking = false;
            objController.enabled = false;
            playerSync.enabled = false;
            inventory.enabled = false;
        }
        else
        {
            isLooking = true;
            objController.enabled = true;
            playerSync.enabled = true;
            inventory.enabled = true;
        }
    }

    void Update()
    {
        if (!isFishing && currentScene != "Avatar") { canMove = true; }
        if (canMove)
        {
            HandleLook();
            HandleInput();
            HandleJump();
            HandleMovement();
        }
        else
        {
        }
        HandleFishing();
    }

    private void HandleLook()
    {
        if (!isLooking)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        Quaternion deltaRotation = Quaternion.Euler(0f, mouseX, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -40f, 40f);

        cameraPivot.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    private void HandleInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        bool isMoving = input.sqrMagnitude > 0.1f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isRunning", isRunning);
    }

    private void HandleFishing()
    {
        if (insideFishingRegion && Input.GetKeyDown(KeyCode.F))
        {
            bool newState = !isFishing;
            canMove = !canMove;
            SetFishing(newState);
            Debug.Log("Fishing toggled. New state: " + newState);
        }
    }


    private void HandleJump()
    {
        CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isFishing)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleMovement()
    {
        if (isFishing || input.sqrMagnitude <= 0.1f) return;

        Vector3 moveDir = transform.forward * input.y + transform.right * input.x;
        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift)) speed *= 2f;

        Vector3 move = moveDir.normalized * speed;
        Vector3 velocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = velocity;
    }


    private void CheckGrounded()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.2f, Vector3.down);
        isGrounded = Physics.Raycast(ray, 0.6f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FishingZone"))
        {
            insideFishingRegion = true;
            Debug.Log("Inside Fish Region");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FishingZone"))
        {
            insideFishingRegion = false;
            Debug.Log("Outside Fish Region");
        }
    }

    public void SetFishing(bool fishing)
    {
        isFishing = fishing;
        animator.SetBool("isFishing", fishing);

        if (fishGame == null)
        {
            Debug.LogWarning("fishGame reference is null!");
            return;
        }

        if (fishing)
        {
            fishGame.GetComponent<Fishgame>().ResetGame();

            StartCoroutine(ShowFishingUIWithDelay(5f));
        }
        else
        {
            fishGame.SetActive(false);
        }

        if (fishing)
        {
            fishingRod.SetActive(true);
        }
        else
        {
            fishingRod.SetActive(false);
        }
    }

    private IEnumerator ShowFishingUIWithDelay(float delay)
    {
        fishGame.SetActive(false);
        yield return new WaitForSeconds(delay);
        if (isFishing)
        {
            fishGame.SetActive(true);
        }
    }

    public bool IsFishing()
    {
        return isFishing;
    }

    public void setCanMove(bool Move)
    {
        canMove = Move;
    }
}