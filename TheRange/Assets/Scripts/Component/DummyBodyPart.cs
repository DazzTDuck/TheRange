using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBodyPart : MonoBehaviour, IHittable
{
    #region variables

    [SerializeField] private float _damageMultiplier;
    private Dummy _dummy;

    #endregion

    /// <summary>
    /// references the dummy object the part is attached to
    /// </summary>
    /// <param name="dummy">head dummy object reference</param>
    public void SetDummy(Dummy dummy)
    {
        _dummy = dummy;
    }

    public void OnHit(float damage)
    {
        //send damage to dummy to display it
       _dummy.DoDamage(Mathf.RoundToInt(damage * _damageMultiplier));
    }
}
