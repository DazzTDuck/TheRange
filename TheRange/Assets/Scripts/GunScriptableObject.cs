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
    public bool automatic;
    public float fireDelay;
    public float reloadDelay;
    public float timeToResetAccuracy = 1.5f;
    public int firstAccurateShots = 2;
    public float recoilMultiplier;
    public float spreadAmount;
}
