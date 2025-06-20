using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MessagePipe;
using DungeonSceneMessage;


public interface IMapScreen
{
    void InitiatePosition(int x, int y);
    void SetScreenState();
    void ResetScreenState();
}

public class BigMapScreen : MonoBehaviour, IMapScreen
{

    private Canvas canvas;

    private System.IDisposable disposable;

    void OnDestroy()
    {
        disposable?.Dispose();
    }

    public void InitiatePosition(int x, int y)
    {
        canvas = GetComponent<Canvas>();
        var image = GetComponent<Image>();

        int width = MapSize.bigMapGridWidth;

        image.rectTransform.anchoredPosition += new Vector2(x * width, y * -1 * width);
    }

    public void SetScreenState()
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

    public void ResetScreenState()
    {
        canvas.enabled = false;
        disposable?.Dispose();
    }

}
