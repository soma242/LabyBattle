using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MessagePipe;
using BattleSceneMessage;

public class CharacterSimulatorCont : MonoBehaviour
{
    [SerializeField]
    private List<Canvas> allyCanvas = new List<Canvas>();

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        var dropSub = GlobalMessagePipe.GetSubscriber<DropCharaMessage>();
        disposableOnDestroy = dropSub.Subscribe(get =>
        {
            allyCanvas[FormationScope.FormToListChara(get.pos)].enabled = false;
        });
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }
}
