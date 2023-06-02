using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    #region variables

    [SerializeField] private PlayerMovementPhysics _playerMovement;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _footsteps;

    [SerializeField] private float _timingAmplitude = 1f;
    [SerializeField] private float _timingPeriod = 0.1f;

    private float _footstepTiming;
    private float _timer;
    private bool _footstepPlaying;

    #endregion

    private void Update()
    {
        HandleFootstepTiming();

        if(_footstepTiming < 0f && !_footstepPlaying)
        {
            _footstepPlaying = true;
            PlayFootstepSound();
        }

        if (_footstepTiming > 0f && _footstepPlaying)
        {
            _footstepPlaying = false;
        }
    }

    private void HandleFootstepTiming()
    {
        //get the absolute velocity from the x and z axis
        float absoluteVelocity = Mathf.Abs(_playerMovement.GetRigidbody().velocity.x) + Mathf.Abs(_playerMovement.GetRigidbody().velocity.z) / 2;

        if (absoluteVelocity < 0.001) //to make sure there are no VERY low numbers
            absoluteVelocity = 0;

        //Camera bobbing up and down
        if (_playerMovement.GetRigidbody().velocity.magnitude > 0.2f && _playerMovement.IsGrounded)
        {
            _timer += Time.deltaTime;
            _footstepTiming = GetTiming(_timer, _timingPeriod, _timingAmplitude, absoluteVelocity);

        }
        else if (_playerMovement.GetRigidbody().velocity.magnitude < 0.2f && _playerMovement.IsGrounded)
        {
            _timer = 0;
            _footstepTiming = 0; //smoothly go back
        }
    }

    private float GetTiming(float timer, float period, float amplitude, float absoluteVelocity = 1)
    {
        float theta = timer / (period);
        float distance = (amplitude * absoluteVelocity) * Mathf.Sin(theta);
        return distance;
    }

    private void PlayFootstepSound()
    {
        AudioClip randomFootstep = _footsteps[Random.Range(0, _footsteps.Length)];
        _audioSource.PlayOneShot(randomFootstep);
    }
}
