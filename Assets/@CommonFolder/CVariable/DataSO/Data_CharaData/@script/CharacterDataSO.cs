using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using Cysharp.Threading.Tasks;
using SkillStruct;

#pragma warning disable CS4014 // disable warning

//�p����

[System.Serializable]
public class SkillTreeList
{
    public MSO_BaseSkillTreeSO skillTreeSO;
    public int treeLevel;



    public void TreeLeveUpl()
    {
        treeLevel++;
    }
    public void TreeLevelReset()
    {
        treeLevel = 0;
    }

    public int GetTreeLeve()
    {
        return treeLevel;
    }

    /*
    public SkillTreeList()
    {
        //�R���X�g���N�^��run�J�n���ɖ���Ă΂��
        //Debug.Log("constructor");
    }
    */
}

public class CharacterDataSO : ScriptableObject
{
    //[SerializeField]
    public CharacterEnumSO charaKey;

    public CharacterImageSO charaImages;

    //�K���̂���X�L���c���[�̃��X�g
    [SerializeField]
    private List<SkillTreeList> skillTreeCatalog = new List<SkillTreeList>();

    //private IAsyncPublisher<RegistCommonSkill> registCommonAPub;
    //protected IPublisher<RegistSkillFinish> registFinishPub;


    //public StateResistanceSO resist;



    // Status
    //setter�ł�SerializeField�Ɗ�����
    [SerializeField]
    private string charaName;
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private int attack; 
    [SerializeField]
    private int magic; 
    [SerializeField]
    private int defence; 
    [SerializeField]
    private int mDefence; 
    [SerializeField]
    private int agility;

    [SerializeField]
    private int targetRate = 50;






    //formation���甭�����CskillTreeCatalog(class�̃��X�g)�ɓo�^����Ă���skillTreeSO�̓����Ɋi�[����Ă���
    public virtual async UniTask RegistMasterySkill(sbyte formNum)
    {
        //From_FormationChara
        var registCommonAPub = GlobalMessagePipe.GetAsyncPublisher<RegistCommonSkill>();
        var registFinishPub = GlobalMessagePipe.GetPublisher<RegistSkillFinish>();

        await registCommonAPub.PublishAsync(new RegistCommonSkill(formNum));
        foreach(SkillTreeList skillTree in skillTreeCatalog)
        {
            skillTree.skillTreeSO.RegistMasterySkill(skillTree.GetTreeLeve(), formNum);
        }
        registFinishPub.Publish(new RegistSkillFinish());

    }

    public virtual void SetStateSub(sbyte form, DisposableBagBuilder bag, Effects effects)
    {
        _ = new StateResist(form, bag, effects);

    }

    public virtual string GetCharaName()
    {
        return charaName;
    }

    public virtual int GetMaxHP()
    {
        return maxHP;
    }

    public virtual int GetAttack()
    {
        return attack;
    }
    public virtual int GetDefence()
    {
        return defence;
    }
    public virtual int GetMagic()
    {
        return magic;
    }
    public virtual int GetMagicDefence()
    {
        return mDefence;
    }

    public virtual int GetAgility()
    {
        return agility;
    }

    public virtual int GetTargetRate()
    {
        return targetRate;
    }

    public List<SkillTreeList> GetTreeList()
    {
        return skillTreeCatalog;
    }



}
