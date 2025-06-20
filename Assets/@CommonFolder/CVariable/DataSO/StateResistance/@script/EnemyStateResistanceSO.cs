using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using SkillStruct;

//StateResistanceを受け取ってBoolに基づいてSubscribeの実行
//Chara,EnemyのFunctionから起動することで汎用の場合の登録を省略し，特別な例を追加しやすくする(PureClassを生成)
//Subを受け取った後に必要な物(Formationのinterfaceに纏める
//Formation内のEffectへの経路，Enemy内のStateRegistanceへの経路, FormationからDisposeする経路(bagを渡してdisposeをBuildでいい)
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
    //falseで有効  trueで耐性あり
    public bool breakPosture;
}