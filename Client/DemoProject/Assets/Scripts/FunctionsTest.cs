using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionsTest : MonoBehaviour
{
    private Transform[] transforms;
    private void Start()
    {
        transforms = this.GetComponentsInChildren<Transform>();
        

    }
    public void Blink()
    {
        for(int i = 0;i< transforms.Length;i++)
        {
            transforms[i].gameObject.layer = 10;
        }

    }
    public void Return()
    {
        for (int i = 0; i < transforms.Length; i++)
        {
            transforms[i].gameObject.layer = 9;
        }
    }
    
    
}
