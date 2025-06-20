using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using DungeonSceneMessage;

using Cysharp.Threading.Tasks;


[CreateAssetMenu(menuName = "dungeonScene/positionHolder")]
public class MSO_DungeonPositionHolderSO : MessageableScriptableObject
{
    public int currentKey;

    public DungeonPos currentPos;
    //{ get; private set; }

    //+1 =>�E�C��
    //-1 =>���C��
    public int currentDirection;
    //true=>�㉺
    //false => ���E
    public bool horizon;

    private ISubscriber<DungeonMapMessage> mapSub;

    //private IPublisher<DungeonDrawMessage> drawPub;
    private IPublisher<MiniMapResetMessage> resetPub;
    private IPublisher<MiniMapUpdateMessage> updatePub;

    private IPublisher<PosChangeMessage> posPub;
    //private IPublisher<RotateDirectionMessage> direPub;

    private IPublisher<DungeonPos, ComponentCheckMessage> checkPub;

    private System.IDisposable disposableDestroy;

    public override void MessageStart()
    {
        mapSub = GlobalMessagePipe.GetSubscriber<DungeonMapMessage>();
        resetPub = GlobalMessagePipe.GetPublisher<MiniMapResetMessage>();
        updatePub = GlobalMessagePipe.GetPublisher<MiniMapUpdateMessage>();

        posPub = GlobalMessagePipe.GetPublisher<PosChangeMessage>();
        //direPub = GlobalMessagePipe.GetPublisher<RotateDirectionMessage>();

        checkPub = GlobalMessagePipe.GetPublisher<DungeonPos, ComponentCheckMessage>();

        var bag = DisposableBag.CreateBuilder();

        //bool test = System.Convert.ToBoolean(0);
        //Debug.Log(test);

        mapSub.Subscribe(async get =>
        {
            //Debug.Log(get.mapData.name);
            currentKey = get.mapData.mapKey;
            currentPos = get.setPos;
            currentDirection = get.setDirection;
            horizon = get.setHorizon;
            //get.mapData.ISetComponent();

            //�`��̊J�n
            resetPub.Publish(new MiniMapResetMessage());
            await UniTask.NextFrame();

            //��������MiniMapController=>DungeonMapData�Ń}�b�v�쐻
            updatePub.Publish(new MiniMapUpdateMessage());
            //posPub.Publish(new PosChangeMessage());

        }).AddTo(bag);

        disposableDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableDestroy?.Dispose();
    }

    public void ChangeDirection()
    {
        currentDirection *= -1;
    }

    public void ChangeHorizon()
    {
        horizon = !horizon;
        //direPub.Publish(new RotateDirectionMessage());
    }

    public void PositionSet(DungeonPos pos)
    {
        //Debug.Log(pos.x + ":x , y:" + pos.y);
        currentPos = pos;
        posPub.Publish(new PosChangeMessage());
        checkPub.Publish(currentPos, new ComponentCheckMessage(11));
    }

}

