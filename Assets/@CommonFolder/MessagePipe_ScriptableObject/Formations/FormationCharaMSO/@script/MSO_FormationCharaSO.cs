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

public enum MovePosition
{
    front,
    back
}


[CreateAssetMenu(menuName = "MessageableSO/Component/Formation/Chara")]
public class MSO_FormationCharaSO : MSO_FormationBaseMSO, IGetFormationInfo
{

    public MSO_CharacterDataSO setChara;

    //private sbyte allChara = 21;

    //private int targetingRateOfFormation;

    private sbyte moveTarget;
    private int moveKey;

    private sbyte activeTarget;
    private int activeKey;

    [SerializeField]
    protected SkillListHolderSO skillListHolderSO;

    public MovePosition currentPos;
    public MovePosition firstPos;


    //Battle�̊J�n���ɁC�L�������ւ񂹂�����Ă����Subscribe�Ȃǂ̏������s���B
    private IAsyncSubscriber<FormationPrepareMessage> formPreASub;

    private IAsyncSubscriber<sbyte, RegistSkillStart> registStartASub;

    //�X�L�����X�g�̍X�V
    private ISubscriber<sbyte, RegistActiveSkill> registASkillSub;
    private ISubscriber<sbyte, RegistMoveSkillOnFront> registFrontSub;
    private ISubscriber<sbyte, RegistMoveSkillOnBack> registBackSub;

    //�o�g���̏����i�������Ȃǁj
    //private IAsyncSubscriber<BattlePrepareMessage> prepareASub;

    private IPublisher<MovePositionChangeMessage> positionChangePub;


    //�o�g���I����
    private ISubscriber<BattleFinishMessage> endSub;

    //�L�����N�^�[�̕Ґ�
    private ISubscriber<sbyte, FormCharacterMessage> formCharaSub;

    //�Z�b�g���ꂽ�L�����N�^�[�̃o�g�����̍s���I���J�n
    private ISubscriber<sbyte, ActionSelectStartMessage> selectStartSub;
    private IPublisher<sbyte, ActionSelectStartMessage> selectStartPub;

    private ISubscriber<ActionSelectCancelMessage> selectCancelSub;

    //�s���I��

    private System.IDisposable disposable;

    private System.IDisposable disposableOnExclusion;

    private IPublisher<SelectSkillListChangeMessage> selectListChangePub;
    private IPublisher<SelectCharaNameMessage> selectNameChangePub;

    //Target�pMessagePipe
    private IPublisher<ReturnTargetName> returnNamePub;
    private ISubscriber<sbyte, GetNextTargetName> nextTargetSub;
    private IPublisher<sbyte, GetNextTargetName> nextTargetPub;
    private ISubscriber<sbyte, GetPreTargetName> preTargetSub;
    private IPublisher<sbyte, GetPreTargetName> preTargetPub;

    //private ISubscriber<GetTauntUserName> tauntTargetSub;

    //�X�L���\��p
    private ISubscriber<BookCommonMoveKeyMessage> moveKeyBookSub;
    private ISubscriber<BookCommonMoveTargetMessage> moveTargetBookSub;
    private ISubscriber<BookCommonActiveKeyMessage> activeKeyBookSub;
    private ISubscriber<BookCommonActiveTargetMessage> activeTargetBookSub;


    private ISubscriber<CommonActiveSkillTiming> commonActiveBookSub;
    private System.IDisposable disposableActiveAttention;


    //agility return�p

    private IAsyncSubscriber<GetCommonActionAgility> commonGetAgiASub;
    private IPublisher<ReturnAgilityMessage> returnAgiPub;


    private ISubscriber<BookCompleteMessage> bookCompSub;

    private System.IDisposable disposableBook;

    private ISubscriber<sbyte, ActiveSkillBootMessage> activeBootSub;
    private IPublisher<ActiveSkillCommand> activeCommandPub;
    
    private ISubscriber<sbyte, MoveSkillBootMessage> moveBootSub;
    private IPublisher<MoveSkillCommand> moveCommandPub;

    private IAsyncSubscriber<EnemyTargetGetMessage> enemyTargetGetASub;
    private IPublisher<EnemyTargetReturn> enemyTargetReturnPub;



    private System.IDisposable disposableMove;


    public Effects effects { get; private set; }

    /*
    ~MSO_FormationCharaSO(){
        Debug.Log("calledfina");
        disposable.Dispose();
    }
    */
    



    public override void MessageStart()
    {
        //setChara = null;
        var bag = DisposableBag.CreateBuilder();

        CommonSubRegist();

        effects = new Effects(GetFormNum());
        //attackBuff.SetValue(1.4f,3);

        //�o�g���J�n���Cprepare�̑O�i�K�ŃL�������Ґ�����Ă��邩�m�F���ď������s��
        formPreASub = GlobalMessagePipe.GetAsyncSubscriber<FormationPrepareMessage>();

        //���l�̏������Ȃǃo�g���J�n���̏���
        //prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattlePrepareMessage>();

        //�Ґ��̈ʒu�ƃL�����N�^�[�ɉ����ĕ\������摜�ƈʒu��ύX����
        positionChangePub = GlobalMessagePipe.GetPublisher<MovePositionChangeMessage>();

        //�o�g���I�����ɏ���
        endSub = GlobalMessagePipe.GetSubscriber<BattleFinishMessage>();


        registStartASub = GlobalMessagePipe.GetAsyncSubscriber<sbyte, RegistSkillStart>();

        registASkillSub = GlobalMessagePipe.GetSubscriber<sbyte, RegistActiveSkill>();
        registFrontSub = GlobalMessagePipe.GetSubscriber<sbyte, RegistMoveSkillOnFront>();
        registBackSub = GlobalMessagePipe.GetSubscriber<sbyte, RegistMoveSkillOnBack>();



        formCharaSub = GlobalMessagePipe.GetSubscriber<sbyte, FormCharacterMessage>();

        enemyTargetGetASub = GlobalMessagePipe.GetAsyncSubscriber<EnemyTargetGetMessage>();
        enemyTargetReturnPub = GlobalMessagePipe.GetPublisher<EnemyTargetReturn>();

        selectStartSub = GlobalMessagePipe.GetSubscriber<sbyte, ActionSelectStartMessage>();
        selectStartPub = GlobalMessagePipe.GetPublisher<sbyte, ActionSelectStartMessage>();

        selectCancelSub = GlobalMessagePipe.GetSubscriber<ActionSelectCancelMessage>();

        selectListChangePub = GlobalMessagePipe.GetPublisher<SelectSkillListChangeMessage>();
        selectNameChangePub = GlobalMessagePipe.GetPublisher<SelectCharaNameMessage>();


        returnNamePub = GlobalMessagePipe.GetPublisher<ReturnTargetName>();
        nextTargetSub = GlobalMessagePipe.GetSubscriber<sbyte, GetNextTargetName>();
        nextTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetNextTargetName>();
        preTargetSub = GlobalMessagePipe.GetSubscriber<sbyte, GetPreTargetName>();
        preTargetPub = GlobalMessagePipe.GetPublisher<sbyte, GetPreTargetName>();

        //tauntTargetSub = GlobalMessagePipe.GetSubscriber<GetTauntUserName>();

        //disposableBook
        //�g�p�X�L���̑I��
        //PrepareBattleSub=> selectStartSub
        moveKeyBookSub = GlobalMessagePipe.GetSubscriber<BookCommonMoveKeyMessage>();
        moveTargetBookSub = GlobalMessagePipe.GetSubscriber<BookCommonMoveTargetMessage>();
        activeKeyBookSub = GlobalMessagePipe.GetSubscriber<BookCommonActiveKeyMessage>();
        activeTargetBookSub = GlobalMessagePipe.GetSubscriber<BookCommonActiveTargetMessage>();

        commonActiveBookSub = GlobalMessagePipe.GetSubscriber<CommonActiveSkillTiming>();

        bookCompSub = GlobalMessagePipe.GetSubscriber<BookCompleteMessage>();

        //disposableActiveAttention
        //�ʏ�^�C�~���O�̃A�N�e�B�u�X�L���̎g�p
        //PrepareBattleSub=> selectStartSub => commonActiveBookSub
        activeBootSub = GlobalMessagePipe.GetSubscriber<sbyte, ActiveSkillBootMessage>();
        activeCommandPub = GlobalMessagePipe.GetPublisher<ActiveSkillCommand>();

        //disposableOnExclusion(�퓬�Q��)
        //�ړ��X�L���̎g�p
        //PrepareBattleSub
        moveBootSub = GlobalMessagePipe.GetSubscriber<sbyte, MoveSkillBootMessage>();
        moveCommandPub = GlobalMessagePipe.GetPublisher<MoveSkillCommand>();

        commonGetAgiASub = GlobalMessagePipe.GetAsyncSubscriber<GetCommonActionAgility>();
        returnAgiPub = GlobalMessagePipe.GetPublisher<ReturnAgilityMessage>();



        //�L�����N�^�[�̕Ґ�����
        formCharaSub.Subscribe(GetFormNum(), i =>
        {
            setChara = i.charaData;

        }).AddTo(bag);

        //�Ґ�������Ă��邩�̊m�F
        formPreASub.Subscribe(async (get, ct) =>
        {
            if (setChara != null)
            {
                participant = true;
                await PrepareBattleSub(ct);

                //������
                currentPos = firstPos;
                currentHP = setChara.GetMaxHP();

                if(currentPos == MovePosition.front)
                {
                    PrepareOnFront();
                }
                else
                {
                    PrepareOnBack();
                }
            }
            else
            {
                participant = false;
                PrepareWatchingSub();
            }
        }).AddTo(bag);

        //�X�L���̃��X�g���������񃊃Z�b�g���č\�z���Ȃ����B
        registStartASub.Subscribe(GetFormNum(), async (get,ct) =>
        {
            skillListHolderSO.aSkillCatalog.Clear();
            skillListHolderSO.mFrontSkillCatalog.Clear();
            skillListHolderSO.mBackSkillCatalog.Clear();
            if (setChara == null)
            {
                return;
            }
            var unregistPub = GlobalMessagePipe.GetPublisher<sbyte, UnregistPassiveSkill>();
            unregistPub.Publish(GetFormNum(), new UnregistPassiveSkill());
            setChara.RegistMasterySkill(GetFormNum());
        }).AddTo(bag);

        //�X�L�����X�g�̍X�V
        registASkillSub.Subscribe(GetFormNum(), get =>
        {
            skillListHolderSO.aSkillCatalog.Add(get.activeSkill);
        }).AddTo(bag);
        registFrontSub.Subscribe(GetFormNum(), get =>
        {
            skillListHolderSO.mFrontSkillCatalog.Add(get.moveSkill);

        }).AddTo(bag);
        registBackSub.Subscribe(GetFormNum(), get =>
        {
            skillListHolderSO.mBackSkillCatalog.Add(get.moveSkill);

        }).AddTo(bag);
        //


        endSub.Subscribe(get =>
        {
            //�퓬�s�\�ɂȂ������ɂ��R�Â���
            participant = false;

            //�퓬�I�����ɐ퓬�Ɋւ���Sub��S�ĊJ��
            disposableOnExclusion?.Dispose();
            disposableBook?.Dispose();
        }).AddTo(bag);

        disposable = bag.Build();
    }

    //�o�g�����ɎQ�����Ă���ꍇ��Sub�B
    private async UniTask PrepareBattleSub(CancellationToken thisCt)
    {
        disposableOnExclusion?.Dispose();
        disposableMove?.Dispose();
        

        var bag = DisposableBag.CreateBuilder();

        //�o�g���J�n���̏����i���l�̏������CImage�̕ύX�j
        //�^�C�~���O�O�サ�Ăق����Ȃ��̂ňڐA
        /*
        prepareASub.Subscribe(async (get, ct) =>
        {

            //Image�̓ǂݍ��ݖ���
            //changeImagePub.Publish(GetFormNum(), new ChangeBattleImage());
            //�O�q��q�̒ʒm������UI�̕\��
            //positionChangePub.Publish(new MovePositionChangeMessage(currentPos, GetFormNum()));

        }).AddTo(bag);
        */

        nextTargetSub.Subscribe(GetFormNum(), get =>
        {
            //Debug.Log(this.name);
            returnNamePub.Publish(new ReturnTargetName(setChara.GetCharaName(), GetFormNum()));
        }).AddTo(bag);

        preTargetSub.Subscribe(GetFormNum(), get =>
        {
            returnNamePub.Publish(new ReturnTargetName(setChara.GetCharaName(), GetFormNum()));
        }).AddTo(bag);

        selectStartSub.Subscribe(GetFormNum(), get =>
        {
            //�I�������̃R���|�[�l���g���玟��ActionSelectStartMessage�𑗐M����B
            //cancel�̃^�C�~���O����CselectNameChangePub�̕���selectListChangePub����
            selectNameChangePub.Publish(new SelectCharaNameMessage(setChara.GetCharaName()));

            selectListChangePub.Publish(new SelectSkillListChangeMessage(skillListHolderSO, currentPos));

            var bagB = DisposableBag.CreateBuilder();

            /*
            tauntTargetSub.Subscribe(get =>
            {
                returnNamePub.Publish(new ReturnTargetName(setChara.GetCharaName(), GetFormNum()));
            }).AddTo(bagB);
            */

            //BookSkill�̃p�����[�^�e��
            moveKeyBookSub.Subscribe(get =>
            {
                moveKey = get.skillNum;
                //Debug.Log(activeKey);
                //�����ɑΏەύX�p��Sub�Ȃ�

            }).AddTo(bagB);
            moveTargetBookSub.Subscribe(get =>
            {

                moveTarget = get.targetNum;

                //Debug.Log(moveTarget);

            }).AddTo(bagB);
            activeKeyBookSub.Subscribe(get =>
            {
                activeKey = get.skillNum;
                //Debug.Log(activeKey);

            }).AddTo(bagB);
            activeTargetBookSub.Subscribe(get =>
            {
                activeTarget = get.targetNum;
                //Debug.Log(activeTarget);
            }).AddTo(bagB);

            bookCompSub.Subscribe(async get =>
            {
                disposableBook?.Dispose();

                //Pub���I���O�Ɏ���Sub���n�܂��Ă��܂�
                await UniTask.NextFrame();

                selectStartPub.Publish(NextForm(GetFormNum()), new ActionSelectStartMessage(NextForm(GetFormNum())));

            }).AddTo(bagB);

            selectCancelSub.Subscribe(get =>
            {
                if(GetFormNum() == FormationScope.FirstChara())
                {
                    return;
                }
                disposableBook?.Dispose();

                UniTask.NextFrame();
                selectStartPub.Publish(PreForm(GetFormNum()), new ActionSelectStartMessage(SbyteHandler.PreForm(GetFormNum())));

            }).AddTo(bagB);

            //�X�L���̔����^�C�~���O��
            //CommonActiveSkillTiming
            commonActiveBookSub.Subscribe(get =>
            {
                var bagA = DisposableBag.CreateBuilder();
                commonGetAgiASub.Subscribe(async (get, ct) =>
                {
                    //Debug.Log(GetFormNum());
                    returnAgiPub.Publish(new ReturnAgilityMessage(GetActualAgility(), GetFormNum()));
                    //disposableActiveAttention.Dispose();
                }).AddTo(bagA);

                activeBootSub.Subscribe(GetFormNum(), get =>
                {
                    Debug.Log(name);
                    activeCommandPub.Publish(new ActiveSkillCommand(activeKey, activeTarget, this));
                    disposableActiveAttention.Dispose();
                }).AddTo(bagA);

                disposableActiveAttention = bagA.Build();

            }).AddTo(bagB);

            //BookSkill��


            //cancel���s�����ꍇ�������Dispose
            disposableBook = bagB.Build();

        }).AddTo(bag);

        enemyTargetGetASub.Subscribe(async(get,ct) =>
        {
            enemyTargetReturnPub.Publish(new EnemyTargetReturn(GetFormNum(), GetTargetRate()));
        }).AddTo(bag);

        moveBootSub.Subscribe(GetFormNum(), get =>
        {
            moveCommandPub.Publish(new MoveSkillCommand(moveKey, moveTarget, GetFormNum()));
        }).AddTo(bag);


        //�_���[�W�̈З͎󂯓���
        ActionSkillSubPrepare(GetFormNum(), bag);


        var checkGetSub = GlobalMessagePipe.GetSubscriber<KnockOutChara>();
        checkGetSub.Subscribe(get =>
        {
            var checkReturnPub = GlobalMessagePipe.GetPublisher<KnockOutChecker>();
            checkReturnPub.Publish(new KnockOutChecker(GetFormNum()));
           // Debug.Log("checked");
        }).AddTo(bag);

        disposableOnExclusion = bag.Build();

    }

    private void PrepareWatchingSub()
    {
        disposableOnExclusion?.Dispose();
        disposableMove?.Dispose();

        var bag = DisposableBag.CreateBuilder();
        nextTargetSub.Subscribe(GetFormNum(), get =>
        {
            //Debug.Log("pre"+this.name);
            nextTargetPub.Publish(NextForm(GetFormNum()), new GetNextTargetName());
        }).AddTo(bag);

        preTargetSub.Subscribe(GetFormNum(), get =>
        {
            preTargetPub.Publish(PreForm(GetFormNum()), new GetPreTargetName());
        }).AddTo(bag);

        //BattleSceneCommander����form=1��Pub���s���Ă���B
        //���Ԃɗ����Ă����O��őg�񂾂̂ŁCSub�������ƍ���B
        selectStartSub.Subscribe(GetFormNum(), get =>
        {
            selectStartPub.Publish(NextForm(GetFormNum()), new ActionSelectStartMessage(NextForm(GetFormNum())));

        }).AddTo(bag);

        disposableOnExclusion = bag.Build();

    }

    private void PrepareOnFront()
    {
        disposableMove?.Dispose();

        var bag = DisposableBag.CreateBuilder();

        var moveSub = GlobalMessagePipe.GetSubscriber<sbyte, NormalMoveToBack>();

        moveSub.Subscribe(GetFormNum(), info =>
        {
            currentPos = MovePosition.back;
            PrepareOnBack();
            var posChangePub = GlobalMessagePipe.GetPublisher<MovePositionChangeMessage>();
            posChangePub.Publish(new MovePositionChangeMessage(currentPos, GetFormNum()));
        }).AddTo(bag);


        ActionSkillSubPrepare(FormationScope.FrontChara(), bag);


        disposableMove = bag.Build();


    }
    private void PrepareOnBack()
    {
        disposableMove?.Dispose();

        var bag = DisposableBag.CreateBuilder();

        var moveSub = GlobalMessagePipe.GetSubscriber<sbyte, NormalMoveToFront>();

        moveSub.Subscribe(GetFormNum(), info =>
        {
            currentPos = MovePosition.front;
            PrepareOnFront();
            var posChangePub = GlobalMessagePipe.GetPublisher<MovePositionChangeMessage>();
            posChangePub.Publish(new MovePositionChangeMessage(currentPos, GetFormNum()));
        }).AddTo(bag);


        ActionSkillSubPrepare(FormationScope.BackChara(), bag);


        disposableMove = bag.Build();
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


    public void ActionSkillSubPrepare(sbyte form, DisposableBagBuilder bag)
    {
        normalDamageSub.Subscribe(form, get =>
        {
            int damage = NormalPDamage(get.damage);
            currentHP -= damage;
            damageNoticePub.Publish(new DamageNoticeMessage(true, GetFormNum(), damage));


            CheckContinuation();

        }).AddTo(bag);
        normalMagicDamageSub.Subscribe(form, get =>
        {
            int damage = NormalMDamage(get.damage);
            currentHP -= damage;
            damageNoticePub.Publish(new DamageNoticeMessage(true, GetFormNum(), damage));
            CheckContinuation();

        }).AddTo(bag);
    }



    public string GetTargetName()
    {
        return setChara.GetCharaName();
    }

    public int GetActualAttack()
    {
        return Mathf.FloorToInt(setChara.GetAttack() * effects.attackBuff.value);
    }
    
    public int GetActualDefence()
    {
        return setChara.GetDefence();
            //Mathf.FloorToInt(setChara.GetDefence() * effects.attackBuff.value);
    }
    
    public int GetActualMagic()
    {
        return Mathf.FloorToInt(setChara.GetAttack() * effects.magicBuff.value);
    }
    public int GetActualMagicDefence()
    {
        return setChara.GetMagicDefence();
            //Mathf.FloorToInt(setChara.GetAttack() * effects.magicBuff.value);
    }
    public int GetActualAgility()
    {
        return Mathf.FloorToInt(setChara.GetAgility() * effects.agiBuff.value);
    }

    public int GetTargetRate()
    {
        return setChara.GetTargetRate()  * Random.Range(1, 10);
    }

    public float GetRatioOnHP()
    {
        float ratio = (float)currentHP / (float)setChara.GetMaxHP();
        //Debug.Log();
        return ratio;
    }

    public void MoveToFront()
    {
        currentPos = MovePosition.front;
        positionChangePub.Publish(new MovePositionChangeMessage(currentPos, GetFormNum()));
    }
    public void MoveToBack()
    {
        currentPos = MovePosition.back;
        positionChangePub.Publish(new MovePositionChangeMessage(currentPos, GetFormNum()));
    }



    private void CheckContinuation()
    {
        if (currentHP < 0)
        {
            participant = false;
            PrepareWatchingSub();


            //���ꂪ�Ȃ���Γ|�ꂽ�L�������J�E���g�����
            UniTask.NextFrame();

            var dropCharaPub = GlobalMessagePipe.GetPublisher<DropCharaMessage>();
            dropCharaPub.Publish(new DropCharaMessage(GetFormNum()));
        }

    }
}
