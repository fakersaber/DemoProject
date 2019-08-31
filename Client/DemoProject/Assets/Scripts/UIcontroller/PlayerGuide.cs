using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGuide : MonoBehaviour
{

    private Tween tween;

    private int tag;
    private float moveX;
    private Vector3 myVec;

    private void Awake()
    {
        DOTween.Init();
        tag = 0;
        moveX = 0f;
        myVec = new Vector3(moveX, 0, 0);
    }

    public void ToLeft()
    {
        tag -= 1;
        if (tag < 0)
        {
            tag = 0;
            return;
        }
        else
        {
            moveX += 2160f;
            myVec.x = moveX;
            tween = transform.DOLocalMove(myVec, 0.2f).SetEase(Ease.Linear);
        }
    }

    public void ToRight()
    {
        tag += 1;
        if (tag > 3)
        {
            tag = 3;
            return;
        }
        else
        {
            moveX -= 2160f;
            myVec.x = moveX;
            tween = transform.DOLocalMove(myVec, 0.2f).SetEase(Ease.Linear);
        }

    }
}
