using UnityEngine;

using MessagePipe;



using BattleSceneMessage;


//#####################一つしか作っちゃダメ。別のシーンで用いるならここからコピーしてusing BattleSceneMessage;をそのシーンのnamespace変更

public class CurrentInputLayerOfBattleScene : MonoBehaviour
{
    private  IPublisher<InputLayer, InputLayerChanged> inputLayerChangedPublisher;
    private  ISubscriber<InputLayer> inputLayerChangeSubscriber;

    private System.IDisposable disposable;

    public InputLayerSO inputLayerSO;


    void Awake()
    {
        var bag = DisposableBag.CreateBuilder();

        Debug.Log("CurrentInputLayerSO");

        //受け取ったInputLayerのfirstSelectを選択するためのPublish。
        inputLayerChangedPublisher = GlobalMessagePipe.GetPublisher<InputLayer, InputLayerChanged>();


        inputLayerChangeSubscriber = GlobalMessagePipe.GetSubscriber<InputLayer>();

        //inputLayerの変更を受け取った時，受け取ったInputLayerのfirstSelectを選択するためにPublishを行う。
        inputLayerChangeSubscriber.Subscribe(i => {
            ChangeValue(i);
        }).AddTo(bag);

        disposable = bag.Build();
    }

    private void ChangeValue(InputLayer i)
    {
        //
        Debug.Log("receive: InputLayerChange = "+i.inputLayerSO);

        //現在のInputLayerを変更。
        inputLayerSO = i.inputLayerSO;

        inputLayerChangedPublisher.Publish(new InputLayer(inputLayerSO), new InputLayerChanged());
    }

    public void OnDestroy()
    {

        disposable.Dispose();
    }
}
