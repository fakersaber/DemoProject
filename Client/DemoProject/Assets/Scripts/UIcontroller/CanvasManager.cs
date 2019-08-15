using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{

    public void CanvasPlay()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void CanvasDisplay()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    //public void CanvasShowButBlock()
    //{
    //    GetComponent<CanvasGroup>().alpha = 1;
    //    GetComponent<CanvasGroup>().interactable = false;
    //    GetComponent<CanvasGroup>().blocksRaycasts = false;
    //}
}
