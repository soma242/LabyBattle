using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

using MessagePipe;


using BattleSceneMessage;

/*
//InputSystemからMessagePipeを起動する。
//イベントを追加後インスペクターからPlayerInputに追加を行う。

public class InputSysetemOfBattleScene : MonoBehaviour
{

    private IPublisher<InputLayerSO, RightInput> rightPub;
    private IPublisher<InputLayerSO, LeftInput> leftPub;
    private IPublisher<InputLayerSO, UpInput> upPub;
    private IPublisher<InputLayerSO, DownInput> downPub;
    private IPublisher<InputLayerSO, EnterInput> enterPub;
    private IPublisher<InputLayerSO, CancelInput> cancelPub;

    private IPublisher<InputLayerSO, ScrollInput> scrollPub;



    private IPublisher<Holdout> holdoutPub;


    //private ISubscriber<DownHolding> downHoldSub;

    
    //private IPublisher<RightInput> RightInputPublisher;

    //[SerializeField]
    private InputLayerHolder currentInputLayerOfBattleScene;

    void Awake()
    {
        currentInputLayerOfBattleScene = GetComponent<InputLayerHolder>();

        rightPub = GlobalMessagePipe.GetPublisher<InputLayerSO, RightInput>();
        leftPub = GlobalMessagePipe.GetPublisher<InputLayerSO, LeftInput>();
        upPub = GlobalMessagePipe.GetPublisher<InputLayerSO, UpInput>();
        downPub = GlobalMessagePipe.GetPublisher<InputLayerSO, DownInput>();
        enterPub = GlobalMessagePipe.GetPublisher<InputLayerSO, EnterInput>();
        cancelPub = GlobalMessagePipe.GetPublisher<InputLayerSO, CancelInput>();

        scrollPub = GlobalMessagePipe.GetPublisher<InputLayerSO, ScrollInput>();

        holdoutPub = GlobalMessagePipe.GetPublisher<Holdout>();

    }



    public void RightInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            rightPub.Publish(currentInputLayerOfBattleScene.inputLayerSO, new RightInput());

            //RightInputPublisher.Publish(new RightInput());
        }else if(context.phase == InputActionPhase.Canceled)
        {
            holdoutPub.Publish(new Holdout());
        }
    }

    public void LeftInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            leftPub.Publish(currentInputLayerOfBattleScene.inputLayerSO, new LeftInput());
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
            upPub.Publish(currentInputLayerOfBattleScene.inputLayerSO, new UpInput());
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
            downPub.Publish(currentInputLayerOfBattleScene.inputLayerSO, new DownInput());
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
            enterPub.Publish(currentInputLayerOfBattleScene.inputLayerSO, new EnterInput());
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
            cancelPub.Publish(currentInputLayerOfBattleScene.inputLayerSO, new CancelInput());
        }
    }
    
    public void WheelUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //context
            //{ action=BattleScene/WheelUp[/Mouse/scroll] phase=Performed time=5.32974370000011 control=Delta:/Mouse/scroll value=(0.00, -120.00) interaction= }
            scrollPub.Publish(currentInputLayerOfBattleScene.inputLayerSO, new ScrollInput(context.ReadValue<Vector2>().y));

            //_CancelInputPublisher.Publish(currentInputLayerOfBattleScene.inputLayerSO, new CancelInput());
        }
    }
}
*/