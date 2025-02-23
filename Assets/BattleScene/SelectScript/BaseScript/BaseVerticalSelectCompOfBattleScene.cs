using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

#pragma warning disable CS4014 // disable warning


using BattleSceneMessage;
//using MessageableManager;



//namespace:BattleSceneMessageを用いているので，BattleScene専用として，他のシーンではusing部とクラス名を変更してコピペで使いまわす。
public class BaseVerticalSelectCompOfBattleScene : MonoBehaviour
{

    //インスタンスごとに変えることで振る舞いを変える識別用の値
    //最初にInputLayerの変更を行わないと何故かこのスクリプトからバグ出る。要デバッグ
    [SerializeField] protected InputLayerSO inputLayerSO;

    //自分の識別番号と自分からつながる識別番号
    protected Holder_VerticalSelectNum holder;
    protected int myNum;
    //protected int upNum;
    //protected int downNum;

    //アタッチされたオブジェクトのイメージ
    protected Image image;

    //Subscribe用に自分のLayerNumとmuNumを結合する（変化しないのでAwake）
    //public SelectMessage(InputLayer _layerNum, int _selectNum)
    protected SelectMessage mySelectMessage;

    //select変更用MessagePipe
    protected IPublisher<SelectMessage, SelectChange> selectPublisher;
    protected ISubscriber<SelectMessage, SelectChange> selectSubscriber;

    //selectのDispose用MessagePipe
    //[Inject] private readonly ISubscriber<DisposeSelect> disposeSelectSubscriber;

    protected System.IDisposable disposable;

    //Input受け入れ用MessagePipe
    [Inject] protected readonly ISubscriber<InputLayerSO, UpInput> upSubscriber;
    [Inject] protected readonly ISubscriber<InputLayerSO, DownInput> downSubscriber;

    protected System.IDisposable disposableInput;



    //継承後イベント処理に追加する

    protected void _Awake()
    {
        //inputLayerと（自分を対象にした）selectMessageを作成
        //これらは一度しか行われない
        holder = GetComponent<Holder_VerticalSelectNum>();

        myNum = holder.myNum;
        /*
        upNum = holder.upNum;
        downNum = holder.downNum;
        */

        image = GetComponent<Image>();

        mySelectMessage = new SelectMessage(inputLayerSO, myNum);

        //BuiltInContainer
        selectPublisher = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSubscriber = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        disposable = selectSubscriber.Subscribe(mySelectMessage, i =>
        {
            SelectThisComponent();
        });


        //Debug.Log(holder.myNum+ "to " + holder.upNum + holder.downNum);

    }
    /*
    protected void _OnEnable()
    {
        //selectされるときのSubscriberの登録

        //var bag = DisposableBag.CreateBuilder();

        
            //.AddTo(bag);

        //disposable = bag.Build();
    }
    */


    //他の位置でdisposeしているので，Destroyのタイミングでdispose対象が存在しない可能性がある。(一度もSubscriber登録を行っていないときなど)
    /*
    protected void _OnDestroy()
    {
        UnselectThisComponent();
    }
    */


    //有効にすると複数回の実行が行われる。
    /*
   protected void _OnDisable()
   {
       //
       Debug.Log("dispose");
       disposable.Dispose();
   }
   */

    //継承後で実際に行うSelect処理のひな形
    public virtual void SelectThisComponent() { }
    public virtual void UnselectThisComponent() { }

    //継承後に上記のSelectに追加する処理内容
    protected async UniTask _SelectThisComponent()
    {

        image.sprite = holder.imageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();

        //現在走っているPublishを受け入れないために1frame待つ
        await UniTask.NextFrame();


        upSubscriber.Subscribe(inputLayerSO, i => {
            NextUpSelect();
        }).AddTo(bag);

        downSubscriber.Subscribe(inputLayerSO, i => {
            NextDownSelect();
        }).AddTo(bag);

        /*
        disposeSelectSubscriber.Subscribe( i => {
            UnselectThisComponent();
        }).AddTo(bag);
        */

        disposableInput = bag.Build();


        //Debug.Log(myNum);

    }

    protected void _UnselectThisComponent()
    {
        //Debug.Log("disposeInput");
        disposableInput.Dispose();
        //Debug.Log("inputDispo is dispose");
        image.sprite = holder.imageSO.offSelect;
    }


    //次のSelectを行う
    protected void NextUpSelect()
    {

        //Debug.Log("up");
        UnselectThisComponent();
        //Debug.Log(holder.upNum);

        selectPublisher.Publish(new SelectMessage(inputLayerSO, holder.upNum), new SelectChange());

    }

    protected void NextDownSelect()
    {
        //Debug.Log("down");
        UnselectThisComponent();
        //Debug.Log(holder.downNum);


        selectPublisher.Publish(new SelectMessage(inputLayerSO, holder.downNum), new SelectChange());

    }

    protected void OnDestroy()
    {
        disposable.Dispose();
    }



}
