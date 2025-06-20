using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MessagePipe;
using DungeonSceneMessage;


public class MiniMapDownStairs : MonoBehaviour
{

    private System.IDisposable disposableOnDestroy;


    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }

    public void SetSubscriber()
    {
        disposableOnDestroy?.Dispose();

        var bag = DisposableBag.CreateBuilder();

        var resetSub = GlobalMessagePipe.GetSubscriber<MiniMapResetMessage>();
        resetSub.Subscribe(get =>
        {
            disposableOnDestroy?.Dispose();
            var canvas = GetComponent<Canvas>();
            canvas.enabled = false;
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
    }

    public void SetPosition(DungeonPos pos)
    {
        var image = GetComponent<Image>();
        //Debug.Log(pos.x + ":x " + pos.y + ":y");
        image.rectTransform.anchoredPosition = new Vector2(pos.x * 40, pos.y * -40);
    }

    public void SetStairs(DungeonPos pos)
    {
        var canvas = GetComponent<Canvas>();
        canvas.enabled = true;

        SetPosition(pos);
        SetSubscriber();

    }
}
