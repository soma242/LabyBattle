using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;


//InputSystemからMessagePipeを起動する。
//イベントを追加後インスペクターからPlayerInputに追加を行う。

public class InputSysetemOfBattleScene : MonoBehaviour
{

    [Inject] private readonly IPublisher<InputLayerSO, RightInput> _RightInputPublisher;
    [Inject] private readonly IPublisher<InputLayerSO, LeftInput> _LeftInputPublisher;
    [Inject] private readonly IPublisher<InputLayerSO, UpInput> _UpInputPublisher;
    [Inject] private readonly IPublisher<InputLayerSO, DownInput> _DownInputPublisher;
    [Inject] private readonly IPublisher<InputLayerSO, EnterInput> _EnterInputPublisher;
    [Inject] private readonly IPublisher<InputLayerSO, CancelInput> _CancelInputPublisher;

    [Inject] private readonly IPublisher<Holdout> holdoutPub;


    //[Inject] private readonly ISubscriber<DownHolding> downHoldSub;

    
    //[Inject] private readonly IPublisher<RightInput> RightInputPublisher;

    //[SerializeField]
    private CurrentInputLayerOfBattleScene currentInputLayerOfBattleScene;

    void Awake()
    {
        currentInputLayerOfBattleScene = GetComponent<CurrentInputLayerOfBattleScene>();
    }


    public void RightInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            _RightInputPublisher.Publish(currentInputLayerOfBattleScene.inputLayerSO, new RightInput());

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
            _LeftInputPublisher.Publish(currentInputLayerOfBattleScene.inputLayerSO, new LeftInput());
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
            _UpInputPublisher.Publish(currentInputLayerOfBattleScene.inputLayerSO, new UpInput());
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
            _DownInputPublisher.Publish(currentInputLayerOfBattleScene.inputLayerSO, new DownInput());
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
            _EnterInputPublisher.Publish(currentInputLayerOfBattleScene.inputLayerSO, new EnterInput());
        }
    }

    public void CancelInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            _CancelInputPublisher.Publish(currentInputLayerOfBattleScene.inputLayerSO, new CancelInput());
        }
    }
}
