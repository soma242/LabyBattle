using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using MenuScene;


[CreateAssetMenu(menuName = "selectOptionHolder/menuScene/menu")]
public class MenuSelectHolderSO : MessageableScriptableObject
{

    public ISubscriber<InputLayerSO, UpInput> upSub; 
    public ISubscriber<InputLayerSO, DownInput> downSub;
    public ISubscriber<InputLayerSO, RightInput> rightSub;
    public ISubscriber<InputLayerSO, LeftInput> leftSub;

    public ISubscriber<InputLayerSO, EnterInput> enterSub;

    public ISubscriber<InputLayerSO, MenuInput> menuSub;

    //select
    public IPublisher<SelectMessage, SelectChange> selectPub;
    public ISubscriber<SelectMessage, SelectChange> selectSub;

    //disposeSelect
    public IPublisher<InputLayerSO, DisposeSelect> selectDispPub;
    public ISubscriber<InputLayerSO, DisposeSelect> selectDispSub;


    public IPublisher<InputLayer> layerPub;

    //MainMenu‚ÌGraphicRaycaster = IPointer‚ÌOnOff
    public IPublisher<MenuIPointerMessage> pointerPub;
    public ISubscriber<MenuIPointerMessage> pointerSub;

    //SkillField
    public IPublisher<SelectNextNameHolder> nextSkillPub;
    public IPublisher<SelectPreNameHolder> preSkillPub;

    public IPublisher<SelectNextTreeHolder> nextTreePub;
    public IPublisher<SelectPreTreeHolder> preTreePub;

    public IPublisher<SkillToStatusMessage> statusPub;

    public IPublisher<SkillDescriptionMessage> descPub;

    public ISelectSkillNameHolder firstSelector;
    public ISelectSkillNameHolder commonSelector;
    public ISelectSkillNameHolder lastSelector;
    public ISelectSkillNameHolder onlySelector;

    //


    //InputSupporterMenu‚©‚çDispose
    public System.IDisposable disposableReturn;


    [SerializeField]
    public InputLayerSO mainLayer;

    [SerializeField]
    public InputLayerSO detailLayer;
    
    [SerializeField]
    public InputLayerSO statusLayer;

    //[SerializeField]
    public InputLayerSO skillLayer;


    public SelectSourceImageSO sourceImageSO;

    public override void MessageStart()
    {
        upSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, UpInput>();
        downSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, DownInput>();
        rightSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, RightInput>();
        leftSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, LeftInput>();

        enterSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, EnterInput>();

        menuSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, MenuInput>();

        selectPub = GlobalMessagePipe.GetPublisher<SelectMessage, SelectChange>();
        selectSub = GlobalMessagePipe.GetSubscriber<SelectMessage, SelectChange>();

        selectDispPub = GlobalMessagePipe.GetPublisher<InputLayerSO, DisposeSelect>();
        selectDispSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, DisposeSelect>();


        layerPub = GlobalMessagePipe.GetPublisher<InputLayer>();

        pointerPub = GlobalMessagePipe.GetPublisher<MenuIPointerMessage>();
        pointerSub = GlobalMessagePipe.GetSubscriber<MenuIPointerMessage>();


        //skillManu
        nextSkillPub = GlobalMessagePipe.GetPublisher<SelectNextNameHolder>();
        preSkillPub = GlobalMessagePipe.GetPublisher<SelectPreNameHolder>();

        nextTreePub = GlobalMessagePipe.GetPublisher<SelectNextTreeHolder>();
        preTreePub = GlobalMessagePipe.GetPublisher<SelectPreTreeHolder>();

        statusPub = GlobalMessagePipe.GetPublisher<SkillToStatusMessage>();

        descPub = GlobalMessagePipe.GetPublisher<SkillDescriptionMessage>();

        //resetASub = GlobalMessagePipe.GetAsyncSubscriber<ResetSkillFieldMessage>();


        firstSelector = new FirstSelectSkillNameHolder(this);
        commonSelector = new CommonSelectSkillNameHolder(this);
        lastSelector = new LastSelectSkillNameHolder(this);
        onlySelector = new OnlySelectSkillNameHolder(this);
    }
}
