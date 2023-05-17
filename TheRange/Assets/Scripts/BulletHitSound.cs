using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BulletHitSound : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [Space]
    [SerializeField] private AudioClip _defualtHit;
    [SerializeField] private AudioClip _metalHit;
    
    private void Awake()
    {
        var hits = Physics.SphereCastAll(transform.position, 0.01f, transform.forward);

        foreach (var hit in hits)
        {
            if(hit.collider.TryGetComponent(out SoundType soundType))
            {
                switch (soundType.sound)
                {
                    case SoundMaterial.Metal:
                        _source.PlayOneShot(_metalHit);
                        break;
                    case SoundMaterial.Person:
                        //_source.PlayOneShot(_metalHit);
                        break;
                    case SoundMaterial.Dirt:
                        //_source.PlayOneShot(_metalHit);
                        break;
                }
                return;
            }
        }

        //if not found, play the default sound
        _source.PlayOneShot(_defualtHit);
    }
}
