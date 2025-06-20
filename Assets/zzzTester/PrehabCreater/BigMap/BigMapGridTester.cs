using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MessagePipe;
using DungeonSceneMessage;




public class BigMapGridTester : MonoBehaviour, IMapGrid
{

    private Canvas canvas;

    private System.IDisposable disposable;

    public void InitiatePosition(int x, int y)
    {
        canvas = GetComponent<Canvas>();
        var image = GetComponent<Image>();

        int width = MapSize.bigMapGridWidth;

        image.rectTransform.anchoredPosition += new Vector2(x * width, y * -1 * width);
    }

    public void SetGridState()
    {
        disposable?.Dispose();
        var resetSub = GlobalMessagePipe.GetSubscriber<MiniMapResetMessage>();
        canvas.enabled = true;
        disposable = resetSub.Subscribe(get =>
        {
            canvas.enabled = false;
            disposable?.Dispose();
        });
    }
}
