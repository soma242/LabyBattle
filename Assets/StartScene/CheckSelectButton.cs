using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using MessagePipe;
using StartSceneMessage;

public class CheckSelectButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private bool yes;


    public Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        CheckPerfome();
    }

    public void CheckPerfome()
    {
        if (yes)
        {
            var yesPub = GlobalMessagePipe.GetPublisher<YesCheckMessage>();
            yesPub.Publish(new YesCheckMessage());
        }
        else
        {
            var noPub = GlobalMessagePipe.GetPublisher<NoCheckMessage>();
            noPub.Publish(new NoCheckMessage());
        }
    }

}
