using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Will be called when player is hovering over an IInteractable object and pressing the interact button
    /// </summary>
    public void OnInteract();

    /// <summary>
    /// Will be called when player is hovering over IInteractable object
    /// </summary>
    public void OnHoverEnter();

    /// <summary>
    /// Will be called when player stops hovering from previous IInteractable object
    /// </summary>
    public void OnHoverExit();
}
