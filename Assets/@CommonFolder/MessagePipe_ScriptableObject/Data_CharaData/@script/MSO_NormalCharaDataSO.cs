using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using Cysharp.Threading.Tasks;
using SkillStruct;

#pragma warning disable CS4014 // disable warning


[CreateAssetMenu(menuName = "MessageableSO/Component/CharaData/physical")]
public class MSO_NormalCharaDataSO : MSO_CharacterDataSO
{

    private IAsyncPublisher<RegistCommonPhysicalSkill> registCommonMagicAPub;

    public override void MessageStart()
    {
        base.MessageStart();
        registCommonMagicAPub = GlobalMessagePipe.GetAsyncPublisher<RegistCommonPhysicalSkill>();
    }

    public override async UniTask RegistMasterySkill(sbyte formNum)
    {
        await registCommonMagicAPub.PublishAsync(new RegistCommonPhysicalSkill(formNum));
        base.RegistMasterySkill(formNum);

    }


}
