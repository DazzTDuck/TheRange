using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable 
{
    public void OnHit(float damage = 0f);
}
