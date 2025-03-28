using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

using BattleSceneMessage;
using SkillStruct;

public struct ReturnAgilityMessage
{
    public int agility;
    public sbyte formNum;
    public ReturnAgilityMessage(int agility, sbyte formNum)
    {
        this.agility = agility;
        this.formNum = formNum;
    }
}

public struct GetCommonActionAgility { }


//publish�̏W��

public class BattleSceneCommand : MonoBehaviour
{
    /*

    //[Inject] private readonly ISubscriber<BattleStartMessage> startSubscriber;
    [Inject] private readonly IPublisher<BattleOptionSelectStart> optionSelectStartPublisher;


    [Inject] private readonly IPublisher<BattleOptionSelecting> optionSelectihgPublisher;
    [Inject] private readonly ISubscriber<BattleOptionSelected> optionSelectedSubscriber;

    [Inject] private readonly IPublisher<BattleProcessStart> processStart;
    */

    private CancellationTokenSource cts;


    private bool battleContinue;

    private sbyte attention;
    private int standard;

    private ISubscriber<ReturnAgilityMessage> returnAgiSub;
    private IAsyncPublisher<GetCommonActionAgility> commonGetAgiAPub;

    private IPublisher<sbyte, ActiveSkillBootMessage> activeBootPub;
    private IPublisher<sbyte, MoveSkillBootMessage> moveBootPub;



    //
    private ISubscriber<BattleStartMessage> startSub;
    private IAsyncPublisher<FormationPrepareMessage> formPreAPub;
    private IAsyncPublisher<BattlePrepareMessage> prepareAPub;
    private IPublisher<BattleFinishMessage> endPub;
    private ISubscriber<BattleFinishMessage> endSub;

    private IAsyncPublisher<TurnStartMessage> turnStartAPub;
    private IAsyncPublisher<EnemyActionSetMessage> enemyActionSetAPub;

    private IPublisher<sbyte, ActionSelectStartMessage> selectStartPub;
    private ISubscriber<sbyte, ActionSelectStartMessage> lastSelectStartSub;
    private IAsyncPublisher<ActionSelectEndMessage> selectEndAPub;
    private IAsyncSubscriber<ActionSelectEndMessage> selectEndASub;


    private IAsyncPublisher<TurnEndMessage> turnEndAPub;

    private System.IDisposable disposable;


    void Awake()
    {
        returnAgiSub = GlobalMessagePipe.GetSubscriber<ReturnAgilityMessage>();
        commonGetAgiAPub = GlobalMessagePipe.GetAsyncPublisher<GetCommonActionAgility>();

        activeBootPub = GlobalMessagePipe.GetPublisher<sbyte, ActiveSkillBootMessage>();
        moveBootPub = GlobalMessagePipe.GetPublisher<sbyte, MoveSkillBootMessage>();

        //�^�[��������
        cts = new CancellationTokenSource();
        startSub = GlobalMessagePipe.GetSubscriber<BattleStartMessage>();
        formPreAPub = GlobalMessagePipe.GetAsyncPublisher<FormationPrepareMessage>();
        prepareAPub = GlobalMessagePipe.GetAsyncPublisher<BattlePrepareMessage>();

        //�܂��������ĂȂ��B
        endPub = GlobalMessagePipe.GetPublisher<BattleFinishMessage>();
        endSub = GlobalMessagePipe.GetSubscriber<BattleFinishMessage>();

        turnStartAPub = GlobalMessagePipe.GetAsyncPublisher<TurnStartMessage>();
        enemyActionSetAPub = GlobalMessagePipe.GetAsyncPublisher<EnemyActionSetMessage>();

        selectStartPub = GlobalMessagePipe.GetPublisher<sbyte, ActionSelectStartMessage>();
        lastSelectStartSub = GlobalMessagePipe.GetSubscriber<sbyte, ActionSelectStartMessage>();

        selectEndAPub = GlobalMessagePipe.GetAsyncPublisher<ActionSelectEndMessage>();
        selectEndASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectEndMessage>();



        turnEndAPub = GlobalMessagePipe.GetAsyncPublisher<TurnEndMessage>();

        


        var bag = DisposableBag.CreateBuilder();
        //BattleStart���󂯎���āC�R�}���h���J�n����
        startSub.Subscribe(get =>
        {
            try
            {
                BattleCommand(cts.Token);

            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == cts.Token)
            {
#if UNITY_EDITOR
                        if (cts.IsCancellationRequested)
                        {
                            // ������CancellationToken�������Ȃ̂ŁA�����ێ�����OperationCanceledException�Ƃ��ē�����
                            throw new OperationCanceledException(ex.Message, ex, cts.Token);
                        }
                        else
                        {
                            // �^�C���A�E�g�������Ȃ̂ŁATimeoutException(�����͓Ǝ��̗�O)�Ƃ��ē�����
                            throw new TimeoutException("The request was canceled due to the configured Timeout ");
                        }
#endif
            }
            
        }).AddTo(bag);

        //�������̍s���I�����I��������󂯎��C���̃X�e�b�v�ɐi��
        lastSelectStartSub.Subscribe(SbyteHandler.NextForm(FormationScope.LastChara()), get =>
        {
            selectEndAPub.PublishAsync(new ActionSelectEndMessage());
        }).AddTo(bag);

        var allDownCharaSub = GlobalMessagePipe.GetSubscriber<AllCharaDownMessage>();
        var allDownEnemySub = GlobalMessagePipe.GetSubscriber<AllEnemyDownMessage>();

        allDownCharaSub.Subscribe(get =>
        {
            endPub.Publish(new BattleFinishMessage());
        }).AddTo(bag);
        allDownEnemySub.Subscribe(get =>
        {
            endPub.Publish(new BattleFinishMessage());

        }).AddTo(bag);

        endSub.Subscribe(get =>
        {
            cts.Cancel();
            disposable.Dispose();
            Debug.Log("battleFinish");
        }).AddTo(bag);


        disposable = bag.Build();
    }



    private async UniTask BattleCommand(CancellationToken ct)
    {
        //�L�����ƓG��Formation�����ꂼ��Ґ�����Ă��邩�m�F
        await formPreAPub.PublishAsync(new FormationPrepareMessage());



        //�o�g���J�n���̏����i�����l�ɍX�V�Ȃǁj
        await prepareAPub.PublishAsync(new BattlePrepareMessage());

        //�o�g�����ɔ�������p�b�V�u�X�L����Sub�J�n
        var passiveStartPub = GlobalMessagePipe.GetPublisher<PassiveOnBattleStartMessage>();
        passiveStartPub.Publish(new PassiveOnBattleStartMessage());

        battleContinue = true;

        //�ȍ~�J��Ԃ������B
        do
        {
            //�^�[���J�n
            await turnStartAPub.PublishAsync(new TurnStartMessage());
            //�G�l�~�[�̍s������
            await enemyActionSetAPub.PublishAsync(new EnemyActionSetMessage(), AsyncPublishStrategy.Sequential);


            //�s���I���҂̕ύX
            selectStartPub.Publish(FormationScope.FirstChara(), new ActionSelectStartMessage(FormationScope.FirstChara()));

            //���x���N�����邱�Ƃ͊m���߂��B
            //Destory����Error: NullReferenceException: Object reference not set to an instance of an object
            //GetAsyncSub���Y��(�������ĂȂ�����)
            var ev = await selectEndASub.FirstAsync(ct);

            //�ړ��X�L����Ґ�����
            for (sbyte i = FormationScope.FirstChara(); i <= FormationScope.LastChara(); i++)
            {
                moveBootPub.Publish(i, new MoveSkillBootMessage());
                //Debug.Log("move");
                await UniTask.NextFrame();
            }

            //Active�X�L����f��������
            do
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: cts.Token);

                standard = 0;
                attention = FormationScope.NoneChara();
                var d = returnAgiSub.Subscribe(get =>
                {
                    if (get.agility > standard)
                    {
                        attention = get.formNum;
                        standard = get.agility;
                    }
                });
                await commonGetAgiAPub.PublishAsync(new GetCommonActionAgility());
                d.Dispose();

                activeBootPub.Publish(attention, new ActiveSkillBootMessage());


            } while (attention != FormationScope.NoneChara());

            //Debug.Log(this.name);


            //�^�[���I��
            await turnEndAPub.PublishAsync(new TurnEndMessage());
        } while (battleContinue);

    }


    






    private void OnDestroy()
    { 
        cts.Cancel();

        disposable.Dispose();
    }


}
