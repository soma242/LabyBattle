using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using DungeonSceneMessage;





public class MapDirectionController : MonoBehaviour
{
    [SerializeField]
    private MSO_DungeonPositionHolderSO posHolder;

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        var bag = DisposableBag.CreateBuilder();

        var rotateSub = GlobalMessagePipe.GetSubscriber<RotateDirectionMessage>();

        var drawSub = GlobalMessagePipe.GetSubscriber<DungeonDrawMessage>();

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
    }
}
