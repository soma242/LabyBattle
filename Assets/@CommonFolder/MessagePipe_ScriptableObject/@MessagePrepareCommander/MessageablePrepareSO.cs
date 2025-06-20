using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using MessagePipe;


using SkillStruct;

//using MessageableManager;

//������i���邢�͎g�p����R���e�i���Ƃɕ����āj�p�ӂ���icontainer�̗p�ӂƈˑ��֌W�̒��������͌��܂��ĂȂ���΂Ȃ�Ȃ��j

/////�菇
//MessageablePrepareSO�ւ�MessageableInjectListSO�̒ǉ��ƃA�^�b�`
//�Ή�����MessageableInjectListSO�֑S�Ă�MessageableScriptableObject�̃A�^�b�`



[CreateAssetMenu(menuName = "MessageableSO/Important/���ЂƂ����p��MessageablePrepareSO")]
public class MessageablePrepareSO : ScriptableObject
{

    //[SerializeField] private MessageableInjectListSO activeSkillSO;
    [SerializeField] private List<MessageableInjectListSO> injectList = new List<MessageableInjectListSO>();



    public void OnEnable()
    {
        //�v���C���ԊO�ɓ��������m�F�p(OnEnable�̍쓮�^�C�~���O�m�F�ɂ�����)
        //Debug.Log("enableRun");
        //OnEnable�CAwake���ɁC�C���X�y�N�^��ł���SO���J���Ă��邩�V�[����ɎQ�Ƃ����Ȃ���Α���Ȃ������B
        //@zzz�ɃA�^�b�`����SetProviderReference����Q�Ƃ�����Ă���B
        //���ۂɎg�p����Ƃ��͍ŏ��ɋN������V�[���ɎQ�Ƃ���邱�ƁB
        //��x�����΂����̂ő��̃V�[������̎Q�Ƃ͏���

#if UNITY_EDITOR
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            //Debug.Log("Started");
#endif

            SetGlobalMessageProvider();


            //MessageableScriptableObject�̎�ނ��ƂɑΉ�����InjectList����s���B
            //activeSkillSO.MessageDependencyInjection();
            foreach (MessageableInjectListSO injecter in injectList)
            {
                injecter.MessageDependencyInjection();
            }
#if UNITY_EDITOR
    }
#endif
    }


    //BuiltinContainer�ɂ��MessagePipe�p�̃R���e�i�p��
    //Broker���R�����g�A�E�g���邱�ƂŁCPub��T����B�iStart�Ȃǂ����j
    public void SetGlobalMessageProvider()
    {
        var builder = new BuiltinContainerBuilder();

        //options =new MessagePipeOptions();
        //options.EnableCaptureStackTrace = true;

        //System.Action<MessagePipeOptions> action = delegate(options);


        //Action<>�������Ɏ���Ă���̂�Delegete��MessagePipeOptions��n���Ă���
        builder.AddMessagePipe(options =>
        {
            options.EnableCaptureStackTrace = true;
        });

        // AddMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.

        //Common
        //builder.AddMessageBroker<SkillEnumKey, EntrySkillMessage>();

        builder.AddMessageBroker<InputLayer>();


        //To_Formation, From_CharacterData
        builder.AddMessageBroker<sbyte, FormCharacterMessage>();

        //ToCharacterCommander
        builder.AddMessageBroker<FormCharacterBootMessage>();
        builder.AddMessageBroker<FormEnemyBootMessage>();

        //builder.AddMessageBroker<RemoveFormCharacterMessage>();

        //To_ActiveSkillCommander
        builder.AddMessageBroker<ActiveSkillCommand>();

        //To_skillSoldier
        builder.AddMessageBroker<NormalAttack>();
        builder.AddMessageBroker<NormalMagic>();
        builder.AddMessageBroker<sbyte, AttackBuffMessage>();

        
        //moveSkill�̎��s
        builder.AddMessageBroker<sbyte, NormalMoveToFront>();
        builder.AddMessageBroker<sbyte, NormalMoveToBack>();
        builder.AddMessageBroker<sbyte, TauntApproachMessage>();


        //From_DamageCalc
        builder.AddMessageBroker<sbyte, NormalDamageCalcMessage>();
        builder.AddMessageBroker<sbyte, NormalMagicDamageCalcMessage>();

        //To_MoveSKillCommander
        builder.AddMessageBroker<MoveSkillCommand>();


        //�X�L���o�^�C To_Formation
        //From_CharacterData, (Phisical,Magic)
        builder.AddMessageBroker<RegistCommonSkill>();
        builder.AddMessageBroker<RegistCommonPhysicalSkill>();
        builder.AddMessageBroker<RegistCommonMagicSkill>();

        builder.AddMessageBroker<sbyte, RegistSkillStart>();
        builder.AddMessageBroker<RegistSkillFinish>();


        builder.AddMessageBroker<sbyte, UnregistPassiveSkill>();
        builder.AddMessageBroker<PassiveOnBattleStartMessage>();



        builder.AddMessageBroker<sbyte, BreakePostureMessage>();
        builder.AddMessageBroker<BreakPostureSuccessEnemy>();




        //From_Formation, To_BreakePostureOnTaunt
        builder.AddMessageBroker<sbyte, TauntSuccessMessage>();


        builder.AddMessageBroker<SelectSkillListChangeMessage>();
        builder.AddMessageBroker<SelectCharaNameMessage>();

        //SkillTargetSort, From_SkillSO, To_TargetOption
        //chara
        builder.AddMessageBroker<Active_SingleCharaTarget>();
        builder.AddMessageBroker<Active_AllCharaTarget>();
        builder.AddMessageBroker<Active_FrontCharaTarget>();
        builder.AddMessageBroker<Active_BackCharaTarget>();

        builder.AddMessageBroker<Move_NoneTarget>();
        builder.AddMessageBroker<Move_SingleEnemyTarget>();

        //enemy
        builder.AddMessageBroker<Active_SingleEnemyTarget>();
        builder.AddMessageBroker<Active_AllEnemyTarget>();

        //enemy�̃^�[�Q�b�g�̎��
        builder.AddMessageBroker<EnemyTargetingAllFrontChara>();
        builder.AddMessageBroker<EnemyTargetingAllChara>();
        builder.AddMessageBroker<EnemyTargetingSingleChara>();

        //����simulate
        builder.AddMessageBroker<EnemyTargetFrontSimulate>();
        builder.AddMessageBroker<EnemyTargetSingleCharaSimulate>();




        builder.AddMessageBroker<sbyte, RegistActiveSkill>();
        builder.AddMessageBroker<sbyte, RegistMoveSkillOnFront>();
        builder.AddMessageBroker<sbyte, RegistMoveSkillOnBack>();


        //builder.AddMessageBroker<ChangeBattleImage>();
        builder.AddMessageBroker<MovePositionChangeMessage>();
        builder.AddMessageBroker<sbyte, SetEnemyImage>();

        builder.AddMessageBroker<sbyte, GetNextTargetName>();
        builder.AddMessageBroker<sbyte, GetPreTargetName>();

        //builder.AddMessageBroker<GetTauntUserName>();

        builder.AddMessageBroker<ReturnTargetName>();

        //�^�[�Q�b�g��l�����ꂼ��̃L��������擾����
        builder.AddMessageBroker<EnemyTargetGetMessage>();
        builder.AddMessageBroker<EnemyTargetReturn>();

        builder.AddMessageBroker<ReturnAgilityMessage>();
        builder.AddMessageBroker<GetCommonActionAgility>();
       


        //Simulator
        builder.AddMessageBroker<ASkillSimulateMessage>();

        builder.AddMessageBroker<MoveSimulateMessage>();

        builder.AddMessageBroker<MoveToBackSimulate>();
        builder.AddMessageBroker<MoveToFrontSimulate>();
        builder.AddMessageBroker<MoveWaitSimulate>();

        builder.AddMessageBroker<TauntSimulateCancell>();
        //From_MoveOption, To_EnemyActionSimu
        builder.AddMessageBroker<MoveOptionChange>();

        builder.AddMessageBroker<sbyte, TauntApproachSimulate>();

        //BookSkill
        builder.AddMessageBroker<BookCommonMoveKeyMessage>();
        builder.AddMessageBroker<BookCommonMoveTargetMessage>();
        builder.AddMessageBroker<BookCommonActiveKeyMessage>();
        builder.AddMessageBroker<BookCommonActiveTargetMessage>();


        builder.AddMessageBroker<CommonActiveSkillTiming>();



        //BootSkill
        //From_BattleSkillCommander, To_Formation
        builder.AddMessageBroker<sbyte, ActiveSkillBootMessage>();
        builder.AddMessageBroker<sbyte, MoveSkillBootMessage>();


        builder.AddMessageBroker<DescriptionDemendMessage>();
        builder.AddMessageBroker<DescriptionFinishMessage>();


        //BattleScene

        builder.AddMessageBroker<InputLayerSO, InputLayerChanged>();

        //To_BattleSceneCommand
        builder.AddMessageBroker<BattleSceneMessage.BattleStartMessage>();
        //From_BattleSceneCommand, To_Formation(Char,Enemy)
        builder.AddMessageBroker<BattleSceneMessage.FormationPrepareMessage>();
        //From_BattleSceneCommand, To_effect,  To_CharaNameSimulator, To_EnemySimulator,
        builder.AddMessageBroker<BattleSceneMessage.BattlePrepareMessage>();
        //From_BattleSceneCommand, To_MSO_FormationCharaSO
        builder.AddMessageBroker<BattleSceneMessage.BattleFinishMessage>();

        builder.AddMessageBroker<BattleSceneMessage.TurnStartMessage>();

        builder.AddMessageBroker<BattleSceneMessage.EnemyActionSetMessage>();

        builder.AddMessageBroker<BattleSceneMessage.DropEnemyMessage>();
        builder.AddMessageBroker<BattleSceneMessage.DropCharaMessage>();

        builder.AddMessageBroker<BattleSceneMessage.KnockOutEnemy>();
        builder.AddMessageBroker<BattleSceneMessage.KnockOutChara>();
        builder.AddMessageBroker<BattleSceneMessage.KnockOutChecker>();

        builder.AddMessageBroker<BattleSceneMessage.AllEnemyDownMessage>();
        builder.AddMessageBroker<BattleSceneMessage.AllCharaDownMessage>();

        //From_BattleSceneCommand, FormationChara,
        //To_FormationChara, ActionSelectSimulator
        builder.AddMessageBroker<sbyte, BattleSceneMessage.ActionSelectStartMessage>();
        builder.AddMessageBroker<BattleSceneMessage.ActionSelectCancelMessage>();
        builder.AddMessageBroker<sbyte, BattleSceneMessage.ActionSelectCancelPerfomeMessage>();

        builder.AddMessageBroker<BattleSceneMessage.AllSimulateRestMessage>();
        builder.AddMessageBroker<BattleSceneMessage.SimulateResetMessage>();

        //�I�������X�L���ƃ^�[�Q�b�g��\��I��
        //From_SelectFinishController, To_FormationChara
        builder.AddMessageBroker<BattleSceneMessage.ActionSelectBookMessage>();
        builder.AddMessageBroker<BattleSceneMessage.BookCompleteMessage>();
        builder.AddMessageBroker<BattleSceneMessage.ActionSelectEndMessage>();

        builder.AddMessageBroker<BattleSceneMessage.TurnEndMessage>();

        builder.AddMessageBroker<BattleSceneMessage.CharaAgilitySetMessage>();



        //builder.AddMessageBroker<BattleSceneMessage.InputLayerChanged>();
        //builder.AddMessageBroker<InputLayerSO, BattleSceneMessage.InputLayerChanged>();

        //builder.AddMessageBroker<BattleSceneMessage.SelectChange>();

        builder.AddMessageBroker<BattleSceneMessage.NewLogSizeMessage>();
        builder.AddMessageBroker<BattleSceneMessage.DisableLogMessage>();
        builder.AddMessageBroker<BattleSceneMessage.DamageNoticeMessage>();
        builder.AddMessageBroker<BattleSceneMessage.BuffNoticeMessage>();

        //�ڐA�iVContainer����j
        builder.AddMessageBroker<BattleSceneMessage.BattleOptionSelectStart>();

        builder.AddMessageBroker<BattleSceneMessage.BattleOptionSelecting>();
        builder.AddMessageBroker<BattleSceneMessage.BattleOptionSelected>();

        /*
        builder.AddMessageBroker<BattleSceneMessage.Holdout>();


        builder.AddMessageBroker<InputLayerSO, BattleSceneMessage.RightInput>();
        builder.AddMessageBroker<InputLayerSO, BattleSceneMessage.LeftInput>();
        builder.AddMessageBroker<InputLayerSO, BattleSceneMessage.UpInput>();
        builder.AddMessageBroker<InputLayerSO, BattleSceneMessage.DownInput>();
        builder.AddMessageBroker<InputLayerSO, BattleSceneMessage.EnterInput>();
        builder.AddMessageBroker<InputLayerSO, BattleSceneMessage.CancelInput>();
        builder.AddMessageBroker<InputLayerSO, BattleSceneMessage.ScrollInput>();
        */

        //builder.AddMessageBroker<BattleSceneMessage.DisposeSelect>();



        //dungeonScene






        builder.AddMessageBroker<DungeonMapMessage>();
        builder.AddMessageBroker<AdvanceMapMessage>();
        //To_DungeonDrawer
        builder.AddMessageBroker<DungeonSceneMessage.DungeonDrawMessage>();

        //From_DungeonDrawer
        builder.AddMessageBroker<DungeonSceneMessage.WallDrawMessage>();
        builder.AddMessageBroker<DungeonSceneMessage.ResetMapMessage>();
        builder.AddMessageBroker<DungeonPos, DungeonSceneMessage.ComponentCheckMessage>();

        //From_DungeonPositionHolderSO
        builder.AddMessageBroker<DungeonSceneMessage.MiniMapUpdateMessage>();
        builder.AddMessageBroker<DungeonSceneMessage.MiniMapResetMessage>();
        builder.AddMessageBroker<DungeonSceneMessage.PosChangeMessage>();
        builder.AddMessageBroker<DungeonSceneMessage.RotateDirectionMessage>();


        builder.AddMessageBroker<DungeonSceneMessage.MapRoadMessage>();

        builder.AddMessageBroker<DungeonSceneMessage.MapExpandMessage>();
        builder.AddMessageBroker<DungeonSceneMessage.MapCloseMessage>();

        //From_DungeonMapDataSO
        //To_MapHolderSO
        builder.AddMessageBroker<DungeonSceneMessage.ScreenValidMessage>();
        builder.AddMessageBroker<DungeonSceneMessage.ComponentValidMessage>();
        //To_BigMapScreenCont
        builder.AddMessageBroker<DungeonSceneMessage.ScreenDrawMessage>();
        //From_DungeonMapDataSO(ScreenDisposableAdmin)
        //To_DungeonMapDataSO, BigMapScreenCont
        builder.AddMessageBroker<DungeonSceneMessage.ScreenTrueMessage>();

        //From_comp, To_DungeonMapDataSO
        builder.AddMessageBroker<DungeonPos, DungeonSceneMessage.ComponentOverlapMessage>();


        builder.AddMessageBroker<DungeonSceneMessage.MapChangeMessage>();

        //From_DownStairs
        builder.AddMessageBroker<DungeonSceneMessage.DownStairsSetMessage>();
        builder.AddMessageBroker<DungeonSceneMessage.UpStairsSetMessage>();

        builder.AddMessageBroker<EncountGroup>();



        //StartScene
        builder.AddMessageBroker<StartSceneDisableMessage>();
        builder.AddMessageBroker<StartSceneEnableMessage>();
        builder.AddMessageBroker<CallBattleMessage>();
        builder.AddMessageBroker<EndBattleSceneMessage>();

        //From_InputSupporterMenu
        builder.AddMessageBroker<MenuToDungeonMessage>();
        //
        builder.AddMessageBroker<DungeonToMenuMessage>();


        builder.AddMessageBroker<ToTitleMessage>();

        //MenuScene
        builder.AddMessageBroker<MenuScene.MenuIPointerMessage>();
        builder.AddMessageBroker<MenuScene.MainToStatusMessage>();
        builder.AddMessageBroker<MenuScene.StatusToMainMessage>();
        builder.AddMessageBroker<MenuScene.StatusToSkillMessage>();
        builder.AddMessageBroker<MenuScene.SkillToStatusMessage>();


        builder.AddMessageBroker<MenuScene.MemberHolderClickMessage>();

        builder.AddMessageBroker<MenuScene.StatusMemberChangeMessage>();

        builder.AddMessageBroker<MenuScene.ResetSkillFieldMessage>();


        builder.AddMessageBroker<MenuScene.SelectNextNameHolder>();
        builder.AddMessageBroker<MenuScene.SelectPreNameHolder>();
        builder.AddMessageBroker<MenuScene.SelectNextTreeHolder>();
        builder.AddMessageBroker<MenuScene.SelectPreTreeHolder>();


        builder.AddMessageBroker<MenuScene.SkillDescriptionMessage>();



        //From_SceneController,
        //To_NewGameController, DungeonMapDataSO
        //�����I��Save����
        builder.AddMessageBroker<SaveCallMessage>();


        builder.AddMessageBroker<InputSystemSwitch>();
        builder.AddMessageBroker<NewGameMessage>();
        builder.AddMessageBroker<GameContinueMessage>();

        builder.AddMessageBroker<NewGameboolSaveTrigger>();


        builder.AddMessageBroker<sbyte, StartSceneMessage.SelectComp>();


        builder.AddMessageBroker<StartSceneMessage.YesCheckMessage>();
        builder.AddMessageBroker<StartSceneMessage.NoCheckMessage>();
        builder.AddMessageBroker<StartSceneMessage.StartSelectMessage>();

        //Input
        builder.AddMessageBroker<UpInput>();
        builder.AddMessageBroker<DownInput>();
        builder.AddMessageBroker<EnterInput>();
        builder.AddMessageBroker<Holdout>();

        //Input
        builder.AddMessageBroker<InputLayerSO, RightInput>();
        builder.AddMessageBroker<InputLayerSO, LeftInput>();
        builder.AddMessageBroker<InputLayerSO, UpInput>();
        builder.AddMessageBroker<InputLayerSO, DownInput>();
        builder.AddMessageBroker<InputLayerSO, EnterInput>();
        builder.AddMessageBroker<InputLayerSO, CancelInput>();
        builder.AddMessageBroker<InputLayerSO, MapInput>();
        builder.AddMessageBroker<InputLayerSO, MenuInput>();
        builder.AddMessageBroker<InputLayerSO, ScrollInput>();

        builder.AddMessageBroker<Holdout>();

        builder.AddMessageBroker<SelectMessage, SelectChange>();
        builder.AddMessageBroker<InputLayerSO, DisposeSelect>();


        //zzzTester
        /*
        builder.AddMessageBroker<SortMessage>();
        builder.AddMessageBroker<SortValueMessage>();
        
        builder.AddMessageBroker<TestFA>();
        */
        builder.AddMessageBroker<TestMessage>();

        // also exists AddMessageBroker<TKey, TMessage>, AddRequestHandler, AddAsyncRequestHandler

        // AddMessageHandlerFilter: Register for filter, also exists RegisterAsyncMessageHandlerFilter, Register(Async)RequestHandlerFilter
        //builder.AddMessageHandlerFilter<MyFilter<BattleStart>>();



        // create provider and set to Global(to enable diagnostics window and global fucntion)
        var provider = builder.BuildServiceProvider();

        GlobalMessagePipe.SetProvider(provider);
    }

}
