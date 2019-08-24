using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    public void Awake()
    {
        animator = GetComponent<Animator>();
    }



    public void SetIce()
    {
        animator.SetBool("iced",true);
    }

    public void SetUnIce()
    {
        animator.SetBool("iced", false);
    }
}
