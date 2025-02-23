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



//namespace:BattleSceneMessage��p���Ă���̂ŁCBattleScene��p�Ƃ��āC���̃V�[���ł�using���ƃN���X����ύX���ăR�s�y�Ŏg���܂킷�B
public class BaseVerticalSelectCompOfBattleScene : MonoBehaviour
{

    //�C���X�^���X���Ƃɕς��邱�ƂŐU�镑����ς��鎯�ʗp�̒l
    //�ŏ���InputLayer�̕ύX���s��Ȃ��Ɖ��̂����̃X�N���v�g����o�O�o��B�v�f�o�b�O
    [SerializeField] protected InputLayerSO inputLayerSO;

    //�����̎��ʔԍ��Ǝ�������Ȃ��鎯�ʔԍ�
    protected Holder_VerticalSelectNum holder;
    protected int myNum;
    //protected int upNum;
    //protected int downNum;

    //�A�^�b�`���ꂽ�I�u�W�F�N�g�̃C���[�W
    protected Image image;

    //Subscribe�p�Ɏ�����LayerNum��muNum����������i�ω����Ȃ��̂�Awake�j
    //public SelectMessage(InputLayer _layerNum, int _selectNum)
    protected SelectMessage mySelectMessage;

    //select�ύX�pMessagePipe
    protected IPublisher<SelectMessage, SelectChange> selectPublisher;
    protected ISubscriber<SelectMessage, SelectChange> selectSubscriber;

    //select��Dispose�pMessagePipe
    //[Inject] private readonly ISubscriber<DisposeSelect> disposeSelectSubscriber;

    protected System.IDisposable disposable;

    //Input�󂯓���pMessagePipe
    [Inject] protected readonly ISubscriber<InputLayerSO, UpInput> upSubscriber;
    [Inject] protected readonly ISubscriber<InputLayerSO, DownInput> downSubscriber;

    protected System.IDisposable disposableInput;



    //�p����C�x���g�����ɒǉ�����

    protected void _Awake()
    {
        //inputLayer�Ɓi������Ώۂɂ����jselectMessage���쐬
        //�����͈�x�����s���Ȃ�
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
        //select�����Ƃ���Subscriber�̓o�^

        //var bag = DisposableBag.CreateBuilder();

        
            //.AddTo(bag);

        //disposable = bag.Build();
    }
    */


    //���̈ʒu��dispose���Ă���̂ŁCDestroy�̃^�C�~���O��dispose�Ώۂ����݂��Ȃ��\��������B(��x��Subscriber�o�^���s���Ă��Ȃ��Ƃ��Ȃ�)
    /*
    protected void _OnDestroy()
    {
        UnselectThisComponent();
    }
    */


    //�L���ɂ���ƕ�����̎��s���s����B
    /*
   protected void _OnDisable()
   {
       //
       Debug.Log("dispose");
       disposable.Dispose();
   }
   */

    //�p����Ŏ��ۂɍs��Select�����̂ЂȌ`
    public virtual void SelectThisComponent() { }
    public virtual void UnselectThisComponent() { }

    //�p����ɏ�L��Select�ɒǉ����鏈�����e
    protected async UniTask _SelectThisComponent()
    {

        image.sprite = holder.imageSO.onSelect;

        var bag = DisposableBag.CreateBuilder();

        //���ݑ����Ă���Publish���󂯓���Ȃ����߂�1frame�҂�
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


    //����Select���s��
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
