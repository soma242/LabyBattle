using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using MessagePipe;

using Cysharp.Threading.Tasks;
using System.Threading;
#pragma warning disable CS1998 // disable warning
#pragma warning disable CS4014 // disable warning


using BattleSceneMessage;
using SkillStruct;


[CreateAssetMenu(menuName = "MessageableSO/Component/Formation/Enemy")]
public class MSO_FormationEnemySO : MSO_FormationBaseMSO, IGetFormationInfo
{
    public MSO_EnemyData enemy;

    private int skillNum;
   // private int skillKey;

    private int targetRate;
    private sbyte targetForm;

    private int totalWeight;

    private IPublisher<ReturnTargetName> returnNamePub;
    private ISubscriber<sbyte, GetNextTargetName> nextTargetSub;
    private IPublisher<sbyte, GetNextTargetName> nextTargetPub;
    private ISubscriber<sbyte, GetPreTargetName> preTargetSub;
    private IPublisher<sbyte, GetPreTargetName> preTargetPub;

    private IAsyncSubscriber<FormationPrepareMessage> formPreASub;
    private IAsyncSubscriber<BattlePrepareMessage> prepareASub;


    private IPublisher<sbyte, SetEnemyImage> setImagePub;

    private System.IDisposable disposable;
    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableOnExclusion;
    private System.IDisposable disposableTarget;
    private System.IDisposable disposableTaunt;


    private IAsyncSubscriber<EnemyActionSetMessage> actionSetASub;

    private IAsyncPublisher<EnemyTargetGetMessage> targetGetAPub;
    private ISubscriber<EnemyTargetReturn> targetReturnSub;

    private ISubscriber<EnemyTargetingAllFrontChara> allFrontSub;
    private IPublisher<EnemyTargetFrontSimulate> frontSimuPub;
    
    private ISubscriber<EnemyTargetingSingleChara> singleCharaSub;
    private IPublisher<EnemyTargetSingleCharaSimulate> singleCharaSimuPub;

    private ISubscriber<CommonActiveSkillTiming> commonActiveBookSub;
    //agility return�p
    private IAsyncSubscriber<GetCommonActionAgility> commonGetAgiASub;
    private IPublisher<ReturnAgilityMessage> returnAgiPub;

    private ISubscriber<sbyte, ActiveSkillBootMessage> activeBootSub;
    private IPublisher<ActiveSkillCommand> activeCommandPub;


    private System.IDisposable disposableTiming;
    private System.IDisposable disposableActiveAttention;





    //taunt �Ή�
    private ISubscriber<sbyte, TauntApproachMessage> tauntApproachSub;
    private IPublisher<sbyte, TauntSuccessMessage> tauntSuccessPub;


    public Effects effects { get; private set; }


    public override void MessageStart()
    {
        CommonSubRegist();


        effects = new Effects(GetFormNum());


        returnNamePub = GlobalMessagePipe.GetPublisher<ReturnTargetName>();
        nextTargetSub = GlobalMessagePipe.GetSubscriber<sbyte, GetNextTargetName>();
        nextTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetNextTargetName>();        
        preTargetSub = GlobalMessagePipe.GetSubscriber<sbyte, GetPreTargetName>();
        preTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetPreTargetName>();

        actionSetASub = GlobalMessagePipe.GetAsyncSubscriber<EnemyActionSetMessage>();

        targetGetAPub = GlobalMessagePipe.GetAsyncPublisher<EnemyTargetGetMessage>();
        targetReturnSub = GlobalMessagePipe.GetSubscriber<EnemyTargetReturn>();

        allFrontSub = GlobalMessagePipe.GetSubscriber<EnemyTargetingAllFrontChara>();
        frontSimuPub = GlobalMessagePipe.GetPublisher<EnemyTargetFrontSimulate>();

        singleCharaSub = GlobalMessagePipe.GetSubscriber<EnemyTargetingSingleChara>();
        singleCharaSimuPub = GlobalMessagePipe.GetPublisher<EnemyTargetSingleCharaSimulate>();

        formPreASub = GlobalMessagePipe.GetAsyncSubscriber<FormationPrepareMessage>();
        prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattlePrepareMessage>();

        commonActiveBookSub = GlobalMessagePipe.GetSubscriber<CommonActiveSkillTiming>();
        commonGetAgiASub = GlobalMessagePipe.GetAsyncSubscriber<GetCommonActionAgility>();
        returnAgiPub = GlobalMessagePipe.GetPublisher<ReturnAgilityMessage>();



        activeBootSub = GlobalMessagePipe.GetSubscriber<sbyte, ActiveSkillBootMessage>();
        activeCommandPub = GlobalMessagePipe.GetPublisher<ActiveSkillCommand>();



        tauntApproachSub = GlobalMessagePipe.GetSubscriber<sbyte, TauntApproachMessage>();
        tauntSuccessPub = GlobalMessagePipe.GetPublisher<sbyte, TauntSuccessMessage>();


        setImagePub = GlobalMessagePipe.GetPublisher<sbyte, SetEnemyImage>();

        var bag = DisposableBag.CreateBuilder();



        formPreASub.Subscribe(async(get,ct) =>
        {
            disposableOnExclusion?.Dispose();
            if (enemy != null)
            {
                participant = true;
                currentHP = enemy.GetMaxHP();
                setImagePub.Publish(GetFormNum(), new SetEnemyImage());
                await PrepareBattleSub(ct);
            }
            else
            {
                participant = false;
                PrepareWatchingSub();
            }
        }).AddTo(bag);

        //�G�l�~�[���̓o�^
        /*
        formCharaSub.Subscribe(formNumSO.formationNum, i =>
        {
            enemy = ;

        }).AddTo(bag);
        */

        

        disposableOnDestroy = bag.Build();
    }


    private async UniTask PrepareBattleSub(CancellationToken thisCt)
    {
        disposableOnExclusion?.Dispose();
        disposableTaunt?.Dispose();
        disposableTarget?.Dispose();
        disposableTiming?.Dispose();

        var bag = DisposableBag.CreateBuilder();

        prepareASub.Subscribe(async(get,ct) =>
        {
            totalWeight = enemy.skillCatalog.GetTotalSkillWeight();
            //Debug.Log(totalWeight);
        }).AddTo(bag);
        /*
        turnStartASub.Subscribe(async(get, ct) =>
        {
        }).AddTo(bag);
        */


        //������ACtionSelect�ɑ΂��閼�O�̕Ԃ�
        nextTargetSub.Subscribe(GetFormNum(), get =>
        {
            //Debug.Log(this.name);
            returnNamePub.Publish(new ReturnTargetName(enemy.GetEnemyName(), GetFormNum()));
        }).AddTo(bag);

        preTargetSub.Subscribe(GetFormNum(), get =>
        {
            returnNamePub.Publish(new ReturnTargetName(enemy.GetEnemyName(), GetFormNum()));
        }).AddTo(bag);

        //�^�[���J�n����ɃG�l�~�[�̍s�������肷��B
        actionSetASub.Subscribe(async (get, ct) =>
        {

            //�z���_�[���ł̔z��ԍ�
            //var i = Mathf.RoundToInt(totalWeight * Random.value);
            //Debug.Log(i);
            skillNum = enemy.skillCatalog.GetSelectSkill(Mathf.RoundToInt(totalWeight * Random.value));


            //�^�[�Q�b�g�̎�ނ��󂯎��Sub
            var bagT = DisposableBag.CreateBuilder();

            //�G�l�~�[�̍s���̃^�[�Q�b�g�̎�ނ��Ƃ̏���
            allFrontSub.Subscribe(get =>
            {
                //�e��ނŕK��Dispose
                disposableTarget?.Dispose();

                targetForm = FormationScope.FrontChara();
                //simulate�p��comp�ւ̃����Z�[�W
                frontSimuPub.Publish(new EnemyTargetFrontSimulate(GetFormNum()));
                //Debug.Log(targetForm);

                var bag = DisposableBag.CreateBuilder();

                tauntApproachSub.Subscribe(GetFormNum(), get =>
                {
                    targetForm = get.movePos.user;
                    tauntSuccessPub.Publish(get.movePos.user, new TauntSuccessMessage(GetFormNum()));
                    Debug.Log("taunt");
                }).AddTo(bag);

                disposableTaunt = bag.Build();

            }).AddTo(bagT);

            singleCharaSub.Subscribe(get =>
            {
                //�e��ނŕK��Dispose
                disposableTarget?.Dispose();

                targetRate = 0;

                //Debug.Log(GetFormNum());

                var d = targetReturnSub.Subscribe(get =>
                {
                    //Debug.Log(get.targetForm + ":" + get.targetRate);

                    if (targetRate > get.targetRate)
                    {
                        return;
                    }
                    else if (targetRate < get.targetRate)
                    {
                        targetRate = get.targetRate;
                        targetForm = get.targetForm;
                    }
                });
                targetGetAPub.PublishAsync(new EnemyTargetGetMessage());
                d.Dispose();
                singleCharaSimuPub.Publish(new EnemyTargetSingleCharaSimulate(GetFormNum(), targetForm));
            }).AddTo(bagT);

            disposableTarget = bagT.Build();


            //�^�[�Q�b�g�̎�ނɑΉ�����Pub���s���B
            enemy.SetEnemySkillTarget(skillNum);

            var bagTime = DisposableBag.CreateBuilder();


            commonActiveBookSub.Subscribe(get =>
            {
                disposableTiming?.Dispose();

                var bagA = DisposableBag.CreateBuilder();

                //���������Ăяo����Ă���
                //=>disposableTiming��Dispose��������Y��
                //dispose���Ă����Ăяo����Ă���(Chara���ł͈��Ȃ̂��m�F)
                //=>dispose���Ԃɍ����Ă��炸�CcommonActiveBookSub������Pub�ɂ��������Ă��܂��Ă���
                //==>disposable�̍\�z�^�C�~���O���x������(dispose�̃^�C�~���O�ɊԂɍ����ĂȂ�)���Ƃ����R���ۂ�
                commonGetAgiASub.Subscribe(async (get, ct) =>
                {
                    //Debug.Log(GetFormNum());
                    returnAgiPub.Publish(new ReturnAgilityMessage(GetActualAgility(), GetFormNum()));
                }).AddTo(bagA);

                //Debug.Log(name);


                activeBootSub.Subscribe(GetFormNum(), get =>
                {
                    //Debug.Log(name);
                    //Debug.Log(skillNum);
                    activeCommandPub.Publish(new ActiveSkillCommand(enemy.GetSettedSkillKey(skillNum), targetForm, this));
                    disposableActiveAttention?.Dispose();
                }).AddTo(bagA);

                disposableActiveAttention = bagA.Build();

            }).AddTo(bagTime);

            disposableTiming = bagTime.Build();


            //Debug.Log("setTime");
            enemy.SetEnemySkillTiming(skillNum);


            //await UniTask.NextFrame();

            //Debug.Log(targetForm);
        }).AddTo(bag);

        NormalDamageSubPrepare(GetFormNum(), bag);
        NormalDamageSubPrepare(FormationScope.AllEnemy(), bag);

        var checkGetSub = GlobalMessagePipe.GetSubscriber<KnockOutEnemy>();
        checkGetSub.Subscribe(get =>
        {
            var checkReturnPub = GlobalMessagePipe.GetPublisher<KnockOutChecker>();
            checkReturnPub.Publish(new KnockOutChecker(GetFormNum()));
        }).AddTo(bag);



        disposableOnExclusion = bag.Build();

    }

    private void PrepareWatchingSub()
    {
        disposableOnExclusion?.Dispose();
        disposableTaunt?.Dispose();
        disposableTarget?.Dispose();
        disposableTiming?.Dispose();



        var bag = DisposableBag.CreateBuilder();
        nextTargetSub.Subscribe(GetFormNum(), get =>
        {
            //Debug.Log("pre"+this.name);
            nextTargetPub.Publish(NextForm(GetFormNum()), new GetNextTargetName());
        }).AddTo(bag);

        preTargetSub.Subscribe(GetFormNum(), get =>
        {
            preTargetPub.Publish(PreForm(GetFormNum()),new GetPreTargetName());
        }).AddTo(bag);

        //Damage
        normalDamageSub.Subscribe(GetFormNum(), get =>
        {
            //Debug.Log(GetFormNum());
            //Debug.Log("watch");
            var normalDamagePub = GlobalMessagePipe.GetPublisher<sbyte, NormalDamageCalcMessage>();
            normalDamagePub.Publish(SbyteHandler.NextForm(GetFormNum()), get);
        }).AddTo(bag);
        normalMagicDamageSub.Subscribe(GetFormNum(), get =>
        {
            //Debug.Log(GetFormNum());
            //Debug.Log("watch");
            var normalDamagePub = GlobalMessagePipe.GetPublisher<sbyte, NormalMagicDamageCalcMessage>();
            normalDamagePub.Publish(SbyteHandler.NextForm(GetFormNum()), get);
        }).AddTo(bag);

        disposableOnExclusion = bag.Build();

    }

    private void NormalDamageSubPrepare(sbyte form, DisposableBagBuilder bag)
    {
        normalDamageSub.Subscribe(form, get =>
        {
            //Debug.Log(GetFormNum());
            //Debug.Log("participant");

            int damage = NormalPDamage(get.damage);
            currentHP -= damage;
            damageNoticePub.Publish(new DamageNoticeMessage(false, get.activePos.target, damage));


        }).AddTo(bag);
        normalMagicDamageSub.Subscribe(form, get=>
        {
            //Debug.Log(GetFormNum());
            //Debug.Log("participant");
            int damage = NormalMDamage(get.damage);
            currentHP -= damage;
            damageNoticePub.Publish(new DamageNoticeMessage(false, GetFormNum(), damage));

            CheckContinuation();
        }).AddTo(bag);
    }



    public int NormalPDamage(float damage)
    {
        float actualDamage = damage - GetActualDefence();
        actualDamage *= effects.GetPosture();
        return Mathf.FloorToInt(actualDamage);
    }
    public int NormalMDamage(float damage)
    {
        float actualDamage = damage - GetActualMagicDefence();
        actualDamage *= effects.GetPosture();

        return Mathf.FloorToInt(actualDamage);
    }


    public string GetSettedSkillName()
    {
        return enemy.GetSettedSkillName(skillNum);
    }

    //IGetFormationInfo
    public string GetTargetName()
    {
        return enemy.GetEnemyName();
    }

    public int GetActualAttack()
    {
        return Mathf.FloorToInt(enemy.GetAttack() * effects.attackBuff.value);
    }
    public int GetActualMagic()
    {
        return Mathf.FloorToInt(enemy.GetMagic() * effects.attackBuff.value);
    }
    public int GetActualDefence()
    {
        return enemy.GetDefence();
    }
    public int GetActualMagicDefence()
    {
        return enemy.GetMagicDefence();
        //Mathf.FloorToInt(setChara.GetAttack() * effects.magicBuff.value);
    }
    public int GetActualAgility()
    {
        return Mathf.FloorToInt(enemy.GetAgility() * effects.agiBuff.value);
    }

    public float GetRatioOnHP()
    {
        return (float)currentHP / (float)enemy.GetMaxHP();
    }
    //


    //�_���[�W����HP���c���Ă��邩�̔�����s��
    private void CheckContinuation()
    {
        if (currentHP < 0)
        {
            participant = false;
            PrepareWatchingSub();

            UniTask.NextFrame();

            var dropEnemyPub = GlobalMessagePipe.GetPublisher<DropEnemyMessage>();
            dropEnemyPub.Publish(new DropEnemyMessage(GetFormNum()));
        }

    }

}
