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
        public MSO_ActiveSkillHolderSO activeSkill;
        public RegistActiveSkill(MSO_ActiveSkillHolderSO activeSkill)
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

    //PassiveSkill��Sub�J�n��ʒm����
    public struct PassiveOnBattleStartMessage { }

    public struct UnregistPassiveSkill { }

    
    //�p�b�V�u�X�L���̔����^�C�~���O�Ȃǂ�Sub
    public struct TauntSuccessMessage 
    {
        public sbyte target;

        public TauntSuccessMessage(sbyte target)
        {
            this.target = target;
        }
    }
    
    public struct BreakePostureMessage { }
    
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

    public struct RegistCommonPhysicalSkill 
    {
        public sbyte formNum;
        public RegistCommonPhysicalSkill(sbyte formNum)
        {
            this.formNum = formNum;
        }
    }    

    public struct RegistCommonMagicSkill
    {
        public sbyte formNum;
        public RegistCommonMagicSkill(sbyte formNum)
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

    //from_MoveTarget, To_comp(MoveTargetCont)
    public struct Move_NoneTarget { }
    public struct Move_SingleEnemyTarget { }

    //enemy
    public struct Active_SingleEnemyTarget { }
    public struct Active_AllEnemyTarget { }



    public class ActiveSkillPosition{

        public IGetFormationInfo userSO;
        public sbyte target;

        public ActiveSkillPosition(sbyte target, IGetFormationInfo userSO)
        {
            this.userSO = userSO;
            this.target = target;
        }
    }

    public struct MoveSkillPosition
    {
        public sbyte target;
        public sbyte user;

        public MoveSkillPosition(sbyte target, sbyte user)
        {
            this.target = target;
            this.user = user;
        }
    }

    /// <summary>
    /// �X�L���g�p
    /// �e�R���|�[�l���g����X�L���R�}���_�[��
    /// </summary>
    ///

    //�\�񂵂��X�L���̎��s���s��
    //From_BattleSceneCommand, To_FormationChara
    public struct ActiveSkillBootMessage
    {
        /*
        public int skillKey;
        public sbyte target;
        public sbyte user;
        public ActiveSkillBootMessage(int skillKey, sbyte targetPos, sbyte userPos)
        {
            this.skillKey = skillKey;
            this.target = targetPos;
            this.user = userPos;
        }
        */
    }

    //ActiveSkillCommander�ւ̃��b�Z�[�W
    public class ActiveSkillCommand
    {
        public int activeKey;
        public ActiveSkillPosition activePos;

        public ActiveSkillCommand(int activeKey, sbyte target, IGetFormationInfo userSO)
        {
            this.activeKey = activeKey;
            activePos = new ActiveSkillPosition(target, userSO);

        }
    }
    



    //From_BattleSceneCommand, To_FormationChara
    public struct MoveSkillBootMessage
    {

    }

    //To_MoveSKillCommander
    public struct MoveSkillCommand
    {
        public int moveKey;
        public MoveSkillPosition movePos;
        public MoveSkillCommand(int moveKey, sbyte target, sbyte user)
        {
            this.moveKey = moveKey;
            this.movePos = new MoveSkillPosition(target, user);
        }
    }



    //DamageCalc����̃��b�Z�[�W
    public class NormalDamageCalcMessage
    {
        public bool OnMagic;
        public float damage;
        public ActiveSkillPosition activePos;

        public NormalDamageCalcMessage(bool OnMagic, float damage, ActiveSkillPosition activePos)
        {
            this.OnMagic = OnMagic;
            this.damage = damage;
            this.activePos = activePos;
        }
    }


    /// <summary>
    /// �X�L���̎��smessage
    /// </summary>

    //�eActiveSkillMSO����eSoldier�ւ̃��b�Z�[�W
    public class NormalAttack {

        public ActiveSkillPosition activePos;

        public float activeRatio;

        public NormalAttack(ActiveSkillPosition activePos, float activeRatio)
        {
            this.activePos = activePos;
            this.activeRatio = activeRatio;
        }
    }    
    
    
    public class NormalMagic {

        public ActiveSkillPosition activePos;

        public float activeRatio;

        public NormalMagic(ActiveSkillPosition activePos, float activeRatio)
        {
            this.activePos = activePos;
            this.activeRatio = activeRatio;
        }
    }

    //moveSKill
    public struct NormalMoveToFront
    {

    }
    
    public struct NormalMoveToBack
    {

    }

    public struct TauntApproachMessage 
    {
        public MoveSkillPosition movePos;

        public TauntApproachMessage(MoveSkillPosition movePos)
        {
            this.movePos = movePos;
        }
    }





    /// <summary>
    /// Return��Sub����Get��Pub
    /// �O���ɔ͈͊O�֔�񂾂Ƃ��p��Sub��p��
    /// �i�L�����N�^�[�ƃG�l�~�[���ꂼ��p�Ӂj
    /// </summary>
    public class ReturnTargetName
    {
        public string targetName;
        public sbyte targetPos;
        public ReturnTargetName(string targetName, sbyte targetPos)
        {
            this.targetName = targetName;
            this.targetPos = targetPos;
        }
    }

    //To_From_MSP_ScopeOutSupporterSO, To_From_Formation(Chara,Enemy), From_TargetController
    public struct GetNextTargetName { }
    public struct GetPreTargetName { }



    //From_BattleSceneCommand, //To_Formation
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
    
    //From_EnemyActionSimulator, To_FormationChara
    //public struct GetTauntUserName { }

    public class SelectCharaNameMessage 
    {
        public string selectorName;
        public SelectCharaNameMessage(string selectorName )
        {
            this.selectorName = selectorName;
        }
    }



    /// <summary>
    /// Return��Sub����Get��APub
    /// AsyncPublishStrategy.Sequential�ň����������K�v�����邩��
    /// </summary>
    public struct EnemyTargetGetMessage { }
    public struct EnemyTargetReturn 
    {
        public sbyte targetForm;
        public int targetRate;
        public EnemyTargetReturn(sbyte targetForm, int targetRate)
        {
            this.targetForm = targetForm;
            this.targetRate = targetRate;
        }
    }



    //From_ActionOptionC, To_ActionSelectSimu
    public struct ASkillSimulateMessage
    {
        public string skillName;
        public ASkillSimulateMessage(string skillName)
        {
            this.skillName = skillName;
        }
    }



    //sbyte�̉����Z���������߂̍\����
    public struct SbyteHandler
    {
        public static sbyte NextForm(sbyte form)
        {
            form++;
            return form;
        }
        public static sbyte PreForm(sbyte form)
        {
            form--;
            return form;
        }
    }

}



public struct CommonActiveSkillTiming { }



public struct BookCommonMoveKeyMessage 
{
    public int skillNum;
    public BookCommonMoveKeyMessage(int skillNum)
    {
        this.skillNum = skillNum;
    }
}
public struct BookCommonMoveTargetMessage
{
    public sbyte targetNum;
    public BookCommonMoveTargetMessage(sbyte targetNum)
    {
        this.targetNum = targetNum;
    }

}
public struct BookCommonActiveKeyMessage
{
    public int skillNum;
    public BookCommonActiveKeyMessage( int skillNum)
    {
        this.skillNum = skillNum;
    }
}
public struct BookCommonActiveTargetMessage
{
    public sbyte targetNum;
    public BookCommonActiveTargetMessage(sbyte targetNum)
    {
        this.targetNum = targetNum;

    }
}






/// <summary>
/// Enemy��Targeting�ƑΉ�����^�E����
/// </summary>
//From_
public struct EnemyTargetingAllFrontChara { }
public struct EnemyTargetingSingleChara { }



public struct EnemyTargetFrontSimulate 
{
    public sbyte enemyForm;

    public EnemyTargetFrontSimulate(sbyte enemyForm)
    {
        this.enemyForm = enemyForm;
    }
}public struct EnemyTargetSingleCharaSimulate 
{
    public sbyte user;
    public sbyte target;

    public EnemyTargetSingleCharaSimulate(sbyte user, sbyte target)
    {
        this.user = user;
        this.target = target;
    }
}

/*
public struct FormationChangeMessage
{

}
*/

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


/// <summary>
/// ���O���ꊇ�ŕۑ�����B���o�����߂ɂ�sbyte��
/// </summary>

//From_FormationChar, To_AllyImageController
public struct MovePositionChangeMessage
{
    public MovePosition currentPos;
    public sbyte formNum;
    public MovePositionChangeMessage(MovePosition currentPos, sbyte formNum)
    {
        this.currentPos = currentPos;
        this.formNum = formNum;
    }
}

public struct MoveSimulateMessage
{
    public sbyte target;

    public MoveSimulateMessage(sbyte target)
    {
        this.target = target;
    }
}

public struct MoveToFrontSimulate { }
public struct MoveToBackSimulate { }
public struct MoveWaitSimulate { }

public struct TauntApproachSimulate
{
    public sbyte enemyForm;

    public TauntApproachSimulate(sbyte enemyForm)
    {
        this.enemyForm = enemyForm;
    }
}
public struct TauntSimulateCancell { }

public struct SetEnemyImage { }

public struct DescriptionDemendMessage 
{
    public string description;

    public DescriptionDemendMessage(string description)
    {
        this.description = description;
    }
}
public struct DescriptionFinishMessage { }