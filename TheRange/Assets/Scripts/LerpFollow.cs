using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpFollow : MonoBehaviour
{
    [SerializeField] private Transform _toFollow;
    [Space]
    [Tooltip("Set lerp speed to 0 to instantly set the position")]
    [SerializeField] private float _lerpSpeedPosition;
    [SerializeField] private float _lerpSpeedRotation;
    [Space]
    [SerializeField] private bool _followPosition;
    [SerializeField] private Vector3 _axisToLerp;
    [SerializeField] private bool _followByOffset;
    [Space]
    [SerializeField] private bool _followRotation;

    private Vector3 _positionOffset;

    private void Awake()
    {
        _positionOffset = transform.position - _toFollow.position; //calcualte position difference to make an offset
    }

    
    private void Update()
    {
        if(_followPosition)
        {
            var offset = _followByOffset ? _positionOffset : Vector3.zero;

            if(_lerpSpeedPosition <= 0)
            {
                transform.position = _toFollow.position + offset;
            }
            else
            {
                //-FIX THIS STUFF-
                float xlerped = 0;
                float ylerped = 0; 
                float zlerped = 0;

                var position = _toFollow.position + offset;
                var positionToLerp = new Vector3(position.x * _axisToLerp.x, position.y * _axisToLerp.y, position.z * _axisToLerp.z);

                //x value lerped
                if (positionToLerp.x > 0) 
                    xlerped = Mathf.Lerp(xlerped, positionToLerp.x, Time.deltaTime * _lerpSpeedPosition);
                else
                    xlerped = position.x;

                //y value lerped
                if (positionToLerp.y > 0) 
                    ylerped = Mathf.Lerp(ylerped, positionToLerp.y, Time.deltaTime * _lerpSpeedPosition);
                else
                    ylerped = position.y;

                //z value lerped
                if (positionToLerp.z > 0) 
                    zlerped = Mathf.Lerp(zlerped, positionToLerp.z, Time.deltaTime * _lerpSpeedPosition);
                else
                    zlerped = position.z;

                //transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * _lerpSpeedPosition);
                transform.position = new Vector3(xlerped, ylerped, zlerped);
            }      
        }

        if (_followRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _toFollow.rotation, Time.deltaTime * _lerpSpeedRotation);
        }
    }
}
