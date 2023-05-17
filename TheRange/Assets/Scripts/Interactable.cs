using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] public bool HasBeenInteracted { get; private set; }
    [Space]
    [SerializeField] private float _timeToResetInteractable = 2f;
    [SerializeField] private UnityEvent OnHoverEnterEvent;
    [SerializeField] private UnityEvent OnHoverExitEvent;
    [SerializeField] private UnityEvent OnInteractEvent;

    public void OnHoverEnter()
    {
        if(!HasBeenInteracted)
            OnHoverEnterEvent?.Invoke();
    }

    public void OnHoverExit()
    {
        if (!HasBeenInteracted)
            OnHoverExitEvent?.Invoke();
    }

    public void OnInteract()
    {
        if (!HasBeenInteracted)
        {
            OnInteractEvent?.Invoke();
            OnHoverExit();


            HasBeenInteracted = true;
            //start timer, end of timer reset the interactable
            var resetTimer = gameObject.AddComponent<Timer>();
            resetTimer.StartTimer(_timeToResetInteractable, () => { HasBeenInteracted = false; Destroy(resetTimer); });
        }   
    }
}
