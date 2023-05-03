using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun name", menuName = "Gun Scriptable/Create New Gun")]
public class GunScriptableObject : ScriptableObject
{
    public string gunName;
    [Space]
    public AmmoTypes ammoType;
    public int magazineCapacity;
    public float fireDelay;
    public float timeToResetAccuracy = 1.5f;
    public int gunDamage;
    public int firstAccurateShots = 2;
    public float recoilMultiplier;
    public float spreadAmount;
}
