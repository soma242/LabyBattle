using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using Cysharp.Threading.Tasks;
using SkillStruct;

#pragma warning disable CS4014 // disable warning


[CreateAssetMenu(menuName = "data/CharaData/physical")]
public class NormalCharaDataSO : CharacterDataSO
{




    public override async UniTask RegistMasterySkill(sbyte formNum)
    {
        var registCommonMagicAPub = GlobalMessagePipe.GetAsyncPublisher<RegistCommonPhysicalSkill>();
        await registCommonMagicAPub.PublishAsync(new RegistCommonPhysicalSkill(formNum));
        base.RegistMasterySkill(formNum);

    }


}
