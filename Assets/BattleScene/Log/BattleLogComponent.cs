using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using MessagePipe;
using BattleSceneMessage;

using Cysharp.Threading.Tasks;


public class BattleLogComponent : MonoBehaviour
{
    private TMP_Text text;
    private Canvas canvas;

    private IPublisher<NewLogSizeMessage> sizePub;
    private ISubscriber<NewLogSizeMessage> sizeSub;

    private IPublisher<DisableLogMessage> disablePub;

    private System.IDisposable disposable;

    private sbyte number;
 


    void OnDestroy()
    {
        disposable?.Dispose();
    }

    public void SetReference()
    {
        text = GetComponent<TMP_Text>();
        canvas = GetComponent<Canvas>();
        sizePub = GlobalMessagePipe.GetPublisher<NewLogSizeMessage>();
        sizeSub = GlobalMessagePipe.GetSubscriber<NewLogSizeMessage>();
        disablePub = GlobalMessagePipe.GetPublisher<DisableLogMessage>();
    }

    public void InstantiateThisComp(string log)
    {
        

        text.SetText(log);

        number = FormationScope.NoneChara();
        float height = 55f;


        if (text.preferredHeight > 55f)
        {

            //Debug.Log(text.rectTransform.rect.size.y);
            while (height < text.preferredHeight)
            {
                height += 55f;
            }

            text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
        text.rectTransform.anchoredPosition = Vector2.zero;
        sizePub.Publish(new NewLogSizeMessage(height));
        UniTask.NextFrame();

        canvas.enabled = true;


        disposable = sizeSub.Subscribe(get =>
        {
            if (number > 39)
            {
                canvas.enabled = false;
                disablePub.Publish(new DisableLogMessage(this, text.rectTransform.sizeDelta.y));
                disposable?.Dispose();
            }

            text.rectTransform.anchoredPosition += get.move;

            number++;

            //Debug.Log(text.rectTransform.position);
        });
    }

    

}
