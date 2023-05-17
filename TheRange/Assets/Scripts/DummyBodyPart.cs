using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBodyPart : MonoBehaviour, IHittable
{
    [SerializeField] private float _damageMultiplier;

    private Dummy _dummy;

    public void SetDummy(Dummy dummy)
    {
        _dummy = dummy;
    }

    public void OnHit(float damage)
    {
       _dummy.DoDamage(Mathf.RoundToInt(damage * _damageMultiplier));
    }
}
