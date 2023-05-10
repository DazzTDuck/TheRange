using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHandler : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private AmmoHandler _ammoHandler;
    [SerializeField] private PlayerMovementPhysics _player;
    [SerializeField] private GameObject _bulletHoleDecal;
    [Space]
    [SerializeField] private GunInfoHolder[] _allGuns;
    [SerializeField] private float _SpreadResetSpeed = 2;
    [SerializeField] private float _SpreadWalkingMultiplier = 3;

    private bool _isReloading = false;
    private bool _isFireing = false;
    private bool _isEquiping = false;
    private bool _wantsToFire = false;
    private bool _wantsToReload = false;
    private float _spreadAmount;
    private int _shotsFiredInARow;
    private float _shotTimer;

    private GunInfoHolder _equipedGun;

    public EventHandler<GunEventArgs> FireWeaponEvent;
    public EventHandler<GunEventArgs> ReloadWeaponEvent;
    public class GunEventArgs : EventArgs
    {
        public bool isLastBullet = false;
    }

    private Ray ray;
    private RaycastHit hit;

    private void Start()
    {
        //set first gun in "_allGuns" to be the equiped gun
        _equipedGun = _allGuns[0];
        EnableEquipedGun();

        //set ammo in the clip for the first time
        _equipedGun.ammoInClip = _equipedGun.data.magazineCapacity;
        //set ammo text
        UpdateAmmoVisual();
    }

    private void Update()
    {
        FireGun();
        GunReload();
        ShootingTimer();
    }

    private void FireGun()
    {
        //lerp spread amount back down to 0
        if (_spreadAmount > 0)
            _spreadAmount = Mathf.Lerp(_spreadAmount, 0, Time.deltaTime * _SpreadResetSpeed);

        if (_spreadAmount < 0.1f) //reset to 0 if too low
            _spreadAmount = 0;

        //get fire input
        _wantsToFire = _equipedGun.data.automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        //if wants to fire but ammo in clip in 0, force a reload
        if (_wantsToFire && !_isFireing && !_isReloading && !_isEquiping && _equipedGun.ammoInClip == 0)
        {
            GunReload(true);
            return;
        }

        if (_wantsToFire && !_isFireing && !_isReloading && !_isEquiping && _equipedGun.ammoInClip > 0)
        {
            //calculate spread of shooting
            var randomSpreadX = UnityEngine.Random.Range(-_spreadAmount, _spreadAmount);
            var randomSpreadY = UnityEngine.Random.Range(-_spreadAmount, _spreadAmount);
            var spreadMultiplier = _player.GetRigidbody().velocity.magnitude > 0.2 ? _SpreadWalkingMultiplier : 1;

            var newMousePosition = new Vector3(Input.mousePosition.x + (randomSpreadX * spreadMultiplier), Input.mousePosition.y + (randomSpreadY * spreadMultiplier), 0);
            ray = Camera.main.ScreenPointToRay(newMousePosition); //set ray point (center of screen (+spread) because mouse is locked)

            //set variables
            _isFireing = true;
            _shotsFiredInARow++;
            _shotTimer = _equipedGun.data.timeToResetAccuracy;

            //is amount fired is above the firstAccurateShots, increase the spread
            if (_shotsFiredInARow > _equipedGun.data.firstAccurateShots)
            {
                _spreadAmount += _equipedGun.data.spreadAmount;
            }

            //Shooting logic
            if (Physics.Raycast(ray, out hit, _equipedGun.data.maxFireDistance))
            {
                if (hit.collider)
                {
                    //instantiate bullet hole on object hit
                    Instantiate(_bulletHoleDecal, hit.point, Quaternion.LookRotation(-hit.normal));

                    //find damageable object / hittable object and trigger hit function

                    //take off the ammo
                    _equipedGun.ammoInClip--;
                    UpdateAmmoVisual();
                }
            }  

            //start timer
            var fireTimer = gameObject.AddComponent<Timer>();
            fireTimer.StartTimer(_equipedGun.data.fireDelay, () => { _isFireing = false; Destroy(fireTimer); });

            //activates fire weapon event
            FireWeaponEvent?.Invoke(this, new GunEventArgs { isLastBullet = _equipedGun.ammoInClip == 0 } );
        }
    }

    private void GunReload(bool bypassInput = false, bool lastBullet = false)
    {
        //for if you want to force a reload by code
        if (!bypassInput)
            _wantsToReload = Input.GetButtonDown("Reload");
        else
            _wantsToReload = bypassInput;


        //check all booleans and if ammo has been used and if there is enough reserve to reload
        if (_wantsToReload && !_isFireing && !_isReloading && !_isEquiping && 
            _equipedGun.ammoInClip < _equipedGun.data.magazineCapacity && _ammoHandler.GetAmmo(_equipedGun.data.ammoType).ammoInInventory > 0)
        {
            _shotsFiredInARow = 0;
            _isReloading = true;        

            //start timer, at the end of timer reset everything and update ammo count
            var reloadTimer = gameObject.AddComponent<Timer>();
            reloadTimer.StartTimer(_equipedGun.data.reloadDelay, () => { AmmoReloadUpdate(); _isReloading = false; Destroy(reloadTimer); });

            ReloadWeaponEvent?.Invoke(this, new GunEventArgs { isLastBullet = _equipedGun.ammoInClip == 0 });
        }
    }

    public void AmmoReloadUpdate()
    {
        //Reload logic
        var reloadAmount = _equipedGun.data.magazineCapacity - _equipedGun.ammoInClip;

        if(_ammoHandler.GetAmmo(_equipedGun.data.ammoType).ammoInInventory - reloadAmount < 0)
        {
           var difference = _ammoHandler.GetAmmo(_equipedGun.data.ammoType).ammoInInventory - reloadAmount;
           reloadAmount += difference;
        }

        _equipedGun.ammoInClip += reloadAmount;
        _ammoHandler.GetAmmo(_equipedGun.data.ammoType).ammoInInventory -= reloadAmount;
        UpdateAmmoVisual();
    }

    private void ShootingTimer()
    {
        if (_shotTimer > 0)
            _shotTimer -= Time.deltaTime;
        else
        {
            _shotTimer = 0;
            _shotsFiredInARow = 0;
        }      
    }

    private void UpdateAmmoVisual()
    {
        _ammoHandler.EditAmmoText(_equipedGun.ammoInClip, _ammoHandler.GetAmmo(_equipedGun.data.ammoType).ammoInInventory);
    }

    private void EnableEquipedGun()
    {
        for (int i = 0; i < _allGuns.Length; i++)
        {
            _allGuns[i].gunObject.SetActive(false);
        }

        _equipedGun.gunObject.SetActive(true);
    }

    public GunInfoHolder GetEquipedGun()
    {
        return _equipedGun;
    }
}

[Serializable]
public struct GunInfoHolder
{
    public GameObject gunObject;
    public int ammoInClip;
    public GunScriptableObject data;
}
