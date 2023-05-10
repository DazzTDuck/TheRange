using System;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [SerializeField] private GunHandler _gunHandler;
    [SerializeField] private PlayerCamera _camera;
    [SerializeField] private PlayerMovementPhysics _player;
    [Space]
    [SerializeField] private float _recoilRecoverSpeed = 5f;
    [SerializeField] private float _crouchRecoilMultiplier = 0.7f;

    private float _recoilMultipier;
    private Vector3 _recoilAmount;

    private void Start()
    {
        _gunHandler.FireWeaponEvent += AddRecoil;
    }

    private void Update()
    {
        SetRecoilAmount();
    }

    private void SetRecoilAmount()
    {
        _recoilMultipier = _player.IsCrouching ? _crouchRecoilMultiplier : 1;

        if (Mathf.Abs(_recoilAmount.x) > 0 || Mathf.Abs(_recoilAmount.y) > 0 || Mathf.Abs(_recoilAmount.z) > 0)
        {
            _camera.AddToRecoil(_recoilAmount, _recoilMultipier);
            //lerp to zero slowly
            _recoilAmount = Vector3.Lerp(_recoilAmount, Vector3.zero, Time.deltaTime * _recoilRecoverSpeed);
        }
        else
        {
            _camera.AddToRecoil(Vector3.zero, 1);
        }      
    }

    private void AddRecoil(object sender, GunHandler.GunEventArgs e)
    {
        float xAmount = 0;
        float yAmount = 0;
        float zAmount = 0;

        //handle all random recoil amounts for each axis
        if (Mathf.Abs(_gunHandler.GetEquipedGun().data.recoilAmountXMin) > 0)
        {
            xAmount = UnityEngine.Random.Range(_gunHandler.GetEquipedGun().data.recoilAmountXMin, _gunHandler.GetEquipedGun().data.recoilAmountXMax);
        }

        if(Mathf.Abs(_gunHandler.GetEquipedGun().data.recoilAmountYMin) > 0)
        {
            yAmount = UnityEngine.Random.Range(_gunHandler.GetEquipedGun().data.recoilAmountYMin, _gunHandler.GetEquipedGun().data.recoilAmountYMax); ;
        }

        if(Mathf.Abs(_gunHandler.GetEquipedGun().data.recoilAmountZMin) > 0)
        {
            zAmount = UnityEngine.Random.Range(_gunHandler.GetEquipedGun().data.recoilAmountZMin, _gunHandler.GetEquipedGun().data.recoilAmountZMax);
        }

        _recoilAmount -= new Vector3(xAmount, yAmount, zAmount);
    }

    private void OnDisable()
    {
        _gunHandler.FireWeaponEvent -= AddRecoil;
    }
}
