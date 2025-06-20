using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MessagePipe;

using DungeonSceneMessage;

public class BigMapDirectionController : MonoBehaviour
{
    private int size;
    private int half;
    private Image image;

    [SerializeField]
    private MSO_DungeonPositionHolderSO posHolder;

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        size = MapSize.bigMapGridWidth;
        half = size / 2;
        image = GetComponent<Image>();

        var bag = DisposableBag.CreateBuilder();

        var rotateSub = GlobalMessagePipe.GetSubscriber<RotateDirectionMessage>();
        var drawSub = GlobalMessagePipe.GetSubscriber<DungeonDrawMessage>();
        var moveSub = GlobalMessagePipe.GetSubscriber<PosChangeMessage>();

        drawSub.Subscribe(get =>
        {
            InitializeRotate();
        }).AddTo(bag);

        rotateSub.Subscribe(get =>
        {
            if (get.right)
            {
                transform.Rotate(0, 0, -90f);
            }
            else
            {
                transform.Rotate(0, 0, 90f);

            }
        }).AddTo(bag);

        moveSub.Subscribe(get =>
        {
            image.rectTransform.anchoredPosition = new Vector2(half + posHolder.currentPos.x * size, -1 * (half + posHolder.currentPos.y * size));
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }

    private void InitializeRotate()
    {
        transform.rotation = default;

        if (!posHolder.horizon)
        {
            transform.Rotate(0, 0, -90f);
            if (posHolder.currentDirection < 0)
            {
                transform.Rotate(0, 0, 180f);
            }
        }
        else
        {
            if (posHolder.currentDirection > -1)
            {
                transform.Rotate(0, 0, 180f);
            }
        }

        image.rectTransform.anchoredPosition = new Vector2(half + posHolder.currentPos.x * size, -1 *( half + posHolder.currentPos.y * size));

    }
}
