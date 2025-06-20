using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;


using BattleSceneMessage;

[CreateAssetMenu(menuName = "selectOptionHolder/battleScene/baseSelectMessaggeHolder")]
public class BaseSelectMessageHolder : MessageableScriptableObject
{
    public ISubscriber<InputLayerSO, UpInput> upSub; //{ get; private set; }
    public ISubscriber<InputLayerSO, DownInput> downSub;
    public ISubscriber<InputLayerSO, RightInput> rightSub;
    public ISubscriber<InputLayerSO, LeftInput> leftSub;

    public ISubscriber<InputLayerSO, EnterInput> enterSub;

    public IPublisher<InputLayerSO, DisposeSelect> selectDispPub;
    public ISubscriber<InputLayerSO, DisposeSelect> selectDispSub;

    [SerializeField]
    public InputLayerSO inputLayerSO;


    public override void MessageStart()
    {
        upSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, UpInput>();
        downSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, DownInput>();
        rightSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, RightInput>();
        leftSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, LeftInput>();

        enterSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, EnterInput>();

        selectDispPub = GlobalMessagePipe.GetPublisher<InputLayerSO, DisposeSelect>();
        selectDispSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, DisposeSelect>();
    }

}
