
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

#pragma warning disable CS1998 // disable warning


[CreateAssetMenu(menuName = "MessageableSO/Component/skillTree/commonMagic")]
public class MSO_CommonMagicSkillTreeSO :  MSO_BaseSkillTreeSO

{
    //private IAsyncSubscriber<RegistCommonMagicSkill> registCommonASub;
    private System.IDisposable disposable;
    private System.IDisposable disposableRegist;

    public override void MessageStart()
    {

        var registCommonASub = GlobalMessagePipe.GetAsyncSubscriber<RegistCommonMagicSkill>();
        disposable = registCommonASub.Subscribe(async (get, ct) =>
        {
            var bag = DisposableBag.CreateBuilder();
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
