using System;
using TMPro;
using UnityEngine;

public class AmmoHandler : MonoBehaviour
{
    #region variables

    [SerializeField] private TMP_Text _ammoInClipText; 
    [SerializeField] private TMP_Text _ammoInInventoryText;
    [Space]
    [SerializeField] private Ammo[] _allAmmoTypes;

    #endregion
    
    /// <summary>
    /// Get the data for the guns ammo type
    /// </summary>
    /// <param name="ammoType">ammo type of your current gun</param>
    /// <returns>Data for requested ammo</returns>
    public Ammo GetAmmo(AmmoTypes ammoType)
    {
        for (int i = 0; i < _allAmmoTypes.Length; i++)
        {
            if (_allAmmoTypes[i].ammoType == ammoType)
                return _allAmmoTypes[i];
        }

        Debug.LogWarning($"Ammo type Not found:{ammoType}, returning new empty Ammo");
        return new Ammo();
    }

    /// <summary>
    /// Edits the ammo UI
    /// </summary>
    public void UpadteAmmoText(int currentAmmoInClip, int currentAmmoInInventory)
    {
        _ammoInClipText.text = currentAmmoInClip.ToString();
        _ammoInInventoryText.text = $"/ {currentAmmoInInventory}";
    }

    /// <summary>
    /// Give player maximum ammo
    /// </summary>
    public void MaximizeAmmo()
    {
        for (int i = 0; i < _allAmmoTypes.Length; i++)
        {
            _allAmmoTypes[i].ammoInInventory = _allAmmoTypes[i].maxAmmo;
        }
    }
}

[Serializable]
public class Ammo
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
    _9mm, 
    _45acp 

}
