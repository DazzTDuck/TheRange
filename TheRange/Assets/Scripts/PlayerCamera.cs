using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerCamera : MonoBehaviour
{
    [Header("--Settings--")]
    [Tooltip("Sensitivity of the camera movement")]
    [SerializeField] private float _sensitivity = 3f;
    [SerializeField] private float _maxRotX = 45f;
    [SerializeField] private float _minRotX = -55;
    [SerializeField] private float _walkingBobbingSpeed= 10f;
    [SerializeField] private float _walkingBobbingAmountCam = 0.05f;
    [SerializeField] private float _walkingBobbingAmountArms = 0.05f;

    [Header("--Player reference--")]
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _arms;
    [SerializeField] private PlayerMovementPhysics _playerMovement;

    [HideInInspector]
    public float rotCamX;
    [HideInInspector]
    public float rotCamY;

    private float _defaultPosYCam = 0;
    private float _defaultPosYArms = 0;
    private float _timer = 0;

    private void Awake()
    {
        HideCursor();
        _defaultPosYCam = transform.localPosition.y;
        _defaultPosYArms = _arms.localPosition.y;
    }

    private void Update()
    {
        CameraPos();
    }

    void CameraPos()
    {
        rotCamY = Input.GetAxisRaw("Mouse X") * _sensitivity;
        rotCamX += Input.GetAxisRaw("Mouse Y") * _sensitivity;

        //Clamping the rotX value
        rotCamX = Mathf.Clamp(rotCamX, _minRotX, _maxRotX);

        //calculate rotations
        var cameraRotation = new Vector3(-rotCamX, _player.eulerAngles.y, 0);
        var bodyRotation = new Vector3(0, _player.eulerAngles.y + rotCamY, 0);
        transform.eulerAngles = cameraRotation;

        //rotate player body
        _player.eulerAngles = bodyRotation;

        //add camera bobbing
        if(_playerMovement.GetRigidbody().velocity.magnitude > 0.3f)
        {
            //TODO - Make bobbing based on the player velocity

            _timer += Time.deltaTime * (_walkingBobbingSpeed * 1);
            var newPositionCamera = new Vector3(transform.localPosition.x, _defaultPosYCam + Mathf.Sin(_timer) * _walkingBobbingAmountCam, transform.localPosition.z);
            var newPositionArms = new Vector3(_arms.localPosition.x, _defaultPosYArms - Mathf.Sin(_timer) * _walkingBobbingAmountArms, _arms.localPosition.z);
            
            transform.localPosition = newPositionCamera;
            _arms.localPosition = newPositionArms;
        }
        else
        {
            _timer = 0f;

            var newPositionCamera = new Vector3(transform.localPosition.x, 
                Mathf.Lerp(transform.localPosition.y, _defaultPosYCam, Time.deltaTime * _walkingBobbingSpeed), transform.localPosition.x);
            transform.localPosition = newPositionCamera;

            var newPositionArms = new Vector3(_arms.localPosition.x,
                Mathf.Lerp(_arms.localPosition.y, _defaultPosYArms, Time.deltaTime * _walkingBobbingSpeed), _arms.localPosition.x);
            _arms.localPosition = newPositionArms;
        }
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

}
