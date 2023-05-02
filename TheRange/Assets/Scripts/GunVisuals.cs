using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunVisuals : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _muzzleFlash;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _animator.speed = 1.0f; //to reset speed
            PlayAnimatorOnFrame(1); //fire animation

            _muzzleFlash.Play();
        }
    }

    public void StopAnimating()
    {
        _muzzleFlash.Stop();
        _animator.speed = 0;
    }

    public void PlayAnimatorOnFrame(int frameNumber)
    {
        _animator.Play("Gun", 0, (1 / 264) * frameNumber); //(1/total_frames)*frame_number
    }
}
