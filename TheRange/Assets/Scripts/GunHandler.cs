using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHandler : MonoBehaviour
{
    public static GunHandler Instance { get; private set; }

    #region variables

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private AmmoHandler _ammoHandler;
    [SerializeField] private PlayerMovementPhysics _player;
    [SerializeField] private GameObject _bulletHoleDecal;
    [Space]
    [SerializeField] private GunInfoHolder[] _allGuns;
    [SerializeField] private float _SpreadResetSpeed = 2;
    [SerializeField] private float _SpreadWalkingMultiplier = 3;
    [SerializeField] private float _SpreadCrouchMultiplier = 0.65f;

    private bool _isReloading = false;
    private bool _isFireing = false;
    private bool _isEquiping = false;
    private bool _wantsToFire = false;
    private bool _wantsToReload = false;
    private float _spreadAmount;
    private float _spreadMultiplier;
    private int _shotsFiredInARow;
    private float _shotTimer;
    private int _equipedIndex = 0;

    public EventHandler<GunEventArgs> FireWeaponEvent;
    public EventHandler<GunEventArgs> ReloadWeaponEvent;
    public EventHandler<GunEventArgs> SwitchWeaponEvent;
    public class GunEventArgs : EventArgs
    {
        public bool isLastBullet = false;
        public bool gunIsSwitched = false;
    }

    private Ray ray;
    private RaycastHit hit;

    #endregion

    private void Awake()
    {
        //If there is an instance, and it's not this, delete myself.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        //set first gun in "_allGuns" to be the equiped gun
        _equipedIndex = 0;
        EnableEquipedGun();

        //give all guns ammo
        SetAmmoOfAllGuns();
        //set ammo text
        UpdateAmmoVisual();
    }

    private void Update()
    {
        FireWeapon();
        ReloadWeapon();
        ShootingTimer();
        SwitchWeapon();
    }

    private void FireWeapon()
    {
        //lerp spread amount back down to 0
        if (_spreadAmount > 0)
            _spreadAmount = Mathf.Lerp(_spreadAmount, 0, Time.deltaTime * _SpreadResetSpeed);

        if (_spreadAmount < 0.1f) //reset to 0 if too low
            _spreadAmount = 0;

        //get fire input
        _wantsToFire = GetEquipedGun().data.automatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        //weapon fire conditions
        if (_wantsToFire && !_isFireing && !_isReloading && !_isEquiping)
        {
            //if wants to fire but ammo in clip in 0, force a reload
            if(GetEquipedGun().ammoInClip <= 0)
            {
                ReloadWeapon(true);
                return;
            }

            //calculate spread of shooting
            float randomSpreadX = UnityEngine.Random.Range(-_spreadAmount, _spreadAmount);
            float randomSpreadY = UnityEngine.Random.Range(-_spreadAmount, _spreadAmount);
            
            //spread multiplier for crouching and walking
            if(_player.GetRigidbody().velocity.magnitude > 0.5f)
            {
                //if walking, 
                _spreadMultiplier = _SpreadWalkingMultiplier;
            }
            else
            {
                // if not walking and/or crouching
                _spreadMultiplier = _player .IsCrouching ? _SpreadCrouchMultiplier : 1;
            }
               
            var newMousePosition = new Vector3(Input.mousePosition.x + (randomSpreadX * _spreadMultiplier), Input.mousePosition.y + (randomSpreadY * _spreadMultiplier), 0);
            ray = Camera.main.ScreenPointToRay(newMousePosition); //set ray point (center of screen (+spread) because mouse is locked)

            //set variables
            _isFireing = true;
            _shotsFiredInARow++;
            _shotTimer = GetEquipedGun().data.timeToResetAccuracy;
                
            //is amount fired is above the firstAccurateShots, increase the spread
            if (_shotsFiredInARow > GetEquipedGun().data.firstAccurateShots)
            {
                _spreadAmount += GetEquipedGun().data.spreadAmount;
            }

            //Shooting logic
            if (Physics.Raycast(ray, out hit, GetEquipedGun().data.maxFireDistance))
            {
                if (hit.collider)
                {
                    //instantiate bullet hole on object hit
                    var bulletHole = Instantiate(_bulletHoleDecal, hit.point, Quaternion.LookRotation(-hit.normal));
                    bulletHole.transform.SetParent(hit.collider.transform, true);

                    //find hittable object and trigger hit function
                    if(hit.collider.TryGetComponent<IHittable>(out var hittable)) 
                    {
                        hittable.OnHit(_ammoHandler.GetAmmo(GetEquipedGun().data.ammoType).ammoDamage);
                    }
                }
            }

            //take off the ammo
            GetEquipedGun().ammoInClip--;
            UpdateAmmoVisual();

            //start timer
            var fireTimer = gameObject.AddComponent<Timer>();
            fireTimer.StartTimer(GetEquipedGun().data.fireDelay, () => { _isFireing = false; Destroy(fireTimer); });

            //activates fire weapon event
            FireWeaponEvent?.Invoke(this, new GunEventArgs { isLastBullet = GetEquipedGun().ammoInClip == 0 } );
        }
    }

    private void ReloadWeapon(bool bypassInput = false, bool lastBullet = false)
    {
        //for if you want to force a reload by code
        if (!bypassInput)
            _wantsToReload = Input.GetButtonDown("Reload");
        else
            _wantsToReload = bypassInput;


        //check all booleans and if ammo has been used and if there is enough reserve to reload
        if (_wantsToReload && !_isFireing && !_isReloading && !_isEquiping && 
            GetEquipedGun().ammoInClip < GetEquipedGun().data.magazineCapacity && _ammoHandler.GetAmmo(GetEquipedGun().data.ammoType).ammoInInventory > 0)
        {
            _shotsFiredInARow = 0;
            _isReloading = true;        

            //start timer, at the end of timer reset everything and update ammo count
            var reloadTimer = gameObject.AddComponent<Timer>();
            reloadTimer.StartTimer(GetEquipedGun().data.reloadDelay, () => { AmmoReloadUpdate(); _isReloading = false; Destroy(reloadTimer); });

            ReloadWeaponEvent?.Invoke(this, new GunEventArgs { isLastBullet = GetEquipedGun().ammoInClip == 0 });
        }
    }

    public void AmmoReloadUpdate()
    {
        //Reload logic
        var reloadAmount = GetEquipedGun().data.magazineCapacity - GetEquipedGun().ammoInClip;

        if(_ammoHandler.GetAmmo(GetEquipedGun().data.ammoType).ammoInInventory - reloadAmount < 0)
        {
           var difference = _ammoHandler.GetAmmo(GetEquipedGun().data.ammoType).ammoInInventory - reloadAmount;
           reloadAmount += difference;
        }

        GetEquipedGun().ammoInClip += reloadAmount;
        _ammoHandler.GetAmmo(GetEquipedGun().data.ammoType).ammoInInventory -= reloadAmount;
        UpdateAmmoVisual();
    }

    private void SwitchWeapon()
    {
        var mouseWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
        
        if(Mathf.Abs(mouseWheelInput) > 0 && !_isEquiping && !_isReloading && !_isFireing)
        {
            //enable bool and up index
            _isEquiping = true;

            //equip timer, at the end of timer reset everything so you can switch again
            var equipTimer = gameObject.AddComponent<Timer>();
            equipTimer.StartTimer(GetEquipedGun().data.switchDelay, () => { SwitchWeaponEvent?.Invoke(this, new GunEventArgs { gunIsSwitched = true }); SecondSwitchTrigger(); Destroy(equipTimer);});

            //change index based on what way you scroll the mouse
            if (mouseWheelInput < 0)
                _equipedIndex++;
            else if (mouseWheelInput > 0)
                _equipedIndex--;

            if (_equipedIndex >= _allGuns.Length) //make sure the index is corrext
                _equipedIndex = 0;
            else if (_equipedIndex < 0)
                _equipedIndex = _allGuns.Length - 1; //go to the last weapon

            SwitchWeaponEvent?.Invoke(this, new GunEventArgs{ gunIsSwitched = false });
        }
    }

    private void SecondSwitchTrigger()
    {
        //switch the equiped gun and enable models of the equiped gun
        EnableEquipedGun();
        UpdateAmmoVisual();

        //equip timer, at the end of timer reset everything so you can switch again
        var equipTimer = gameObject.AddComponent<Timer>();
        equipTimer.StartTimer(GetEquipedGun().data.switchRecoverDelay, () => { _isEquiping = false; Destroy(equipTimer); });
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

    public void UpdateAmmoVisual()
    {
        _ammoHandler.EditAmmoText(GetEquipedGun().ammoInClip, _ammoHandler.GetAmmo(GetEquipedGun().data.ammoType).ammoInInventory);
    }

    private void EnableEquipedGun()
    {
        for (int i = 0; i < _allGuns.Length; i++)
        {
            _allGuns[i].gunObject.SetActive(false);
        }

        GetEquipedGun().gunObject.SetActive(true);
    }

    private void SetAmmoOfAllGuns()
    {
        for (int i = 0; i < _allGuns.Length; i++)
        {
            //set ammo in clip for each gun so all have ammo from start
            _allGuns[i].ammoInClip = _allGuns[i].data.magazineCapacity;
        }
    }

    public float GetSpreadAmount()
    {
        return _spreadAmount * _spreadMultiplier;
    }

    public ref GunInfoHolder GetEquipedGun()
    {
        return ref _allGuns[_equipedIndex];
    }
}

[Serializable]
public struct GunInfoHolder
{
    public GameObject gunObject;
    public int ammoInClip;
    public GunScriptableObject data;
}
