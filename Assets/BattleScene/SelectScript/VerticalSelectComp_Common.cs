using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using BattleSceneMessage;

#pragma warning disable CS4014 // disable warning


public class VerticalSelectComp_Common : BaseVerticalSelectCompOfBattleScene
{

    //�C�x���g����
    void Awake()
    {
        _Awake();
    }





    //Select����
    public override void SelectThisComponent() 
    {
        _SelectThisComponent();
    }
    public override void UnselectThisComponent() 
    {
        _UnselectThisComponent();
    }
}
