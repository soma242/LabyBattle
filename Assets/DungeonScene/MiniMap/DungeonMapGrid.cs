using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MessagePipe;
using DungeonSceneMessage;


public interface IMapGrid
{
    void InitiatePosition(int x, int y);
    void SetGridState();

}

public class DungeonMapGrid : MonoBehaviour, IMapGrid
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

        image.rectTransform.anchoredPosition += new Vector2(x*40, y*-40);
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
