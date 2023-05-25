using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class ArmsFollowCamera : MonoBehaviour
{
    [SerializeField] private PlayerMovementPhysics _playerMovement;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Vector3 _swayAmount;
    [SerializeField] private float _lerpSpeedPosition;
    [SerializeField] private float _lerpSpeedRotation;
    [SerializeField] private float _swayAmplitude = 1f;
    [SerializeField] private float _swayPeriod = 0.1f;

    private Vector3 _positionOffset;
    private Vector3 _sway;
    private float _timer;

    private void Start()
    {
        _positionOffset = transform.localPosition - _cameraTransform.localPosition; //calcualte position difference to make an offset
    }

    private void LateUpdate()
    {
        var position = _cameraTransform.localPosition + _positionOffset;

        transform.localPosition = position + transform.TransformVector(_sway);
        transform.rotation = Quaternion.Lerp(transform.rotation, _cameraTransform.rotation, Time.deltaTime * _lerpSpeedRotation);

        HandleSway();
    }

    private void HandleSway()
    {
        //get the absolute velocity from the x and z axis
        var absoluteVelocity = Mathf.Abs(_playerMovement.GetRigidbody().velocity.x + _playerMovement.GetRigidbody().velocity.z) / 2;

        if (absoluteVelocity < 0.001) //to make sure there are no VERY low numbers
            absoluteVelocity = 0;

        //Camera bobbing up and down
        if (_playerMovement.GetRigidbody().velocity.magnitude > 0.2f && _playerMovement.IsGrounded)
        {
            _timer += Time.deltaTime;
            _sway = GetSway(_timer, _swayPeriod, _swayAmplitude, _swayAmount, absoluteVelocity);

        }
        else if (_playerMovement.GetRigidbody().velocity.magnitude < 0.2f && _playerMovement.IsGrounded)
        {
            _timer = 0;
            _sway = Vector3.Lerp(_sway, Vector3.zero, Time.deltaTime); //smoothly go back
        }
    }

    private Vector3 GetSway(float timer, float period, float amplitude, Vector3 localPosition, float absoluteVelocity = 1)
    {
        float theta = timer / (period);
        float distance = (amplitude * absoluteVelocity) * Mathf.Sin(theta);
        return localPosition * distance;
    }
}