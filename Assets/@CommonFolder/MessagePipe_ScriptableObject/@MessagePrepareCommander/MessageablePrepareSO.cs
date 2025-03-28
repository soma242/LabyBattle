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

        builder.AddMessagePipe(/* configure option */);

        // AddMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.

        //Common
        //builder.AddMessageBroker<SkillEnumKey, EntrySkillMessage>();

        //To_Formation, From_CharacterData
        builder.AddMessageBroker<sbyte, FormCharacterMessage>();

        //ToCharacterCommander
        builder.AddMessageBroker<FormCharacterBootMessage>();

        //builder.AddMessageBroker<RemoveFormCharacterMessage>();

        //To_ActiveSkillCommander
        builder.AddMessageBroker<ActiveSkillCommand>();

        //To_skillSoldier
        builder.AddMessageBroker<NormalAttack>();
        builder.AddMessageBroker<NormalMagic>();
        
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
        builder.AddMessageBroker<RegistCommonSkill>();
        builder.AddMessageBroker<RegistCommonPhysicalSkill>();
        builder.AddMessageBroker<RegistCommonMagicSkill>();

        builder.AddMessageBroker<sbyte, RegistSkillStart>();
        builder.AddMessageBroker<RegistSkillFinish>();


        builder.AddMessageBroker<sbyte, UnregistPassiveSkill>();
        builder.AddMessageBroker<PassiveOnBattleStartMessage>();



        builder.AddMessageBroker<sbyte, BreakePostureMessage>();




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

        //�I�������X�L���ƃ^�[�Q�b�g��\��I��
        //From_SelectFinishController, To_FormationChara
        builder.AddMessageBroker<BattleSceneMessage.ActionSelectBookMessage>();
        builder.AddMessageBroker<BattleSceneMessage.BookCompleteMessage>();
        builder.AddMessageBroker<BattleSceneMessage.ActionSelectEndMessage>();

        builder.AddMessageBroker<BattleSceneMessage.TurnEndMessage>();

        builder.AddMessageBroker<BattleSceneMessage.CharaAgilitySetMessage>();


        builder.AddMessageBroker<BattleSceneMessage.InputLayer>();

        //builder.AddMessageBroker<BattleSceneMessage.InputLayerChanged>();
        builder.AddMessageBroker<InputLayerSO, BattleSceneMessage.InputLayerChanged>();

        builder.AddMessageBroker<BattleSceneMessage.SelectChange>();
        builder.AddMessageBroker<BattleSceneMessage.SelectMessage, BattleSceneMessage.SelectChange>();
        builder.AddMessageBroker<BattleSceneMessage.InputLayerChanged>();

        builder.AddMessageBroker<BattleSceneMessage.NewLogSizeMessage>();
        builder.AddMessageBroker<BattleSceneMessage.DisableLogMessage>();
        builder.AddMessageBroker<BattleSceneMessage.DamageNoticeMessage>();

        //zzzTester
        /*
        builder.AddMessageBroker<SortMessage>();
        builder.AddMessageBroker<SortValueMessage>();
        
        builder.AddMessageBroker<TestFA>();
        */

        // also exists AddMessageBroker<TKey, TMessage>, AddRequestHandler, AddAsyncRequestHandler

        // AddMessageHandlerFilter: Register for filter, also exists RegisterAsyncMessageHandlerFilter, Register(Async)RequestHandlerFilter
        //builder.AddMessageHandlerFilter<MyFilter<BattleStart>>();

        // create provider and set to Global(to enable diagnostics window and global fucntion)
        var provider = builder.BuildServiceProvider();
        GlobalMessagePipe.SetProvider(provider);
    }

}
