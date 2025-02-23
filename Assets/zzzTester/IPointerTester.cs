using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;



public class IPointerTester : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        Debug.Log("pointerEnter");
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        Debug.Log("pointerExit");
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log("Click");
    }

}
