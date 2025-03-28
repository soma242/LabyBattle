using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

using Cysharp.Threading.Tasks;

using MessagePipe;
using SkillStruct;
using BattleSceneMessage;

[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/formation/chara")]
public class MSO_FormationCommander : MessageableScriptableObject
{

    private sbyte count;

    [SerializeField]
    private List<MSO_FormationCharaSO> formationCharaCatalog = new List<MSO_FormationCharaSO>();
    
    [SerializeField]
    private List<MSO_FormationEnemySO> formationEnemyCatalog = new List<MSO_FormationEnemySO>();

    /*
    private ISubscriber<NormalMoveToFront> moveToFrontSub;
    private ISubscriber<NormalMoveToBack> moveToBackSub;

    */

    private System.IDisposable disposable;

    private ISubscriber<BattleStartMessage> battleStartSub;
    private System.IDisposable disposableBattle;
    private System.IDisposable disposableCheckConti;


    public override void MessageStart()
    {

#if UNITY_EDITOR
        formationCharaCatalog.TrimExcess();
        formationEnemyCatalog.TrimExcess();
#endif

        battleStartSub = GlobalMessagePipe.GetSubscriber<BattleStartMessage>();

        //バトル開始時にSub開始
        disposable = battleStartSub.Subscribe(GetBattleImage =>
        {
            var bag = DisposableBag.CreateBuilder();

            //Enemyの全滅判定
            var dropEmemySub = GlobalMessagePipe.GetSubscriber<DropEnemyMessage>();
            var dropCharaSub = GlobalMessagePipe.GetSubscriber<DropCharaMessage>();

            dropEmemySub.Subscribe(get =>
            {
                count = FormationScope.NoneChara();
                var checkReturnSub = GlobalMessagePipe.GetSubscriber<KnockOutChecker>();
                disposableCheckConti = checkReturnSub.Subscribe(get =>
                {
                    
                    count = get.pos;
                    Debug.Log("count: "+count);
                });
                var checkGetPub = GlobalMessagePipe.GetPublisher<KnockOutEnemy>();
                checkGetPub.Publish(new KnockOutEnemy());
                disposableCheckConti?.Dispose();

                UniTask.NextFrame();

                if (count == 0)
                {
                    Debug.Log("count: " + count);

                    Debug.Log("all down");
                    var allDownPub = GlobalMessagePipe.GetPublisher<AllEnemyDownMessage>();
                    allDownPub.Publish(new AllEnemyDownMessage());
                }
            }).AddTo(bag);
            //Charaの全滅判定
            dropCharaSub.Subscribe(get =>
            {
                count = FormationScope.NoneChara();
                var checkReturnSub = GlobalMessagePipe.GetSubscriber<KnockOutChecker>();
                disposableCheckConti = checkReturnSub.Subscribe(get =>
                {
                    count = get.pos;
                    Debug.Log("count: " + count);
                });
                var checkGetPub = GlobalMessagePipe.GetPublisher<KnockOutChara>();
                checkGetPub.Publish(new KnockOutChara());
                disposableCheckConti?.Dispose();

                UniTask.NextFrame();
                //Debug.Log("count: " + count);

                if (count == 0)
                {
                    Debug.Log("all down");
                    var allDownPub = GlobalMessagePipe.GetPublisher<AllCharaDownMessage>();
                    allDownPub.Publish(new AllCharaDownMessage());
                }
            }).AddTo(bag);

            var endSub = GlobalMessagePipe.GetSubscriber<BattleFinishMessage>();
            endSub.Subscribe(get =>
            {
                disposableBattle?.Dispose();
            }).AddTo(bag);

            disposableBattle = bag.Build();

        });



    }

    //Chara
    public string GetCharaName(int i)
    {
        return formationCharaCatalog[i].setChara.GetCharaName();
    }
    public bool GetParticipant(int i)
    {
        return formationCharaCatalog[i].participant;
    }

    public Sprite GetBattleImage(int i)
    {
        return formationCharaCatalog[i].setChara.charaImages.battleImage;
    }

    public MovePosition GetMovePosition(int i)
    {
        return formationCharaCatalog[i].currentPos;
    }

    public float GetCharaRatioOnHP(int i)
    {
        //Debug.Log(formationCharaCatalog[i].GetRatioOnHP());
        return formationCharaCatalog[i].GetRatioOnHP();
    }

    //Enemy
    public string GetEnemyName(int i)
    {
        return formationEnemyCatalog[i].enemy.GetEnemyName();
    }

    public string GetSettedSkillName(int i)
    {
        return formationEnemyCatalog[i].GetSettedSkillName();
    }

    public bool GetEnemyParticipant(int i)
    {
        return formationEnemyCatalog[i].participant;
    }

    public float GetEnemyRatioOnHP(int i)
    {
        return formationEnemyCatalog[i].GetRatioOnHP();
    }
}
