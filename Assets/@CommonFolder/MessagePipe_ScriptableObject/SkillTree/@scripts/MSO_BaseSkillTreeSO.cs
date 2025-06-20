using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

public class MSO_BaseSkillTreeSO : MessageableScriptableObject
{
    public string treeName;
    public List<MSO_SkillHolderSO> skillCatalog = new List<MSO_SkillHolderSO>();

    private System.IDisposable disposable;

    public  void RegistMasterySkill(int level, sbyte formNum)
    {
        if(level==0)
            return;

        var bag = DisposableBag.CreateBuilder();

        //0-4, 0-4
        //=>level4‚Ì‚Æ‚«0-3
        //=>level1‚Ì‚Æ‚«0
        for (int i = -1; i < level-1 ; i++)
        {
            skillCatalog[i+1].RegistThisSkill(formNum, bag);
        }
        var registFinishSub = GlobalMessagePipe.GetSubscriber<RegistSkillFinish>();
        registFinishSub.Subscribe(get =>
        {
            disposable?.Dispose();
        }).AddTo(bag);

        disposable = bag.Build();

    }
}
