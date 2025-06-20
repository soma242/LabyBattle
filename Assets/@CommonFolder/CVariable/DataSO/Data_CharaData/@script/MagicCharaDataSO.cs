using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using Cysharp.Threading.Tasks;
using SkillStruct;

#pragma warning disable CS4014 // disable warning

[CreateAssetMenu(menuName = "data/CharaData/magic")]

public class MagicCharaDataSO : CharacterDataSO
{



    public override async UniTask RegistMasterySkill(sbyte formNum)
    {
        var registCommonMagicAPub = GlobalMessagePipe.GetAsyncPublisher<RegistCommonMagicSkill>();
        await registCommonMagicAPub.PublishAsync(new RegistCommonMagicSkill(formNum));
        base.RegistMasterySkill(formNum);

    }
}
