using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using Cysharp.Threading.Tasks;
using SkillStruct;

#pragma warning disable CS4014 // disable warning

//継承元

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
        //コンストラクタはrun開始時に毎回呼ばれる
        //Debug.Log("constructor");
    }
    */
}

public class MSO_CharacterDataSO : MessageableScriptableObject
{
    //[SerializeField]
    public CharacterEnumSO charaKey;

    public CharacterImageSO charaImages;

    //適正のあるスキルツリーのリスト
    [SerializeField]
    private List<SkillTreeList> skillTreeCatalog = new List<SkillTreeList>();

    private IAsyncPublisher<RegistCommonSkill> registCommonAPub;
    protected IPublisher<RegistSkillFinish> registFinishPub;

    protected IPublisher<sbyte, FormCharacterMessage> formCharaPub;


    protected System.IDisposable disposable;

    protected MSO_CharacterDataSO thisComp;

    // Status
    //setterではSerializeFieldと干渉する
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

    //formationから発動し，skillTreeCatalog(classのリスト)に登録されているskillTreeSOの内部に格納されている
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
