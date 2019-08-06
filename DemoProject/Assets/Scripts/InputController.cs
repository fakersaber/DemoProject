using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : ScrollRect
{
    private float Radius = 50f;
    public Vector2 direct;


    // Start is called before the first frame update
    //void Start()
    //{
    //    Radius = (transform as RectTransform).sizeDelta.x * 0.5f;
    //}
    // Update is called once per frame

    void Update()
    {
        //direct = content.localPosition / Radius;
        //Debug.Log(content.localPosition);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        Vector2 currentPosition = content.anchoredPosition;
        if (currentPosition.magnitude > Radius)
        {
            currentPosition = currentPosition.normalized * Radius;
            SetContentAnchoredPosition(currentPosition);
        }

        direct = content.localPosition / Radius;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        direct = new Vector2(0f,0f);
    }

}
