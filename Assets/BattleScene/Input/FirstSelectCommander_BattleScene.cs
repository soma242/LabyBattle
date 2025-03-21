
//SOに移植した。

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning

[System.Serializable]
public class InputLayerFirstSelect
{
    public InputLayerSO inputLayer;
    public int firstNum;
}


//InputLayer毎に用意
public class FirstSelectCommander_BattleScene : MonoBehaviour
{
    private SelectMessage selectMessage;
    private IPublisher<SelectMessage, SelectChange> selectPublisher;


    //InputLayer変更受け取り用MessagePipe
    [SerializeField]
    private InputLayerFirstSelect battleOption;
    //public InputLayerSO inputLayerSO;

    private ISubscriber<InputLayerSO, InputLayerChanged> layerChangeSub;
    private System.IDisposable disposable;

    void Awake()
    {



        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();

        layerChangeSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, InputLayerChanged>();

        var bag = DisposableBag.CreateBuilder();

        layerChangeSub.Subscribe(battleOption.inputLayer, i => {
            selectPublisher.Publish(new SelectMessage(battleOption.inputLayer, battleOption.firstNum), new SelectChange());
        }).AddTo(bag);

        disposable = bag.Build();
    }

    void Destroy()
    {
        disposable?.Dispose();
    }


}
*/