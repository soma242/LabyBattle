using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

using MessagePipe;

namespace MenuScene
{

    public class StatusMemberHolder : MonoBehaviour, IPointerClickHandler
    {
        public Image image;
        public TMP_Text text;

        public bool selecting = false;
        public int id;

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            var clickPub = GlobalMessagePipe.GetPublisher<MemberHolderClickMessage>();
            clickPub.Publish(new MemberHolderClickMessage(id));
        }
    }

}
