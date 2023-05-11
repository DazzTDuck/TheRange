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
    public float maxFireDistance;
    public float fireDelay;
    public float reloadDelay;
    [Space]
    public float timeToResetAccuracy = 1.5f;
    public int firstAccurateShots = 2;
    public float spreadAmount;
    [Space]
    public float recoilAmountXMin;
    public float recoilAmountXMax;
    [Space]
    public float recoilAmountYMin;
    public float recoilAmountYMax;
    [Space]
    public float recoilAmountZMin;
    public float recoilAmountZMax;
    [Space]
    public AudioClip gunFire;
    public AudioClip gunReload;

}
