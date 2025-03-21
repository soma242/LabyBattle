using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using Cysharp.Threading.Tasks;
using SkillStruct;

#pragma warning disable CS4014 // disable warning

[CreateAssetMenu(menuName = "MessageableSO/Component/CharaData/magic")]

public class MSO_MagicCharaDataSO : MSO_CharacterDataSO
{
    private IAsyncPublisher<RegistCommonMagicSkill> registCommonMagicAPub;

    public override void MessageStart()
    {
        base.MessageStart();
        registCommonMagicAPub = GlobalMessagePipe.GetAsyncPublisher<RegistCommonMagicSkill>();
    }

    public override async UniTask RegistMasterySkill(sbyte formNum)
    {
        await registCommonMagicAPub.PublishAsync(new RegistCommonMagicSkill(formNum));
        base.RegistMasterySkill(formNum);

    }
}
