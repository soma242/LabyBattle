using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MessagePipe;
using DungeonSceneMessage;

public class BigMapDownStairs : MonoBehaviour
{
    public System.IDisposable disposableOnDestroy;

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }

    public void SetDownStairs(DungeonPos pos)
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

        int width = MapSize.bigMapGridWidth;
        var image = GetComponent<Image>();
        image.rectTransform.anchoredPosition = new Vector2(pos.x * width, pos.y * -1 * width);

        var canvas = GetComponent<Canvas>();
        canvas.enabled = true;


    }
}
