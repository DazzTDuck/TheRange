using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class LerpFollow : MonoBehaviour
{
    [SerializeField] private Transform _toFollowPosition;
    [SerializeField] private Transform _toFollowRotation;
    [Space]
    [Tooltip("Set lerp speed to 0 to instantly set the position")]
    [SerializeField] private float _lerpSpeedPosition;
    [SerializeField] private float _lerpSpeedRotation;
    [Space]
    [SerializeField] private bool _followPosition;
    [SerializeField] private Vector3 _axisToLerp;
    [SerializeField] private Vector3 _axisToFollow;
    [SerializeField] private bool _followByOffset;
    [Space]
    [SerializeField] private bool _followRotation;

    private Vector3 _positionOffset;

    /// <summary>
    /// This is to control which axis that is following you want the value to be lerped
    /// </summary>
    private Vector3 _positionToLerp;
    /// <summary>
    /// This is to control on which axis the following occurs, if set to 0 then the position will be set to 0 on that axis
    /// </summary>
    private Vector3 _positionToFollow;


    private float _xlerped = 0;
    private float _ylerped = 0;
    private bool[] _lerpAxis = {false, false, false};
    private float _zlerped = 0;

    private void Awake()
    {
        _positionOffset = transform.position - _toFollowPosition.position; //calcualte position difference to make an offset
    }
  
    private void Update()
    {
        if(_followPosition)
        {
            var offset = _followByOffset ? _positionOffset : Vector3.zero;
            var position = _toFollowPosition.position + offset;

            if (_lerpSpeedPosition <= 0)
            {
                transform.position = position;
            }
            else
            {
                _positionToLerp = new Vector3(position.x * _axisToLerp.x, position.y * _axisToLerp.y, position.z * _axisToLerp.z);
                _positionToFollow = new Vector3(position.x * _axisToFollow.x, position.y * _axisToFollow.y, position.z * _axisToFollow.z);

                //x value lerped
                LerpAxisToPosition(_positionToLerp.x, _positionToFollow.x, ref _lerpAxis[0], ref _xlerped, position.x, transform.position.x);

                //y value lerped
                LerpAxisToPosition(_positionToLerp.y, _positionToFollow.y, ref _lerpAxis[1], ref _ylerped, position.y, transform.position.y);

                //z value lerped
                LerpAxisToPosition(_positionToLerp.z, _positionToFollow.z, ref _lerpAxis[2], ref _zlerped, position.z, transform.position.z);

                transform.position = new Vector3(_xlerped, _ylerped, _zlerped); //setting the values
            }      
        }

        if (_followRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _toFollowRotation.rotation, Time.deltaTime * _lerpSpeedRotation);
        }
    }

    private void FixedUpdate()
    {
        //set lerps in fixed update to avoid stuttering

        if (_lerpAxis[0] == true)
            _xlerped = LerpAxisTo(_xlerped, _positionToLerp.x); 

        if (_lerpAxis[1] == true)
            _ylerped = LerpAxisTo(_ylerped, _positionToLerp.y);

        if(_lerpAxis[2] == true)
            _zlerped = LerpAxisTo(_zlerped, _positionToLerp.z);
    }

    private float LerpAxisTo(float axis, float target)
    {
        return Mathf.Lerp(axis, target, Time.fixedDeltaTime * _lerpSpeedPosition);
    }

    private void LerpAxisToPosition(float positionToLerp, float positionToFollow, ref bool boolAxis, ref float axisLerped, float axisPosition, float followIfZero)
    {
        if (positionToLerp > 0)
            boolAxis = true; //start lerping
        else
        {
            axisLerped = Mathf.Abs(positionToFollow) > 0 ? axisPosition : followIfZero; //control if the axis gets followed at all
            boolAxis = false;
        }
    }
}
