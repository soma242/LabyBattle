namespace SkillStruct
{

    /// <summary>
    /// スキルを各キャラの持つスキルツリーから注入
    /// 編成，スキル修得したタイミングで行う
    /// From_(各種類の)SkillHolder, To_FormationChara
    /// </summary>
    
    //ActiveSkill
    public class RegistActiveSkill
    {
        public ActiveSkillHolderSO activeSkill;
        public RegistActiveSkill(ActiveSkillHolderSO activeSkill)
        {
            this.activeSkill = activeSkill;
        }
    }
    //前衛で使用する移動スキル
    public class RegistMoveSkillOnFront
    {
        public MoveSkillOnFrontHolderSO moveSkill;
        public RegistMoveSkillOnFront(MoveSkillOnFrontHolderSO moveSkill)
        {
            this.moveSkill = moveSkill;
        }
    }
    //後衛で使用できる移動スキル
    public class RegistMoveSkillOnBack
    {
        public MoveSkillOnBackHolderSO moveSkill;
        public RegistMoveSkillOnBack(MoveSkillOnBackHolderSO moveSkill)
        {
            this.moveSkill = moveSkill;
        }
    }

    /// <summary>
    /// From_FormationChara, To_BattleOptionのコンポーネント
    /// </summary>
    public class SelectSkillListChangeMessage
    {
        public SkillListHolderSO skillListHolderSO;
        public MovePosition currentPos;
        public SelectSkillListChangeMessage(SkillListHolderSO skillListHolderSO, MovePosition currentPos)
        {
            this.skillListHolderSO = skillListHolderSO;
            this.currentPos = currentPos;
        }
    }

    //共通スキルの登録開始
    //From_CharacterDataSO， To_CommonSkillTree
    public struct RegistCommonSkill 
    {
        public sbyte formNum;
        public RegistCommonSkill(sbyte formNum)
        {
            this.formNum = formNum;
        }
    }

    public struct RegistSkillStart { }
    public struct RegistSkillFinish { }

    /// <summary>
    /// スキルのターゲット
    /// </summary>
    //chara
    public struct Active_SingleCharaTarget { }
    public struct Active_AllCharaTarget { }
    public struct Active_FrontCharaTarget { }
    public struct Active_BackCharaTarget { }

    //enemy
    public struct Active_SingleEnemyTarget { }
    public struct Active_AllEnemyTarget { }

    public class ActiveSkillPosition{

        public MSO_FormationCharaSO fromSO;
        public sbyte to;

        public ActiveSkillPosition(MSO_FormationCharaSO fromSO, sbyte to)
        {
            this.fromSO = fromSO;
            this.to = to;
        }
    }

    /// <summary>
    /// スキル使用
    /// 各コンポーネントからスキルコマンダーへ
    /// </summary>

    //ActiveSkillCommanderへのメッセージ
    public class ActiveSkillCommand
    {
        public int activeNum;

        public ActiveSkillPosition activePos;

        public ActiveSkillCommand(int activeNum, ActiveSkillPosition activePos)
        {
            this.activeNum = activeNum;

            this.activePos = activePos;
        }
    }

    //To_MoveSKillCommander
    public struct MoveSkillCommand
    {
        public int moveNum;
        public sbyte formNum;
        public MoveSkillCommand(int moveNum, sbyte formNum)
        {
            this.moveNum = moveNum;
            this.formNum = formNum;
        }
    }

    //To_MoveSKillCommander
    public struct TargetingMoveSkillCommand
    {
        public int moveNum;
        public sbyte formNum;
        public sbyte targetNum;
        public TargetingMoveSkillCommand(int moveNum, sbyte formNum, sbyte targetNum)
        {
            this.moveNum = moveNum;
            this.targetNum = targetNum;
            this.formNum = formNum;
        }
    }

    //DamageCalcからのメッセージ
    public struct NormalDamageCalcMessage{}

    //各ActiveSkillMSOから各Soldierへのメッセージ
    public struct NormalAttack {

        public ActiveSkillPosition activePos;

        public float activeRatio;

        public NormalAttack(ActiveSkillPosition activePos, float activeRatio)
        {
            this.activePos = activePos;
            this.activeRatio = activeRatio;
        }
    }


}

public class ChangeActionMessage
{
    public ActiveSkillHolderSO aSkillHolder;
}

public struct FormationChangeMessage
{

}

//To_CharacterCommander, From_Component
//選択したキャラクターを選択した編成位置に編成
public struct FormCharacterBootMessage
{
    public int charaNum;
    public sbyte formNum;

    public FormCharacterBootMessage(int charaNum, sbyte formNum)
    {
        this.charaNum = charaNum;
        this.formNum = formNum;
    }
}

//編成を変更でなく解除した時のメッセージ
//public struct RemoveFormCharacterMessage { }

//To_FormationCharaSO, From_CharacterDataSO（）
//指定のキャラクターデータへの参照を渡す
public class FormCharacterMessage
{
    public MSO_CharacterDataSO charaData;

    public FormCharacterMessage(MSO_CharacterDataSO charaData)
    {
        this.charaData = charaData;
    }
}

