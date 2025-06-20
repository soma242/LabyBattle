using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using DungeonSceneMessage;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

public enum relativePosition
{
    current,
    left,
    right,
    secondCenter,
    secondLeft,
    secondRight,
    thirdCenter,
    thirdLeft,
    thirdRight
}


public class DangeonDrawer : MonoBehaviour
{

    //[SerializeField]
    private DungeonPos pos;
    private int drawPos;

    //+1 =>右，下
    //-1 =>左，上
    private int currentDirection;
    //true=>上下
    //false => 左右
    private bool horizon;

    private int i;

    private CancellationTokenSource cts;


    [SerializeField]
    private MSO_DungeonMapHolderSO mapHolder;
    [SerializeField]
    private MSO_DungeonPositionHolderSO positionHolder;

    private IPublisher<WallDrawMessage> wallPub;

    private IPublisher<RotateDirectionMessage> rotatePub;

    private IPublisher<DungeonPos, ComponentCheckMessage> checkPub;

    private System.IDisposable disposableDestroy;

    [SerializeField]
    private InputLayerSO moveLayer;

    // Input
    private ISubscriber<InputLayerSO, RightInput> rightSub;
    private ISubscriber<InputLayerSO, LeftInput> leftSub;
    private ISubscriber<InputLayerSO, UpInput> upSub;
    private ISubscriber<InputLayerSO, DownInput> downSub;

    void Awake()
    {
        cts = new CancellationTokenSource();

        wallPub = GlobalMessagePipe.GetPublisher<WallDrawMessage>();
        rotatePub = GlobalMessagePipe.GetPublisher<RotateDirectionMessage>();
        checkPub = GlobalMessagePipe.GetPublisher<DungeonPos, ComponentCheckMessage>();

        rightSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, RightInput>();
        leftSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, LeftInput>();
        upSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, UpInput>();
        downSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, DownInput>();


        var bag = DisposableBag.CreateBuilder();

        var drawSub = GlobalMessagePipe.GetSubscriber<DungeonDrawMessage>();
        drawSub.Subscribe(get =>
        {
            BootDrawDungeonView();
        }).AddTo(bag);

        InputDungeonMove(bag);

        var mapSub = GlobalMessagePipe.GetSubscriber<DungeonMapMessage>();
        mapSub.Subscribe(get =>
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();
        }).AddTo(bag);

        disposableDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableDestroy?.Dispose();
        cts?.Cancel();
    }

    private void InputDungeonMove(DisposableBagBuilder bag)
    {
        //方向転換
        rightSub.Subscribe(moveLayer, get =>
        {

            if (positionHolder.horizon)
            {
                positionHolder.ChangeDirection();
                positionHolder.ChangeHorizon();


            }
            else
            {
                positionHolder.ChangeHorizon();

            }
            rotatePub.Publish(new RotateDirectionMessage(true));

            BootDrawDungeonView();
        }).AddTo(bag);

        leftSub.Subscribe(moveLayer, get =>
        {

            if (!positionHolder.horizon)
            {
                positionHolder.ChangeDirection();
                positionHolder.ChangeHorizon();


            }
            else
            {
                positionHolder.ChangeHorizon();

            }
            rotatePub.Publish(new RotateDirectionMessage(false));

            BootDrawDungeonView();
        }).AddTo(bag);

        //移動
        upSub.Subscribe(moveLayer, get =>
        {
            pos = positionHolder.currentPos;
            if (positionHolder.horizon)
            {
                pos.y += positionHolder.currentDirection;
            }
            else
            {
                pos.x += positionHolder.currentDirection;
            }

            if (mapHolder.currentMap.IGetWallBool(pos))
            {
                positionHolder.PositionSet(pos);
                BootDrawDungeonView();
            }

        }).AddTo(bag);
        
        downSub.Subscribe(moveLayer, get =>
        {
            pos = positionHolder.currentPos;
            if (positionHolder.horizon)
            {
                pos.y -= positionHolder.currentDirection;
            }
            else
            {
                pos.x -= positionHolder.currentDirection;
            }

            if (mapHolder.currentMap.IGetWallBool(pos))
            {
                positionHolder.PositionSet(pos);
                BootDrawDungeonView();
            }

        }).AddTo(bag);
    }


    private async UniTask DrawDungeonView(CancellationToken ct)
    {
        int medium = 2;
        bool temp = true;

        
        pos = positionHolder.currentPos;
        currentDirection = positionHolder.currentDirection;
        horizon = positionHolder.horizon;

        
        //相対位置
        // 3456
        //11 012
        // 789 10
        drawPos = 0;

        checkPub.Publish(pos, new ComponentCheckMessage(drawPos));

        //上下
        if (horizon)
        {
            for (i = 0; i < 3; i++)
            {
                //Debug.Log(i);

                pos.y += currentDirection;
                //Debug.Log(pos.x + "" + pos.y);

                checkPub.Publish(pos, new ComponentCheckMessage(drawPos));
                temp = mapHolder.currentMap.IGetWallBool(pos);
                //Debug.Log(temp);
                wallPub.Publish(new WallDrawMessage(temp, drawPos));
                if (!temp)
                {
                    break;
                }

                drawPos++;
                medium++;
            }
            if (medium == 5)
            {
                medium = 4;
            }
            
            //左側
            drawPos = 3;
            pos = positionHolder.currentPos;
            pos.x += currentDirection;

            for(i = 0; i < medium; i++)
            {
                checkPub.Publish(pos, new ComponentCheckMessage(drawPos));
                temp = mapHolder.currentMap.IGetWallBool(pos);
                wallPub.Publish(new WallDrawMessage(temp, drawPos));

                drawPos++;
                pos.y += currentDirection;

            }

            //右側
            drawPos = 7;
            pos = positionHolder.currentPos;
            pos.x -= currentDirection;

            for (i = 0; i < medium; i++)
            {
                checkPub.Publish(pos, new ComponentCheckMessage(drawPos));

                //Debug.Log("left");
                temp = mapHolder.currentMap.IGetWallBool(pos);
                wallPub.Publish(new WallDrawMessage(temp, drawPos));

                drawPos++;
                pos.y += currentDirection;

            }

        }

        else
        {
            for (i = 0; i < 3; i++)
            {
                //Debug.Log(i);
                pos.x += currentDirection;
                checkPub.Publish(pos, new ComponentCheckMessage(drawPos));

                temp = mapHolder.currentMap.IGetWallBool(pos);
                //Debug.Log(temp);
                //Debug.Log(drawPos);
                wallPub.Publish(new WallDrawMessage(temp, drawPos));
                if (!temp)
                {
                    break;
                }

                drawPos++;
                medium++;

            }
            if (medium == 5)
            {
                medium = 4;
            }

            drawPos = 3;
            pos = positionHolder.currentPos;
            pos.y -= currentDirection;

            for (i = 0; i < medium; i++)
            {
                //Debug.Log("right");
                checkPub.Publish(pos, new ComponentCheckMessage(drawPos));

                temp = mapHolder.currentMap.IGetWallBool(pos);
                wallPub.Publish(new WallDrawMessage(temp, drawPos));



                drawPos++;
                pos.x += currentDirection;

            }

            drawPos = 7;
            pos = positionHolder.currentPos;
            pos.y += currentDirection;

            for (i = 0; i < medium; i++)
            {
                //Debug.Log("left");
                checkPub.Publish(pos, new ComponentCheckMessage(drawPos));

                temp = mapHolder.currentMap.IGetWallBool(pos);
                wallPub.Publish(new WallDrawMessage(temp, drawPos));

                drawPos++;
                pos.x += currentDirection;

            }
        }


    }

    private void BootDrawDungeonView()
    {
        //Debug.Log("draw");

        try
        {

            DrawDungeonView(cts.Token);

        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
        {
#if UNITY_EDITOR
                Debug.Log("cancel perfome");
                        if (cts.IsCancellationRequested)
                        {
                            // 引数のCancellationTokenが原因なので、それを保持したOperationCanceledExceptionとして投げる
                            throw new OperationCanceledException(ex.Message, ex, cts.Token);
                        }
                        else
                        {
                            // タイムアウトが原因なので、TimeoutException(或いは独自の例外)として投げる
                            throw new TimeoutException("The request was canceled due to the configured Timeout ");
                        }
#endif
        }
    }



}
