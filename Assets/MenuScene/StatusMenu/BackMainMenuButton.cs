using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using MenuScene;


using MessagePipe;
public class BackMainMenuButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        var backMainPub = GlobalMessagePipe.GetPublisher<StatusToMainMessage>();
        backMainPub.Publish(new StatusToMainMessage());

    }
}
