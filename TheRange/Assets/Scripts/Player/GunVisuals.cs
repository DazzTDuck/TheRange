using System;
using System.Collections;
using UnityEngine;

public class GunVisuals : MonoBehaviour
{
    #region variables

    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ParticleSystem _muzzleFlash;

    #endregion

    private void Start()
    {
        //subscribe to all gun events
        GunHandler.Instance.FireWeaponEvent += PlayFireAnimation;
        GunHandler.Instance.ReloadWeaponEvent += PlayReloadAnimation;
        GunHandler.Instance.SwitchWeaponEvent += PlaySwitchAnimation;      
    }

    private void PlayFireAnimation(object sender, GunHandler.GunEventArgs e)
    {
        //play sound
        _audioSource.PlayOneShot(GunHandler.Instance.GetEquipedGun().data.gunFire);

        float frameToPlay = e.isLastBullet ? 83.0f : 1.0f; //different animation based on last bullet

        _animator.speed = 1.0f; //to reset speed
        PlayAnimatorOnFrame(frameToPlay); //fire animation

        _muzzleFlash.Play();
    }

    private void PlayReloadAnimation(object sender, GunHandler.GunEventArgs e)
    {
        if (e.isCanceled)
        {
            _audioSource.Stop();
            return;
        }

        //play sound
        _audioSource.PlayOneShot(GunHandler.Instance.GetEquipedGun().data.gunReload);

        var frameToPlay = e.isLastBullet ? 95.0f : 12.0f; //different animation based on last bullet

        _animator.speed = 1.0f; //to reset speed
        PlayAnimatorOnFrame(frameToPlay); //reload animation
    }

    private void PlaySwitchAnimation(object sender, GunHandler.GunEventArgs e)
    {
        var frameToPlay = e.gunIsSwitched ? 187.0f : 176.0f; //different animation based if the gun has switched

        //get sound based on switch state
        var sound = e.gunIsSwitched ? GunHandler.Instance.GetEquipedGun().data.gunSlide : GunHandler.Instance.GetEquipedGun().data.gunSwitch;
        //play sound
        _audioSource.PlayOneShot(sound);

        _animator.speed = 1.0f; //to reset speed
        PlayAnimatorOnFrame(frameToPlay); //switch animation
    }

    /// <summary>
    /// Stops the animation from progressing
    /// </summary>
    public void StopAnimating()
    {
        _muzzleFlash.Stop();
        _animator.speed = 0.0f;
    }

    /// <summary>
    /// starts an animation in animator from a cerntain frame
    /// </summary>
    /// <param name="frameNumber">frame to start the animation on</param>
    public void PlayAnimatorOnFrame(float frameNumber)
    {
        _animator.Play("Gun", 0, (1 / 264.0f) * frameNumber); //(1/total_frames)*frame_number
    }


    private void OnDisable()
    {
        //remove events incase object gets removed to avoid errors
        GunHandler.Instance.FireWeaponEvent -= PlayFireAnimation;
        GunHandler.Instance.ReloadWeaponEvent -= PlayReloadAnimation;
        GunHandler.Instance.SwitchWeaponEvent -= PlaySwitchAnimation;
    }
}
