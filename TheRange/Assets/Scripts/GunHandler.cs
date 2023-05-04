using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHandler : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private GunInfoHolder[] _allGuns;
    [Space]
    [SerializeField] private AmmoHandler _ammoHandler;

    private bool _isReloading = false;
    private bool _isFireing = false;
    private bool _isEquiping = false;
    private bool _wantsToFire = false;

    private GunInfoHolder _equipedGun;

    public EventHandler FireWeaponEvent;
    public EventHandler ReloadWeaponEvent;

    private void Start()
    {
        //set first gun in "_allGuns" to be the equiped gun
        _equipedGun = _allGuns[0];
        EnableEquipedGun();

    }

    private void Update()
    {
        FireGun();
        GunReload();
    }

    private void FireGun()
    {
        _wantsToFire = _equipedGun.data.automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (_wantsToFire && !_isFireing && !_isReloading && !_isEquiping)
        {
            _isFireing = true;

            //Shooting logic


            //start timer
            var fireTimer = gameObject.AddComponent<Timer>();
            fireTimer.StartTimer(_equipedGun.data.fireDelay, () => { _isFireing = false; Destroy(fireTimer); });

            //activates fire weapon event
            FireWeaponEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GunReload()
    {
        if (Input.GetButtonDown("Reload") && !_isFireing && !_isReloading && !_isEquiping)
        {
            _isReloading = true;

            //Reload logic


            //start timer
            var reloadTimer = gameObject.AddComponent<Timer>();
            reloadTimer.StartTimer(_equipedGun.data.reloadDelay, () => { _isReloading = false; Destroy(reloadTimer); });

            ReloadWeaponEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    private void EnableEquipedGun()
    {
        for (int i = 0; i < _allGuns.Length; i++)
        {
            _allGuns[i].gunObject.SetActive(false);
        }

        _equipedGun.gunObject.SetActive(true);
    }
}

[Serializable]
public struct GunInfoHolder
{
    public GameObject gunObject;
    public GunScriptableObject data;
}
