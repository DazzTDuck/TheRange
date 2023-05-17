using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class ArmsFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _toFollowPosition;
    [SerializeField] private Transform _toFollowRotation;
    [SerializeField] private float _lerpSpeedPosition;
    [SerializeField] private float _lerpSpeedRotation;
    [SerializeField] private float _smoothTime = 0.5f;

    private Vector3 _positionOffset;
    private float velocity;
    float yPosition;

    private void Awake()
    {
        _positionOffset = transform.position - _toFollowPosition.position; //calcualte position difference to make an offset
    }
  
    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, _toFollowRotation.rotation, Time.deltaTime * _lerpSpeedRotation);

        var position = _toFollowPosition.position + _positionOffset;
        transform.position = new Vector3(position.x, transform.position.y, position.z);
    }

    private void FixedUpdate()
    {
        var position = _toFollowPosition.position + _positionOffset;

        yPosition = Mathf.SmoothDamp(yPosition, position.y, ref velocity, _smoothTime);
        var ylerped = LerpAxis(transform.position.y, yPosition);
        transform.position = new Vector3(transform.position.x, ylerped, transform.position.z);
    }

    private float LerpAxis(float axis, float target)
    {
        return Mathf.Lerp(axis, target, Time.fixedDeltaTime * _lerpSpeedPosition);
    }
}
