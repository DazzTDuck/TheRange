using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class ArmsFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _lerpSpeedPosition;
    [SerializeField] private float _lerpSpeedRotation;

    private Vector3 _positionOffset;
    private Vector3 _newPosition;

    private void Awake()
    {
        _positionOffset = transform.position - _cameraTransform.position; //calcualte position difference to make an offset
    }

    private void LateUpdate()
    {
        var position = _cameraTransform.position + _positionOffset;

        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * _lerpSpeedPosition);
        //transform.position = position;
        transform.rotation = Quaternion.Lerp(transform.rotation, _cameraTransform.rotation, Time.deltaTime * _lerpSpeedRotation);
    }
}
