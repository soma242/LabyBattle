using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.InputSystem;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using DungeonSceneMessage;



//InputSystemからMessagePipeを起動する。
//イベントを追加後インスペクターからPlayerInputに追加を行う。

public class CommonInputSystem : MonoBehaviour
{

    private IPublisher<InputLayerSO, RightInput> rightPub;
    private IPublisher<InputLayerSO, LeftInput> leftPub;
    private IPublisher<InputLayerSO, UpInput> upPub;
    private IPublisher<InputLayerSO, DownInput> downPub;
    private IPublisher<InputLayerSO, EnterInput> enterPub;
    private IPublisher<InputLayerSO, CancelInput> cancelPub;

    private IPublisher<InputLayerSO, MapInput> mapPub;
    private IPublisher<InputLayerSO, MenuInput> menuPub;

    private IPublisher<InputLayerSO, ScrollInput> scrollPub;



    private IPublisher<Holdout> holdoutPub;

    

    

    //[Inject] private readonly ISubscriber<DownHolding> downHoldSub;


    //[Inject] private readonly IPublisher<RightInput> RightInputPublisher;

    [SerializeField]
    private MSO_InputLayerHolder currentInputLayer;

    void Awake()
    {
        //currentInputLayer = GetComponent<InputLayerHolder>();


        rightPub = GlobalMessagePipe.GetPublisher<InputLayerSO, RightInput>();
        leftPub = GlobalMessagePipe.GetPublisher<InputLayerSO, LeftInput>();
        upPub = GlobalMessagePipe.GetPublisher<InputLayerSO, UpInput>();
        downPub = GlobalMessagePipe.GetPublisher<InputLayerSO, DownInput>();
        enterPub = GlobalMessagePipe.GetPublisher<InputLayerSO, EnterInput>();
        cancelPub = GlobalMessagePipe.GetPublisher<InputLayerSO, CancelInput>();
        mapPub = GlobalMessagePipe.GetPublisher<InputLayerSO, MapInput>();

        menuPub = GlobalMessagePipe.GetPublisher<InputLayerSO, MenuInput>();

        scrollPub = GlobalMessagePipe.GetPublisher<InputLayerSO, ScrollInput>();

        holdoutPub = GlobalMessagePipe.GetPublisher<Holdout>();



        

        
    }



    public void RightInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            rightPub.Publish(currentInputLayer.inputLayerSO, new RightInput());

            //RightInputPublisher.Publish(new RightInput());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            holdoutPub.Publish(new Holdout());
        }
    }

    public void LeftInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            leftPub.Publish(currentInputLayer.inputLayerSO, new LeftInput());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            holdoutPub.Publish(new Holdout());
        }
    }

    public void UpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //Debug.Log("DunUp");
            //var _UpInputPublisher = GlobalMessagePipe.GetPublisher<InputLayerSO, DungeonSceneMessage.UpInput>();
            upPub.Publish(currentInputLayer.inputLayerSO, new UpInput());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            holdoutPub.Publish(new Holdout());
        }
    }

    public void DownInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //Debug.Log(currentInputLayer.inputLayerSO);

            downPub.Publish(currentInputLayer.inputLayerSO, new DownInput());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            holdoutPub.Publish(new Holdout());
        }

    }

    public void EnterInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            enterPub.Publish(currentInputLayer.inputLayerSO, new EnterInput());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            holdoutPub.Publish(new Holdout());
        }
    }

    public void CancelInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            cancelPub.Publish(currentInputLayer.inputLayerSO, new CancelInput());
        }
    }

    public void MapInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            mapPub.Publish(currentInputLayer.inputLayerSO, new MapInput());
        }
    }
    public void MenuInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //Debug.Log("menu");
            menuPub.Publish(currentInputLayer.inputLayerSO, new MenuInput());
        }
    }
    
    public void WheelUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //context
            //{ action=BattleScene/WheelUp[/Mouse/scroll] phase=Performed time=5.32974370000011 control=Delta:/Mouse/scroll value=(0.00, -120.00) interaction= }
            scrollPub.Publish(currentInputLayer.inputLayerSO, new ScrollInput(context.ReadValue<Vector2>().y));

            //_CancelInputPublisher.Publish(currentInputLayer.inputLayerSO, new CancelInput());
        }
    }
    
}

