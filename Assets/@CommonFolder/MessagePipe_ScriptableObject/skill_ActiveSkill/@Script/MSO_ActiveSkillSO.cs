using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillStruct;

using MessagePipe;

//ActiveSkill�𑝂₵����
//ActiveSkill��SO���쐬
//  Injecter�ɒǉ�
//  ActiveSkillCommander�̃��X�g�ɒǉ�
//���V�������ނȂ�V�K�ɍ쐬����Pub�ɑΉ�����Sub������SoldierSO���쐬
//  Injecter�i���쐬�CPrepare���X�g�ɒǉ���j�ɒǉ�
//���V�����v�Z���@�Ȃ�(�V����Message�ɑΉ�����jDamageCalc�Ɍv�Z���@��ǉ�



public class MSO_ActiveSkillSO : ScriptableObject
{
    public int activeKey;

    public virtual void ActiveSkillBoot(ActiveSkillPosition acitvePos) { }

    public int GetSkillEffectKey()
    {
        return activeKey;
    }


}
