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
    [SerializeField]
    private int treeLevel;

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

public class MSO_CharacterDataSO : MessageableScriptableObject
{
    //[SerializeField]
    public CharacterEnumSO charaKey;

    public CharacterImageSO charaImages;

    //�K���̂���X�L���c���[�̃��X�g
    [SerializeField]
    private List<SkillTreeList> skillTreeCatalog = new List<SkillTreeList>();

    private IAsyncPublisher<RegistCommonSkill> registCommonAPub;
    protected IPublisher<RegistSkillFinish> registFinishPub;

    protected IPublisher<sbyte, FormCharacterMessage> formCharaPub;


    protected System.IDisposable disposable;

    protected MSO_CharacterDataSO thisComp;

    // Status
    //setter�ł�SerializeField�Ɗ�����
    [SerializeField]
    private int maxHP;
    [SerializeField]
    private int attack; 
    [SerializeField]
    private int agility;


    public override void MessageStart()
    {
        registCommonAPub = GlobalMessagePipe.GetAsyncPublisher<RegistCommonSkill>();

        registFinishPub = GlobalMessagePipe.GetPublisher<RegistSkillFinish>();

        thisComp = this;
        formCharaPub = GlobalMessagePipe.GetPublisher<sbyte, FormCharacterMessage>();

    }

    public void FormingChara(sbyte i)
    {


        formCharaPub.Publish(i, new FormCharacterMessage(thisComp));

        //Debug.Log("testSub");
    }

    //formation���甭�����CskillTreeCatalog(class�̃��X�g)�ɓo�^����Ă���skillTreeSO�̓����Ɋi�[����Ă���
    public async UniTask RegistMasterySkill(sbyte formNum)
    {
        await registCommonAPub.PublishAsync(new RegistCommonSkill(formNum));
        foreach(SkillTreeList skillTree in skillTreeCatalog)
        {
            skillTree.skillTreeSO.RegistMasterySkill(skillTree.GetTreeLeve(), formNum);
        }
        registFinishPub.Publish(new RegistSkillFinish());

    }

    public virtual int GetMaxHP()
    {
        return maxHP;
    }

    public virtual int GetAttack()
    {
        return attack;
    }

    public virtual int GetAgility()
    {
        return agility;
    }



}
