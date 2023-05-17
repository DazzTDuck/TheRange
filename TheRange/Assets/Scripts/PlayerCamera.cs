using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("--Settings--")]
    [Tooltip("Sensitivity of the camera movement")]
    [SerializeField] private float _sensitivity = 3f;
    [SerializeField] private float _maxRotX = 45f;
    [SerializeField] private float _minRotX = -55;
    [SerializeField] private float _bobbingAmplitude = 1f;
    [SerializeField] private float _bobbingPeriod = 0.1f;

    [Header("--Player reference--")]
    [SerializeField] private Transform _player;
    [SerializeField] private PlayerMovementPhysics _playerMovement;

    [HideInInspector]
    public float rotCamX;
    [HideInInspector]
    public float rotCamY;

    private Vector3 _recoilAmount;
    private Vector3 _startLocalPositionCam;
    private float _timer;
    private float _recoilMultiplier;

    private void Awake()
    {
        HideCursor();
        _startLocalPositionCam = transform.localPosition;
    }

    private void Update()
    {
        CalculateRotationsAndPositions();
    }

    private void CalculateRotationsAndPositions()
    {
        //getting raw mouse input values
        rotCamY = Input.GetAxisRaw("Mouse X") * _sensitivity;
        rotCamX += Input.GetAxisRaw("Mouse Y") * _sensitivity;

        //Clamping the rotX value
        rotCamX = Mathf.Clamp(rotCamX, _minRotX, _maxRotX);

        //calculate rotations
        var cameraRotation = new Vector3(-rotCamX, _player.eulerAngles.y, 0);
        var bodyRotation = new Vector3(0, _player.eulerAngles.y + rotCamY, 0);
        transform.eulerAngles = cameraRotation + (_recoilAmount * _recoilMultiplier);

        //rotate player body
        _player.eulerAngles = bodyRotation;

        HandleCameraBobbing();
    }

    private void HandleCameraBobbing()
    {
        //get the absolute velocity from the x and z axis
        var absoluteVelocity = Mathf.Abs(_playerMovement.GetRigidbody().velocity.x + _playerMovement.GetRigidbody().velocity.z) / 2;

        if (absoluteVelocity < 0.001) //to make sure there are no VERY low numbers
            absoluteVelocity = 0;

        //Camera bobbing up and down
        if (_playerMovement.GetRigidbody().velocity.magnitude > 0.2f && _playerMovement.IsGrounded)
        {
            _timer += Time.deltaTime;
            transform.localPosition = GetBobbing(_timer, _bobbingPeriod, _bobbingAmplitude, _startLocalPositionCam, absoluteVelocity);

        }
        else if (_playerMovement.GetRigidbody().velocity.magnitude < 0.2f && _playerMovement.IsGrounded)
        {
            _timer = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _startLocalPositionCam, Time.deltaTime * 1); //smoothly go back
        }
    }

    private Vector3 GetBobbing(float timer, float period, float amplitude,Vector3 localPosition, float absoluteVelocity = 1)
    {
        float theta = timer / (period);
        float distance = (amplitude * absoluteVelocity) * Mathf.Sin(theta);
        return localPosition + Vector3.up * distance;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void AddToRecoil(Vector3 recoilAmount, float recoilMultiplier)
    {
        _recoilMultiplier = recoilMultiplier;
        _recoilAmount = recoilAmount;
    }
}
