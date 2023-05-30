using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunVisuals : MonoBehaviour
{
    #region variables

    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private RectTransform[] _spreadIndicators = new RectTransform[4];

    public bool IndicateSpread { get => _indicateSpread; set { _indicateSpread = value; } }

    private bool _indicateSpread = true;
    private Vector3[] _originalPositionsSpreads = new Vector3[4];

    #endregion

    private void Start()
    {
        //subscribe to all gun events
        GunHandler.Instance.FireWeaponEvent += PlayFireAnimation;
        GunHandler.Instance.ReloadWeaponEvent += PlayReloadAnimation;
        GunHandler.Instance.SwitchWeaponEvent += PlaySwitchAnimation;

        //set all original positions of crosshair lines
        for (int i = 0; i < _spreadIndicators.Length; i++)
        {
            _originalPositionsSpreads[i] = _spreadIndicators[i].localPosition;
        }
        
    }

    private void Update()
    {
        if (!_indicateSpread)
            return;

        for (int i = 0; i < _spreadIndicators.Length; i++)
        {
            //always update spread indicators (crosshair)
            if (GunHandler.Instance.GetSpreadAmount() > 0)
            {
                //i % 2, the second in the array needs to go in the negative direction
                float spreadAmount = i % 2 == 0 ? -GunHandler.Instance.GetSpreadAmount() : GunHandler.Instance.GetSpreadAmount();
                //i >= 2, the top and bottom crosshair lines need to move via the Y axis
                Vector3 newVector = i >= 2 ? new Vector3(0, spreadAmount, 0) : new Vector3(spreadAmount, 0, 0);

                //apply per indicator
                Vector3 newPos = _originalPositionsSpreads[i] + newVector;
                _spreadIndicators[i].localPosition = newPos;
            }
            else
            {
                //reset when no spread
                _spreadIndicators[i].localPosition = _originalPositionsSpreads[i];
            }
        }   
    }

    private void PlayFireAnimation(object sender, GunHandler.GunEventArgs e)
    {
        //play sound
        _audioSource.PlayOneShot(GunHandler.Instance.GetEquipedGun().data.gunFire);

        var frameToPlay = e.isLastBullet ? 83.0f : 1.0f; //different animation based on last bullet

        _animator.speed = 1.0f; //to reset speed
        PlayAnimatorOnFrame(frameToPlay); //fire animation

        _muzzleFlash.Play();
    }

    private void PlayReloadAnimation(object sender, GunHandler.GunEventArgs e)
    {
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

    public void StopAnimating()
    {
        _muzzleFlash.Stop();
        _animator.speed = 0.0f;
    }

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
