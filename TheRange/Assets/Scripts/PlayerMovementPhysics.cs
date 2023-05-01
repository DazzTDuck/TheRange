using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class PlayerMovementPhysics : MonoBehaviour
{
    #region Variables
    [Header("--Movement--")]
    [Tooltip("Changes the movespeed of the character")]
    [SerializeField] private float _runningMoveSpeed = 7;
    [Tooltip("The speed of how fast the player accelerates")]
    [SerializeField] private float _moveAcceleration = 10;
    [SerializeField] private float _moveDeceleration = 3;
    [SerializeField] private float _slideDeceleration = 0.35f;
    [SerializeField] private float _moveStopVelocity = 0.1f;
    [SerializeField] private float _moveInAirMultiplier = 0.2f;
    [Tooltip("This value determines how fast your speed drops in the air")]
    [SerializeField] private float _moveInAirLerpSpeed = 3f;

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
    public bool IsGrounded { get; private set; }

    [Header("--Crouching--")]
    [SerializeField] private float _crouchWalkSpeedMultiplier = 0.5f;
    [SerializeField] private float _crouchLerpSpeed;
    [SerializeField] private float _normalHeight;
    [SerializeField] private float _crouchHeight;
    public bool IsCrouching {  get; private set; }

    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _playerCollider;

    //private variables
    private bool isStationary;
    private float _runSpeedPercent;
    private float _jumpCooldownTimer;
    private float _inputMagnitude;
    private bool _jumpCooldown = false;
    private bool _wantToJump = false;
    private Vector3 _velocity;
    private float _horizontal;
    private float _vertical;
    private Vector3 _curWorldInput;
    private float _moveSpeed;
    private float _horizontalMultiplier;
    private float _verticalMultiplier;
    #endregion

    private void Update()
    {
        Jumping();
        Crouching();
    }

    void FixedUpdate()
    {
        CheckIfGround();
        Movement();
    }

    private void Movement()
    {
        //when crouching and have enough speed make it feel like you're sliding more
        var decelerationAmount = IsCrouching && _rigidbody.velocity.magnitude > 0.3f ? _slideDeceleration : _moveDeceleration;

        //when crouching make movespeed slower over time
        var moveSpeedValue = IsCrouching ? (_runningMoveSpeed * _crouchWalkSpeedMultiplier) : _runningMoveSpeed;
        _moveSpeed = Mathf.Lerp(_moveSpeed, moveSpeedValue, Time.fixedDeltaTime * _crouchLerpSpeed);

        //input with speed var calculated in
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        //if in air limit the input amount for the player, lerp to make it gradually drop so it feels smooth (controlled by _moveInAirLerpSpeed)
        _horizontalMultiplier = Mathf.Lerp(_horizontalMultiplier, IsGrounded ? 1 : _moveInAirMultiplier, Time.fixedDeltaTime * _moveInAirLerpSpeed);
        _horizontal *= _horizontalMultiplier;
        //also for the vertical axis
        _verticalMultiplier = Mathf.Lerp(_verticalMultiplier, IsGrounded ? 1 : _moveInAirMultiplier, Time.fixedDeltaTime * _moveInAirLerpSpeed);
        _vertical *= _verticalMultiplier;

        //change input to world space
        _curWorldInput = Camera.main.transform.TransformDirection(new Vector3(_horizontal, 0, _vertical));
        //how much input is given of the world input
        _inputMagnitude = _curWorldInput.magnitude;
        //flattend world input
        _curWorldInput = new Vector3(_curWorldInput.x, 0, _curWorldInput.z).normalized;

        float targetSpeed = _moveSpeed * _inputMagnitude;
        
        Vector3 playerVel = _rigidbody.velocity;

        Vector3 velocityHorizontal = new(playerVel.x, 0, playerVel.z); //horizontal velocity of the player    
        
        isStationary = velocityHorizontal.sqrMagnitude < _moveStopVelocity && _inputMagnitude == 0;

        //if player is stationary, lerp the the velocity to 0 to make sure that there is no small forces being aplied
        if (isStationary)
        {
            _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, new Vector3(0, _rigidbody.velocity.y, 0), 5 * Time.fixedDeltaTime);
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
        velAccelerationMult *= _moveAcceleration;

        Vector3 finalForce = _curWorldInput * velAccelerationMult + -velocityHorizontal * overSpeedMult * decelerationAmount;    

        //https://twitter.com/freyaholmer/status/1203059678705602562
        Vector3 inputCrossVelocity = Vector3.Cross(_curWorldInput, velocityHorizontal.normalized);
        //flipping the product so its aligns with left and right
        inputCrossVelocity = Quaternion.AngleAxis(90, _curWorldInput) * inputCrossVelocity;

        finalForce += 0.5f * _moveAcceleration * inputCrossVelocity;

        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }

    private void Jumping()
    {
        _wantToJump = Input.GetButtonDown("Jump");

        //when the cooldown is active add it up with time and if it has exceeded the cooldown time you can jump again
        if (_jumpCooldown && IsGrounded && !_wantToJump)
        {
            _jumpCooldownTimer += Time.deltaTime;
            if(_jumpCooldownTimer >= jumpCooldownTime)
            {
                _jumpCooldownTimer = 0;
                _jumpCooldown = false;
            }
        }

        if (_wantToJump && IsGrounded && !_jumpCooldown)
        {
            //calculate the force needed to jump the height given
            var jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);

            //if player is moving down reset the velocity to zero so it always reaches full height when jumping     
            if(_rigidbody.velocity.y < 0)
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            }

            //get current velocity and half it to reduce the speed, this launches the player towards the direction the player is moving
            var currentVelocity = new Vector3(_rigidbody.velocity.x * 0.5f, 0, _rigidbody.velocity.z * 0.5f);

            _rigidbody.AddForce(currentVelocity.x, jumpVelocity, currentVelocity.z, ForceMode.Impulse);
            _jumpCooldown = true;
        }
        
    }

    private void Crouching()
    {
        IsCrouching = Input.GetButton("Crouch");
        var newHeight = IsCrouching ? _crouchHeight : _normalHeight;
        
        if(_playerCollider.height != newHeight)
        {
            _playerCollider.height = Mathf.Lerp(_playerCollider.height, newHeight, Time.deltaTime * _crouchLerpSpeed);
        }
    }
    private void CheckIfGround()
    {
        //this checks if the player is standing on the ground
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        //so there is always a little velocity down on the player for when it falls
        if (IsGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;           
        }
    }

    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
    }
}
