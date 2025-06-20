
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

#pragma warning disable CS1998 // disable warning


[CreateAssetMenu(menuName = "MessageableSO/Component/skillTree/commonPhys")]
public class MSO_CommonPhysicalSkillTreeSO : MSO_BaseSkillTreeSO

{
    //private IAsyncSubscriber<RegistCommonPhysicalSkill> registCommonASub;
    private System.IDisposable disposable;
    private System.IDisposable disposableRegist;

    public override void MessageStart()
    {

        var bag = DisposableBag.CreateBuilder();

        var registCommonASub = GlobalMessagePipe.GetAsyncSubscriber<RegistCommonPhysicalSkill>();
        disposable = registCommonASub.Subscribe(async (get, ct) =>
        {
            foreach (MSO_SkillHolderSO skillHolder in skillCatalog)
            {

                skillHolder.RegistThisSkill(get.formNum, bag);
            }
            var registFinishSub = GlobalMessagePipe.GetSubscriber<RegistSkillFinish>();
            registFinishSub.Subscribe(get =>
            {
                disposableRegist?.Dispose();
            }).AddTo(bag);

            disposableRegist = bag.Build();
        });
    }
}


