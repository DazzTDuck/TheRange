using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletHitSound : MonoBehaviour
{
    #region variables

    [SerializeField] private AudioSource _source;
    [Space]
    [SerializeField] private AudioHitSound _hitDefault;
    [SerializeField] private AudioHitSound _hitMetal;
    [SerializeField] private AudioHitSound _hitFlesh;

    #endregion

    private void Awake()
    {
        //looks for object around object
        var hits = Physics.SphereCastAll(transform.position, 0.01f, transform.forward);

        foreach (var hit in hits)
        {
            //tries to find sound type script to identify which sounds to use
            if(hit.collider.TryGetComponent(out SoundType soundType))
            {
                switch (soundType.sound)
                {
                    case SoundMaterial.Metal:
                        _source.PlayOneShot(_hitMetal.hitSound, _hitMetal.volume);
                        break;
                    case SoundMaterial.Person:
                        _source.PlayOneShot(_hitFlesh.hitSound, _hitFlesh.volume);
                        break;
                }
                return;
            }
        }

        //if not found, play the default sound
        _source.PlayOneShot(_hitDefault.hitSound, _hitDefault.volume);
    }
}

[Serializable]
public struct AudioHitSound
{
    public AudioClip hitSound;
    public float volume;
}
