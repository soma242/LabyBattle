using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

//StateResistance���󂯎����Bool�Ɋ�Â���Subscribe�̎��s
//Chara,Enemy��Function����N�����邱�ƂŔėp�̏ꍇ�̓o�^���ȗ����C���ʂȗ��ǉ����₷������(PureClass�𐶐�)
//Sub���󂯎������ɕK�v�ȕ�(Formation��interface�ɓZ�߂�
//Formation����Effect�ւ̌o�H�CEnemy����StateRegistance�ւ̌o�H, Formation����Dispose����o�H(bag��n����dispose��Build�ł���)
public class EnemyStateResistDetail
{
    public Effects effects;
    public EnemyStateResistDetail(sbyte pos, DisposableBagBuilder bag, Effects effect, EnemyStateResistanceSO valids)
    {
        this.effects = effect;
        if (!valids.breakPosture)
        {
            var breakeSub = GlobalMessagePipe.GetSubscriber<sbyte, BreakePostureMessage>();
            breakeSub.Subscribe(pos, get => {
                if (effect.breakePosture.SetValid())
                {
                    var successPub = GlobalMessagePipe.GetPublisher<BreakPostureSuccessEnemy>();
                    successPub.Publish(new BreakPostureSuccessEnemy(pos));
                }

            }).AddTo(bag);
        }
    }
}

[CreateAssetMenu(menuName = "data/state/EnemyStateResistance")]
public class EnemyStateResistanceSO : ScriptableObject
{
    //false�ŗL��  true�őϐ�����
    public bool breakPosture;
}