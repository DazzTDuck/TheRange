using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private Action _timerCallback;
    private float _timer;

    public void StartTimer(float timerLength, Action timerCallback)
    {
        _timer = timerLength;
        _timerCallback = timerCallback;
    }

    private void Update()
    {
        if(_timer > 0)
            _timer -= Time.deltaTime;

        if (IsTimerComplete())
        {
            _timerCallback();
        }         
    }

    public bool IsTimerComplete()
    {
        return _timer <= 0;
    }
}
