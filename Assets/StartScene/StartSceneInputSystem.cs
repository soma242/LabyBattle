/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


using MessagePipe;
using VContainer;
using VContainer.Unity;

using StartSceneMessage;

public class StartSceneInputSystem : MonoBehaviour
{




    private IPublisher<InputLayerSO, UpInput> upPub;
    private IPublisher<InputLayerSO, DownInput> downPub;
    private IPublisher<InputLayerSO, EnterInput> enterPub;


    

    private IPublisher<Holdout> holdoutPub;





    void Awake()
    {
        upPub = GlobalMessagePipe.GetPublisher<InputLayerSO, UpInput>();
        downPub = GlobalMessagePipe.GetPublisher<InputLayerSO, DownInput>();

        enterPub = GlobalMessagePipe.GetPublisher<InputLayerSO, EnterInput>();


        holdoutPub = GlobalMessagePipe.GetPublisher<Holdout>();


    }


    public void UpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            upPub.Publish( new UpInput());
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
            //Debug.Log("downSS");
            downPub.Publish( new DownInput());
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
            enterPub.Publish( new EnterInput());
        }

    }





}
*/