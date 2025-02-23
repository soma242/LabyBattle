namespace SkillStruct
{

    /// <summary>
    /// �X�L�����e�L�����̎��X�L���c���[���璍��
    /// �Ґ��C�X�L���C�������^�C�~���O�ōs��
    /// From_(�e��ނ�)SkillHolder, To_FormationChara
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
    //�O�q�Ŏg�p����ړ��X�L��
    public class RegistMoveSkillOnFront
    {
        public MoveSkillOnFrontHolderSO moveSkill;
        public RegistMoveSkillOnFront(MoveSkillOnFrontHolderSO moveSkill)
        {
            this.moveSkill = moveSkill;
        }
    }
    //��q�Ŏg�p�ł���ړ��X�L��
    public class RegistMoveSkillOnBack
    {
        public MoveSkillOnBackHolderSO moveSkill;
        public RegistMoveSkillOnBack(MoveSkillOnBackHolderSO moveSkill)
        {
            this.moveSkill = moveSkill;
        }
    }

    /// <summary>
    /// From_FormationChara, To_BattleOption�̃R���|�[�l���g
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

    //���ʃX�L���̓o�^�J�n
    //From_CharacterDataSO�C To_CommonSkillTree
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
    /// �X�L���̃^�[�Q�b�g
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
    /// �X�L���g�p
    /// �e�R���|�[�l���g����X�L���R�}���_�[��
    /// </summary>

    //ActiveSkillCommander�ւ̃��b�Z�[�W
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

    //DamageCalc����̃��b�Z�[�W
    public struct NormalDamageCalcMessage{}

    //�eActiveSkillMSO����eSoldier�ւ̃��b�Z�[�W
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
//�I�������L�����N�^�[��I�������Ґ��ʒu�ɕҐ�
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

//�Ґ���ύX�łȂ������������̃��b�Z�[�W
//public struct RemoveFormCharacterMessage { }

//To_FormationCharaSO, From_CharacterDataSO�i�j
//�w��̃L�����N�^�[�f�[�^�ւ̎Q�Ƃ�n��
public class FormCharacterMessage
{
    public MSO_CharacterDataSO charaData;

    public FormCharacterMessage(MSO_CharacterDataSO charaData)
    {
        this.charaData = charaData;
    }
}

