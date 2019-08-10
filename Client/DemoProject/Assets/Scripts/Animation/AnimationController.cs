using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniController : MonoBehaviour
{
    private Animator anim;

    public void Awake()
    {
        anim = this.GetComponent<Animator>();
        
    }


    private void Start()
    {
        //var test = anim.GetParameter(1);
        SetAttacked();
    }

    public void SetDie()
    {
        anim.SetBool("isdie", true);
    }

    public void SetAttacked()
    {
        //anim.GetParameter(0);
        anim.SetBool("isattacked", true);
    }
}
