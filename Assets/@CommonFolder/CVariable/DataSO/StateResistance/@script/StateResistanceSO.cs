using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

//StateResistance���󂯎����Bool�Ɋ�Â���Subscribe�̎��s
//Chara,Enemy��Function����N�����邱�ƂŔėp�̏ꍇ�̓o�^���ȗ����C���ʂȗ��ǉ����₷������(PureClass�𐶐�)
//Sub���󂯎������ɕK�v�ȕ�(Formation��interface�ɓZ�߂�
//Formation����Effect�ւ̌o�H�CEnemy����StateRegistance�ւ̌o�H, Formation����Dispose����o�H(bag��n����dispose��Build�ł���)
public class StateResist
{
    public Effects effects;
    public StateResist(sbyte pos, DisposableBagBuilder bag, Effects effect)
    {
        this.effects = effect;

    }
}

[CreateAssetMenu(menuName = "data/state/StateResistance")]
public class StateResistanceSO : ScriptableObject
{
    //false�ŗL��  true�őϐ�����
    //public bool breakPosture;
}


