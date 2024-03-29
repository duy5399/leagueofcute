using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody rigidbodyPlayer;
    [SerializeField] private List<Animator> animator;
    [SerializeField] private PlayerInput playerInput;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private Transform orientation;
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private bool isMoving;
    [SerializeField] private Vector3 groundVelocity;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private bool isGrounded;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float jumpMultiplier;
    [SerializeField] private bool isJumping;

    [Header("Drag")]
    [SerializeField] private float groundDrag;
    [SerializeField] private float airDrag;

    [Header("Cursor")]
    [SerializeField] private bool isCursorDisplay;
    [SerializeField] private float sensX;
    //[SerializeField] private float sensY;
    [SerializeField] private float mouseX;
    //[SerializeField] private float mouseY;
    //[SerializeField] private float rotationX;
    [SerializeField] private float rotationY;

    private void Awake()
    {
        rigidbodyPlayer = GetComponent<Rigidbody>();
        rigidbodyPlayer.freezeRotation = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).childCount != 0 && transform.GetChild(i).GetChild(0).GetComponent<Animator>() != null)
            {
                animator.Add(transform.GetChild(i).GetChild(0).GetComponent<Animator>());
            }
        }
    }

    private void OnEnable()
    {
        if(playerInput == null)
        {
            playerInput = new PlayerInput();
        }
        playerInput.Enable();
        playerInput.PlayerControls.Jump.performed += PlayerJump;
        playerInput.UI.Cursor.performed += cxt => ShowHideCursor(true);
        playerInput.UI.Cursor.canceled += cxt => ShowHideCursor(false);
    }

    private void Disable()
    {
        playerInput.Disable();
    }


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        PlayerInput();
        GroundCheck();
        RotationPlayer();
    }

    private void FixedUpdate()
    {
        PlayerMove();
        SpeedControl();
        UpdateAnimations();
    }

    private void PlayerInput()
    {
        //di chuyển
        horizontalInput = playerInput.PlayerControls.Movement.ReadValue<Vector2>().x;
        verticalInput = playerInput.PlayerControls.Movement.ReadValue<Vector2>().y;

        //horizontalInput = Input.GetAxisRaw("Horizontal");
        //verticalInput = Input.GetAxisRaw("Vertical");

        mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;

        moveDirection = (transform.forward * verticalInput) + (transform.right * horizontalInput);
        //moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        if (horizontalInput != 0 || verticalInput != 0)
        {
            isMoving = true;
            foreach (var anim in animator)
            {               
                anim.SetTrigger("LeftToRight");
            }
        }
        else
        {
            isMoving = false;
        }
    }

    private void PlayerMove()
    {
        if (isGrounded)
        {
            rigidbodyPlayer.AddForce(moveDirection.normalized * moveSpeed * speedMultiplier, ForceMode.Force);
            Debug.Log("grounded force: " + moveDirection.normalized * moveSpeed * speedMultiplier);
        }
        else if(!isGrounded)
        {
            rigidbodyPlayer.AddForce(moveDirection.normalized * moveSpeed * 0.5f * speedMultiplier * jumpMultiplier, ForceMode.Force);
            Debug.Log("!grounded force: " + moveDirection.normalized * moveSpeed * speedMultiplier * jumpMultiplier);
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.2f, groundLayerMask);
        if(isGrounded)
        {
            rigidbodyPlayer.drag = groundDrag;
        }
        else
        {
            rigidbodyPlayer.drag = airDrag;
        }
    }

    private void SpeedControl()
    {
        groundVelocity = new Vector3(rigidbodyPlayer.velocity.x, 0 , rigidbodyPlayer.velocity.z);
        if( groundVelocity.magnitude > moveSpeed )
        {
            Vector3 limitedVelocity = groundVelocity.normalized * moveSpeed;
            rigidbodyPlayer.velocity = new Vector3(limitedVelocity.x, rigidbodyPlayer.velocity.y, limitedVelocity.z);
            //Debug.Log("rb.velocity" + rb.velocity.magnitude);
        }
    }

    private void PlayerJump(InputAction.CallbackContext context)
    {
        if (isGrounded && !isJumping)
        {
            isJumping = true;
            rigidbodyPlayer.velocity = new Vector3(rigidbodyPlayer.velocity.x, 0f, rigidbodyPlayer.velocity.z);
            rigidbodyPlayer.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        isJumping = false;
    }

    public void ShowHideCursor(bool boolean)
    {
        if (!boolean)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isCursorDisplay = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            isCursorDisplay = true;
        }
    }

    public void RotationPlayer()
    {
        if (!isCursorDisplay)
        {
            rotationY += mouseX;
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }

    private void UpdateAnimations()
    {
        foreach (var anim in animator)
        {
            anim.SetBool("isGrounded", isGrounded);
            anim.SetFloat("HorizontalInput", horizontalInput);
            anim.SetFloat("VerticalInput", verticalInput);
            anim.SetFloat("VelocityZ", rigidbodyPlayer.velocity.z);
            anim.SetFloat("VelocityX", rigidbodyPlayer.velocity.y);
            anim.SetBool("isMoving", isMoving);
            anim.SetBool("isJumping", isJumping);
            anim.SetFloat("GroundVelocity", groundVelocity.magnitude);
        }
    }
}
