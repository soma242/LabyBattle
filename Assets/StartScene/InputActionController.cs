using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

using Cysharp.Threading.Tasks;
using MessagePipe;

public class InputActionController : MonoBehaviour
{
    //private ISubscriber<InputSystemSwitch> sceneSub;

    private PlayerInput system;

    [SerializeField]
    private MSO_InputLayerHolder holder;

    //UnloadSceneÇµÇΩå„ÇÃInputLayerÇï€éùÇµÇƒÇ®Ç≠
    public InputLayerSO dungeonLayer;




    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableLayer;

    void Awake()
    {

        //Debug.Log("awake");

        system = GetComponent<PlayerInput>();

        var sceneSub = GlobalMessagePipe.GetSubscriber<InputSystemSwitch>();

        disposableOnDestroy = sceneSub.Subscribe(async get =>
        {

            Debug.Log(get.scene);

            switch (get.scene)
            {
                case SceneName.start:
                    system.SwitchCurrentActionMap("StartSceneAction");
                    Debug.Log(system.currentActionMap);
                    break;


                case SceneName.dungeon:
                    system.SwitchCurrentActionMap("DungeonScene");
                    Debug.Log(system.currentActionMap);
                    var sceneSub = GlobalMessagePipe.GetSubscriber<InputSystemSwitch>();
                    await UniTask.NextFrame();
                    //ëºÇÃSceneÇ…à⁄çsÇµÇΩéûÇ…ÇªÇÃéûì_ÇÃLayerÇï€ë∂

                    disposableLayer = sceneSub.Subscribe(get =>
                    {
                        disposableLayer?.Dispose();
                        //Debug.Log("layerSub");

                        dungeonLayer = holder.inputLayerSO;

                    });

                    break;
                case SceneName.battle:
                    system.SwitchCurrentActionMap("BattleScene");
                    Debug.Log(system.currentActionMap);
                    break;
                case SceneName.menu:
                    system.SwitchCurrentActionMap("MenuScene");
                    Debug.Log(system.currentActionMap);
                    break;
            }
        });

    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
        disposableLayer?.Dispose();
    }
}
