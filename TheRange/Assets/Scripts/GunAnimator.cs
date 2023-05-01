using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.speed = 1.0f; //to reset speed
            PlayAnimatorOnFrame(1); //fire animation
        }
    }

    public void StopAnimating()
    {
        animator.speed = 0;
    }

    public void PlayAnimatorOnFrame(int frameNumber)
    {
        animator.Play("Gun", 0, (1 / 264) * frameNumber); //(1/total_frames)*frame_number
    }
}
