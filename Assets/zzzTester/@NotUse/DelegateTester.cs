/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

public class DelegateTester : MonoBehaviour
{
    [SerializeField]
    private ActiveSkillEnumSO key;


    private delegate void PerformSoldier();

    [Inject] private readonly ISubscriber<BattleStart> battleSub;


    void Awake()
    {
        battleSub.Subscribe(i =>
        {
            DelegetePerfome();
        });
        PerformSoldier performSoldier = new PerformSoldier(NotRegist);
    }


    private void DelegetePerfome()
    {
        performSoldier = key.name;
        performSoldier();
    }

    private void NotRegist()
    {

    }

    private void Assist()
    {
        Debug.Log("Assist");
    }

}
*/