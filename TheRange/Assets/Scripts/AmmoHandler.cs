using System;
using UnityEngine;

public class AmmoHandler : MonoBehaviour
{
    [SerializeField] private Ammo[] _allAmmoTypes;

    private void Start()
    {
        Debug.Log(GetAmmo(AmmoTypes._9mm).maxAmmo);
    }

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
    public int maxAmmo;
    public int ammoAvalable;
}

public enum AmmoTypes
{
    _9mm,
    _45acp

}
