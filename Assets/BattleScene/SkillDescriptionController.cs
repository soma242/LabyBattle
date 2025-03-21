using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using MessagePipe;

public class SkillDescriptionController : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private TMP_Text text;

    private ISubscriber<DescriptionDemendMessage> demandSub;
    private ISubscriber<DescriptionFinishMessage> finishSub;

    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableFinish;

    void Awake()
    {
        demandSub = GlobalMessagePipe.GetSubscriber<DescriptionDemendMessage>();
        finishSub = GlobalMessagePipe.GetSubscriber<DescriptionFinishMessage>();

        var bag = DisposableBag.CreateBuilder();

        demandSub.Subscribe(get =>
        {
            var mousePos = Input.mousePosition;
            //var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            transform.position = mousePos;
            canvas.enabled = true;
            
            text.SetText(get.description);
            disposableFinish = finishSub.Subscribe(get =>
            {
                disposableFinish.Dispose();
                canvas.enabled = false;
            });
        }).AddTo(bag);



        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
        disposableFinish?.Dispose();
    }

}
