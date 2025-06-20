using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

[CreateAssetMenu(menuName = "MessageableSO/Component/useful/layerHolder")]
public class MSO_InputLayerHolder : MessageableScriptableObject
{
    private IPublisher<InputLayerSO, InputLayerChanged> changeInfoPub;
    private ISubscriber<InputLayer> changeSub;

    private System.IDisposable disposableOnDestroy;

    public InputLayerSO inputLayerSO;

    public override void MessageStart()
    {
        changeInfoPub = GlobalMessagePipe.GetPublisher<InputLayerSO, InputLayerChanged>();
        changeSub = GlobalMessagePipe.GetSubscriber<InputLayer>();

        disposableOnDestroy = changeSub.Subscribe(i => {
            inputLayerSO = i.inputLayerSO;
            //Debug.Log("receive: InputLayerChange = " + i.inputLayerSO);
            changeInfoPub.Publish(i.inputLayerSO, new InputLayerChanged());

        });

    }

     ~MSO_InputLayerHolder()
    {
        disposableOnDestroy?.Dispose();
    }
}
