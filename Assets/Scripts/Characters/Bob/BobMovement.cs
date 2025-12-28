using UnityEngine;

/// <summary>
/// Handles player movement, rotation and animation states for Bob.
/// Includes acceleration, movement grace period and stable rotation handling.
/// </summary>
public class BobMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [Tooltip("Walking movement speed")]
    public float walkSpeed = 1.5f;

    [Tooltip("Running movement speed")]
    public float runSpeed = 3.5f;

    [Tooltip("Acceleration toward target speed")]
    public float acceleration = 15f;

    [Header("Rotation")]
    [Tooltip("How fast the character rotates toward movement direction")]
    public float rotationSpeed = 8f;

    [Header("Movement Tuning")]
    [Tooltip("Time (in seconds) before the character is considered stopped")]
    [SerializeField] private float stopDelay = 0.1f;

    [Header("References")]
    [Tooltip("Transform used as rotation pivot (centered pivot)")]
    [SerializeField] private Transform rotationPivot;

    private Animator animator;

    // Input & movement
    private Vector3 input;
    private Vector3 velocity;
    private float currentSpeed;

    // Direction memory (prevents rotation glitches)
    private Vector3 lastMoveDirection = Vector3.forward;

    // Movement states
    private bool isMoving;
    private bool isWalking;
    private bool isRunning;

    // Grace period timer
    private float lastMovementTime;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        lastMovementTime = -stopDelay; // Avoid anim glitches
    }

    void Update()
    {
        ReadInput();
        UpdateMovementStates();
        UpdateAnimation();
        RotateTowardsMovement();
    }

    void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Reads raw movement input and normalizes it.
    /// Stores last valid movement direction.
    /// </summary>
    void ReadInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        input = new Vector3(x, 0f, z);

        if (input.magnitude > 1f)
            input.Normalize();

        // Store last valid direction
        if (input.magnitude > 0.01f)
            lastMoveDirection = input;
    }

    /// <summary>
    /// Updates logical movement states using a grace period
    /// to avoid animation flickering.
    /// </summary>
    void UpdateMovementStates()
    {
        bool hasInput = input.magnitude > 0.01f;

        if (hasInput)
            lastMovementTime = Time.time;

        // Consider movement active until stopDelay expires
        isMoving = Time.time - lastMovementTime < stopDelay;
        Debug.Log(Time.time - lastMovementTime);

        isRunning = isMoving && Input.GetKey(KeyCode.Space);
        isWalking = isMoving && !isRunning;
    }

    /// <summary>
    /// Moves the character with acceleration smoothing.
    /// </summary>
    void Move()
    {
        float targetSpeed = 0f;

        if (isMoving)
            targetSpeed = isRunning ? runSpeed : walkSpeed;

        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            acceleration * Time.fixedDeltaTime
        );

        velocity = input * currentSpeed;
        transform.position += velocity * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Smoothly rotates toward the last valid movement direction.
    /// </summary>
    void RotateTowardsMovement()
    {
        if (!isMoving || rotationPivot == null)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(
            lastMoveDirection,
            Vector3.up
        );

        rotationPivot.rotation = Quaternion.Slerp(
            rotationPivot.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Sends logical states and movement direction to the Animator.
    /// Animator does NOT manage speed.
    /// </summary>
    void UpdateAnimation()
    {
        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsRunning", isRunning);
    }
}