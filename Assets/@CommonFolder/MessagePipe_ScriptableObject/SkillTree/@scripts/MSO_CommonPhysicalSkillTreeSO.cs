
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

#pragma warning disable CS1998 // disable warning


[CreateAssetMenu(menuName = "MessageableSO/Component/skillTree/commonPhys")]
public class MSO_CommonPhysicalSkillTreeSO : MSO_BaseSkillTreeSO

{
    private IAsyncSubscriber<RegistCommonPhysicalSkill> registCommonASub;
    private System.IDisposable disposable;

    public override void MessageStart()
    {
        registCommonASub = GlobalMessagePipe.GetAsyncSubscriber<RegistCommonPhysicalSkill>();
        disposable = registCommonASub.Subscribe(async (get, ct) =>
        {
            foreach (MSO_SkillHolderSO skillHolder in skillCatalog)
            {

                skillHolder.RegistThisSkill(get.formNum);
            }
        });
    }
}


