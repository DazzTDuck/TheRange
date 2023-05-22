using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class ArmsFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _lerpSpeedPosition;
    [SerializeField] private float _lerpSpeedRotation;

    private Vector3 _positionOffset;
    private float _yLerped;

    private void Awake()
    {
        _positionOffset = transform.position - _cameraTransform.position; //calcualte position difference to make an offset
    }
  
    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, _cameraTransform.rotation, Time.smoothDeltaTime * _lerpSpeedRotation);

        var position = _cameraTransform.position + _positionOffset;
        _yLerped = LerpAxis(transform.position.y, position.y);

        transform.position = new Vector3(position.x, _yLerped, position.z);
    }

    private float LerpAxis(float axis, float target)
    {
        return Mathf.Lerp(axis, target, Time.smoothDeltaTime * _lerpSpeedPosition);
    }
}
