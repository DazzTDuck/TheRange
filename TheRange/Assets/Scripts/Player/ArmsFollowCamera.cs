using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class ArmsFollowCamera : MonoBehaviour
{
    #region variables

    [SerializeField] private PlayerMovementPhysics _playerMovement;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _lerpSpeedRotation;
    [SerializeField] private float _swayChangeLerpSpeed;
    [Space]
    [SerializeField] private Vector3 _swayAmount;
    [SerializeField] private float _swayAmplitude = 1f;
    [SerializeField] private float _swayPeriod = 0.1f;
    [Space]
    [SerializeField] private Vector3 _airSwayAmount;
    [SerializeField] private float _airSwayAmplitude = 1f;
    [SerializeField] private float _airSwayPeriod = 0.1f;

    private Vector3 _positionOffset;
    private Vector3 _sway;
    private float _timer;

    #endregion

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
        var absoluteVelocityXZ = Mathf.Abs(_playerMovement.GetRigidbody().velocity.x) + Mathf.Abs(_playerMovement.GetRigidbody().velocity.z) / 2;

        if (absoluteVelocityXZ < 0.001) //to make sure there are no VERY low numbers
            absoluteVelocityXZ = 0;

        //Camera bobbing up and down
        if (_playerMovement.GetRigidbody().velocity.magnitude > 0.2f && _playerMovement.IsGrounded)
        {
            _timer += Time.deltaTime;
            var newSway = GetSway(_timer, _swayPeriod, _swayAmplitude, _swayAmount, absoluteVelocityXZ);

            //smoothly change value to fix sway stuttering when walking after a jump
            _sway = Vector3.Lerp(_sway, newSway, Time.deltaTime * _swayChangeLerpSpeed);

        }
        else if (!_playerMovement.IsGrounded) //sway if you're in the air or jumping
        {            
            _timer += Time.deltaTime;
            var newSway = GetSway(_timer, _airSwayPeriod, _airSwayAmplitude, _airSwayAmount);

            //smoothly change value to fix sway stuttering when jumping while walking
            _sway = Vector3.Lerp(_sway, newSway, Time.deltaTime * _swayChangeLerpSpeed); 
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