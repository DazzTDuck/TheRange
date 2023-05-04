using System;
using UnityEngine;

public class AmmoHandler : MonoBehaviour
{
    [SerializeField] private Ammo[] _allAmmoTypes;


    public Ammo GetAmmo(AmmoTypes ammoType)
    {
        for (int i = 0; i < _allAmmoTypes.Length; i++)
        {
            if (_allAmmoTypes[i].ammoType == ammoType)
                return _allAmmoTypes[i];
        }

        Debug.LogWarning($"Ammo type Not found:{ammoType}, returning new Ammo()");
        return new Ammo();
    }
}

[Serializable]
public struct Ammo
{
    public string name;
    [Space]
    public AmmoTypes ammoType;
    [Space]
    public int maxAmmo;
    public int ammoInInventory;
    [Space]
    public int ammoDamage;
    public float ammoRecoilPower;
}

public enum AmmoTypes
{
    _9mm, //smaller bullet, faster, less recoil, less power, less damage
    _45acp //bigger buller, slower, more recoil, more power, more damage

}
