using System;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class PlayerMovementPhysics : MonoBehaviour
{
    #region Variables
    [Header("--Movement--")]
    [Tooltip("Changes the movespeed of the character")]
    public float moveSpeed = 7;
    [Tooltip("The speed of how fast the player accelerates")]
    public float moveAcceleration = 10;
    public float moveDeceleration = 3;
    public float moveStopVelocity = 0.1f;

    [Header("--Jumping--")]
    [Tooltip("Height of the jump in meters")]
    public float jumpHeight = 4f;
    [Tooltip("the amount of time that needs to acceed so the player can jump again (to prevent spamming the jump)")]
    public float jumpCooldownTime = 0.1f;
    [Tooltip("Transform for the GroundCheck object in the player")]
    public Transform groundCheck;
    [Tooltip("Radius of how far it will check is there is ground below the player")]
    public float groundDistance = 0.5f;
    [Tooltip("LayerMask for the ground")]
    public LayerMask groundLayer;
    [Tooltip("This will show if the player is on the ground or not")]
    public bool isGrounded;

    [Header("--Gravity--")]
    [Tooltip("This is the gravity applied to the player when hes falling down from a jump")]
    public float fallGravityMultiplier = 1.5f;

    [HideInInspector]
    public Rigidbody rb;

    //exposed variables
    [HideInInspector]
    public bool isStationary;

    //private variables
    private float _runSpeedPercent;
    private float _jumpCooldownTimer;
    private float _inputMagnitude;
    private bool _jumpCooldown = false;
    private bool _wantToJump = false;
    private Vector3 _velocity;
    private float _horizontal;
    private float _vertical;
    private Vector3 _curWorldInput;  
    #endregion

    private void Awake()
    {      
        rb = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        CheckIfGround();
        Movement();        
        Jumping();
    }

    void Movement()
    {
        //input with speed var calculated in
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        //change input to world space
        _curWorldInput = Camera.main.transform.TransformDirection(new Vector3(_horizontal, 0, _vertical));
        //how much input is given of the world input
        _inputMagnitude = _curWorldInput.magnitude;
        //flattend world input
        _curWorldInput = new Vector3(_curWorldInput.x, 0, _curWorldInput.z).normalized;

        float targetSpeed = moveSpeed * _inputMagnitude;
        
        Vector3 playerVel = rb.velocity;

        Vector3 velocityHorizontal = new(playerVel.x, 0, playerVel.z); //horizontal velocity of the player    
        
        isStationary = velocityHorizontal.sqrMagnitude < moveStopVelocity && _inputMagnitude == 0;

        if (isStationary)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), 5 * Time.fixedDeltaTime);
            return;
        }

        float playerVelMag = playerVel.x * playerVel.x + playerVel.z * playerVel.z; //more optimized then Mathf.sqrt and .sqrtMagnitude

        if(_inputMagnitude > 0)
        {
            _runSpeedPercent = playerVelMag / (targetSpeed * targetSpeed); //completed optimization of previous line        
        }
        else
        {
            _runSpeedPercent = 2;
        }
        
        float overSpeedMult = 0f;
        if(_runSpeedPercent > 1)
        {
            overSpeedMult = Mathf.Clamp01(_runSpeedPercent - 1);
            _runSpeedPercent = Mathf.Clamp01(_runSpeedPercent);
        }

        float velAccelerationMult = 1 - _runSpeedPercent; //want to know the remaining acceleration so we one-minus the value      
        
        //how much to accelerate by
        velAccelerationMult *= moveAcceleration;
        
        Vector3 finalForce = _curWorldInput * velAccelerationMult + -velocityHorizontal * overSpeedMult * moveDeceleration;    

        //https://twitter.com/freyaholmer/status/1203059678705602562
        Vector3 inputCrossVelocity = Vector3.Cross(_curWorldInput, velocityHorizontal.normalized);
        //flipping the product so its aligns with left and right
        inputCrossVelocity = Quaternion.AngleAxis(90, _curWorldInput) * inputCrossVelocity;


        finalForce += inputCrossVelocity * moveAcceleration * 0.5f;
        
        rb.AddForce(finalForce, ForceMode.Acceleration);    
    }

    void Jumping()
    {

        _wantToJump = Input.GetButtonDown("Jump");

        //when the cooldown is active add it up with time and if it has exceeded the cooldown time you can jump again
        if (_jumpCooldown && isGrounded && !_wantToJump)
        {
            _jumpCooldownTimer += Time.deltaTime;
            if(_jumpCooldownTimer >= jumpCooldownTime)
            {
                _jumpCooldownTimer = 0;
                _jumpCooldown = false;
            }
        }

        if (_wantToJump && isGrounded && !_jumpCooldown)
        {
            //calculate the force needed to jump the height given
            var jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);

            //if player is moving down reset the velocity to zero so it always reaches full height when jumping     
            if(rb.velocity.y < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }

            rb.AddForce(0, jumpVelocity, 0, ForceMode.Impulse);
            _jumpCooldown = true;
        }
        
    }
    void CheckIfGround()
    {
        //this checks if the player is standing on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        //so there is always a little velocity down on the player for when it falls
        if (isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;           
        }
    }
}
