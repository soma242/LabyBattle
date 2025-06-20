using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

[CreateAssetMenu(menuName = "selectOptionHolder/startScene/start")]
public class StartSelectHolderSO : MessageableScriptableObject
{
    public ISubscriber<InputLayerSO, UpInput> upSub;
    public ISubscriber<InputLayerSO, DownInput> downSub;
    public ISubscriber<InputLayerSO, RightInput> rightSub;
    public ISubscriber<InputLayerSO, LeftInput> leftSub;

    public ISubscriber<InputLayerSO, EnterInput> enterSub;

    public IPublisher<SelectMessage, SelectChange> selectPub;
    public ISubscriber<SelectMessage, SelectChange> selectSub;

    public IPublisher<InputLayer> layerPub;

    [SerializeField]
    public InputLayerSO startLayer;
    [SerializeField]
    public InputLayerSO checkNewGameLayer;

    public override void MessageStart()
    {
        upSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, UpInput>();
        downSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, DownInput>();
        rightSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, RightInput>();
        leftSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, LeftInput>();

        enterSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, EnterInput>();

        selectPub = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSub = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        layerPub = GlobalMessagePipe.GetPublisher<InputLayer>();
    }

}
