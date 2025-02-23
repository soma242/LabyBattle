
using UnityEngine;
using UnityEngine.EventSystems;


public class BaseButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        ButtonPerformance();
    }


    public virtual void ButtonPerformance()
    {

    }
}
