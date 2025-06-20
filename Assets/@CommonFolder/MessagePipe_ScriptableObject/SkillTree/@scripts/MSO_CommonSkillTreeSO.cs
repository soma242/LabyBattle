using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

#pragma warning disable CS1998 // disable warning


[CreateAssetMenu(menuName = "MessageableSO/Component/skillTree/common")]

public class MSO_CommonSkillTreeSO : MSO_BaseSkillTreeSO
{
    //private IAsyncSubscriber<RegistCommonSkill> registCommonASub;
    private System.IDisposable disposable;
    private System.IDisposable disposableRegist;

    public override void MessageStart()
    {
        var bag = DisposableBag.CreateBuilder();

        //Common‚ÌƒXƒLƒ‹‚Í’Ç‰Á‚¹‚¸‚Æ‚à“o˜^‚µ‚½‚¢
        var registCommonASub = GlobalMessagePipe.GetAsyncSubscriber<RegistCommonSkill>();
        disposable = registCommonASub.Subscribe(async (get, ct) =>
        {
            foreach(MSO_SkillHolderSO skillHolder in skillCatalog)
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
