using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
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
public class MSO_FormationCharaSO : MSO_FormationBaseMSO
{

    public MSO_CharacterDataSO setChara;

    [SerializeField]
    protected SkillListHolderSO skillListHolderSO;

    private MovePosition currentPos;
    private MovePosition firstPos;

    private int currentHP;

    private IAsyncSubscriber<sbyte, RegistSkillStart> registStartASub;

    //スキルリストの更新
    private ISubscriber<sbyte, RegistActiveSkill> registASkillSub;
    private ISubscriber<sbyte, RegistMoveSkillOnFront> registFrontSub;
    private ISubscriber<sbyte, RegistMoveSkillOnBack> registBackSub;

    //バトルの終了時処理
    private IAsyncSubscriber<BattleFinishMessage> prepareASub;

    //キャラクターの編成
    private ISubscriber<sbyte, FormCharacterMessage> formCharaSub;

    //セットされたキャラクターのバトル時の行動選択開始
    private ISubscriber<sbyte, ActionSelectStartMessage> selectStartSub;

    //行動選択

    private System.IDisposable disposable;

    private IPublisher<SelectSkillListChangeMessage> selectListChangePub;



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

        effects = new Effects();
        //attackBuff.SetValue(1.4f,3);

        registStartASub = GlobalMessagePipe.GetAsyncSubscriber<sbyte, RegistSkillStart>();

        registASkillSub = GlobalMessagePipe.GetSubscriber<sbyte, RegistActiveSkill>();
        registFrontSub = GlobalMessagePipe.GetSubscriber<sbyte, RegistMoveSkillOnFront>();
        registBackSub = GlobalMessagePipe.GetSubscriber<sbyte, RegistMoveSkillOnBack>();


        prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattleFinishMessage>(); 

        formCharaSub = GlobalMessagePipe.GetSubscriber<sbyte, FormCharacterMessage>();
        selectStartSub = GlobalMessagePipe.GetSubscriber<sbyte, ActionSelectStartMessage>();

        selectListChangePub = GlobalMessagePipe.GetPublisher<SelectSkillListChangeMessage>();

        registStartASub.Subscribe(formNumSO.formationNum, async (get,ct) =>
        {
            skillListHolderSO.aSkillCatalog.Clear();
            skillListHolderSO.mFrontSkillCatalog.Clear();
            skillListHolderSO.mBackSkillCatalog.Clear();
            setChara.RegistMasterySkill(formNumSO.formationNum);
        }).AddTo(bag);

        //スキルリストの更新
        registASkillSub.Subscribe(formNumSO.formationNum, get =>
        {
            skillListHolderSO.aSkillCatalog.Add(get.activeSkill);
        }).AddTo(bag);
        registFrontSub.Subscribe(formNumSO.formationNum, get =>
        {
            skillListHolderSO.mFrontSkillCatalog.Add(get.moveSkill);

        }).AddTo(bag);
        registBackSub.Subscribe(formNumSO.formationNum, get =>
        {
            skillListHolderSO.mBackSkillCatalog.Add(get.moveSkill);

        }).AddTo(bag);
        //


        prepareASub.Subscribe(async (get, ct) => 
        {
            currentPos = firstPos;
            currentHP = setChara.GetMaxHP();
        }).AddTo(bag);

        formCharaSub.Subscribe(formNumSO.formationNum, i =>
        {
            setChara = i.charaData;

        }).AddTo(bag);

        selectStartSub.Subscribe(formNumSO.formationNum, get =>
        {
            //currentPos = MovePosition.back;
            //Debug.Log(currentPos);
            selectListChangePub.Publish(new SelectSkillListChangeMessage(skillListHolderSO, currentPos));
        }).AddTo(bag);

        disposable = bag.Build();
    }



    


    public int GetActualAttack()
    {
        return Mathf.FloorToInt(setChara.GetAttack() * effects.attackBuff.value);
    }
    public int GetActualAgility()
    {
        return Mathf.FloorToInt(setChara.GetAgility() * effects.agiBuff.value);
    }


}
