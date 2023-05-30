using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable 
{
    /// <summary>
    ///will be called when the player hits an IHittable object
    /// </summary>
    /// <param name="damage">amount of damage the player has done</param>
    public void OnHit(float damage = 0f);
}
