using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunVisuals : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ParticleSystem _muzzleFlash;
    

    private void Start()
    {
        GunHandler.FireWeaponEvent += PlayFireAnimation;
        GunHandler.ReloadWeaponEvent += PlayReloadAnimation;
    }

    private void PlayFireAnimation(object sender, GunHandler.GunEventArgs e)
    {
        //play sound
        _audioSource.PlayOneShot(GunHandler.GetEquipedGun().data.gunFire);

        var frameToPlay = e.isLastBullet ? 83.0f : 1.0f; //different animation based on last bullet

        _animator.speed = 1.0f; //to reset speed
        PlayAnimatorOnFrame(frameToPlay); //fire animation

        _muzzleFlash.Play();
    }

    private void PlayReloadAnimation(object sender, GunHandler.GunEventArgs e)
    {
        //play sound
        _audioSource.PlayOneShot(GunHandler.GetEquipedGun().data.gunReload);

        var frameToPlay = e.isLastBullet ? 95.0f : 12.0f; //different animation based on last bullet

        _animator.speed = 1.0f; //to reset speed
        PlayAnimatorOnFrame(frameToPlay); //fire animation
    }

    public void StopAnimating()
    {
        _muzzleFlash.Stop();
        _animator.speed = 0;
    }

    public void PlayAnimatorOnFrame(float frameNumber)
    {
        _animator.Play("Gun", 0, (1 / 264.0f) * frameNumber); //(1/total_frames)*frame_number
    }


    private void OnDisable()
    {
        GunHandler.FireWeaponEvent -= PlayFireAnimation;
        GunHandler.ReloadWeaponEvent -= PlayReloadAnimation;
    }
}
