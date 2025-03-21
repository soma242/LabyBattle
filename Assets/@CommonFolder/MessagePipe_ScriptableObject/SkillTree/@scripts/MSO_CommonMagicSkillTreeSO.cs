
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

#pragma warning disable CS1998 // disable warning


[CreateAssetMenu(menuName = "MessageableSO/Component/skillTree/commonMagic")]
public class MSO_CommonMagicSkillTreeSO :  MSO_BaseSkillTreeSO

{
    private IAsyncSubscriber<RegistCommonMagicSkill> registCommonASub;
    private System.IDisposable disposable;

    public override void MessageStart()
    {
        registCommonASub = GlobalMessagePipe.GetAsyncSubscriber<RegistCommonMagicSkill>();
        disposable = registCommonASub.Subscribe(async (get, ct) =>
        {
            foreach (MSO_SkillHolderSO skillHolder in skillCatalog)
            {

                skillHolder.RegistThisSkill(get.formNum);
            }
        });
    }
}
