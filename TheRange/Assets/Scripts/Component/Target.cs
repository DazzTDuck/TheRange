using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IHittable
{
    #region variables

    [SerializeField] private int _pointsGetOnHit;
    [SerializeField] private Vector3 _rotationWhenHit;
    [Space]
    [SerializeField] private bool _rotateOnHit = false;
    [SerializeField] private bool _rotateShortWay = false;
    [SerializeField] private float _rotateLerpSpeed = 3f;
    [SerializeField] private bool _resetRotationAfterTime = false;
    [SerializeField] private float _resetRotationTime = 4f;

    private Quaternion _currentRotation;
    private Quaternion _nextRotation;
    private bool _rotate = false;
    private bool _canBeHit = true;

    #endregion

    private void Start()
    {
        _currentRotation = transform.localRotation;
    }

    private void Update()
    {
        if (!_rotateOnHit)
            return;

        var rotationTo = _rotate ? _nextRotation : _currentRotation;
        transform.localRotation = Lerp(transform.localRotation, rotationTo, _rotateLerpSpeed * Time.deltaTime, _rotateShortWay);
    }

    public void OnHit(float damage)
    {
        if (!_canBeHit)
            return;

        //rotate object
        SwitchRotation();

        //register points
        GameManager.Instance.AddPoints(_pointsGetOnHit);

        //start timer if 
        if (_resetRotationAfterTime)
        {
            _canBeHit = false;

            //start timer
            var timer = gameObject.AddComponent<Timer>();
            timer.StartTimer(_resetRotationTime, () => { _canBeHit = true; SwitchRotation(); Destroy(timer); });
        }
    }

    public void SwitchRotation()
    {
        //swap between rotations
        _nextRotation = Quaternion.Euler(_rotationWhenHit);
        _rotate = !_rotate;
    }

    //NOT WRITTEN BY ME, FOUND THIS TO HELP MY PROBLEM
    public static Quaternion Lerp(Quaternion p, Quaternion q, float t, bool shortWay)
    {
        if (shortWay)
        {
            float dot = Quaternion.Dot(p, q);
            if (dot < 0.0f)
                return Lerp(ScalarMultiply(p, -1.0f), q, t, true);
        }

        Quaternion r = Quaternion.identity;
        r.x = p.x * (1f - t) + q.x * (t);
        r.y = p.y * (1f - t) + q.y * (t);
        r.z = p.z * (1f - t) + q.z * (t);
        r.w = p.w * (1f - t) + q.w * (t);
        return r;
    }
    //NOT WRITTEN BY ME, FOUND THIS TO HELP MY PROBLEM
    public static Quaternion ScalarMultiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }
}
