using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractHandler : MonoBehaviour
{
    #region variables

    [SerializeField] private Transform _camera;
    [SerializeField] private float _maxInteractDistance;

    RaycastHit _hit;
    IInteractable _lastInteractable;

    #endregion

    private void Update()
    {
        //interaction logic
        if (Physics.Raycast(_camera.position, _camera.forward, out _hit, _maxInteractDistance))
        {
            if (_hit.collider.TryGetComponent(out IInteractable interactable))
            {
                _lastInteractable = interactable;
                interactable.OnHoverEnter();

                if (Input.GetButtonDown("Interact"))
                {
                    interactable.OnInteract();
                }
            }
            else
            {
                RemoveLastInteractable();
            }
                
        }
        else
        {
            RemoveLastInteractable();
        }
            
    }

    private void RemoveLastInteractable()
    {
        if (_lastInteractable != null)
        {
            _lastInteractable.OnHoverExit();
            _lastInteractable = null;
        }
    }
}
