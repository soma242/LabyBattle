using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;

using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/formation/chara")]
public class MSO_FormationCommander : MessageableScriptableObject
{
    [SerializeField]
    private List<MSO_FormationCharaSO> formationCharaCatalog = new List<MSO_FormationCharaSO>();
    
    [SerializeField]
    private List<MSO_FormationEnemySO> formationEnemyCatalog = new List<MSO_FormationEnemySO>();

    /*
    private ISubscriber<NormalMoveToFront> moveToFrontSub;
    private ISubscriber<NormalMoveToBack> moveToBackSub;

    private System.IDisposable disposable;
    */

    public override void MessageStart()
    {

#if UNITY_EDITOR
        formationCharaCatalog.TrimExcess();
        formationEnemyCatalog.TrimExcess();
#endif

        /*
        var bag = DisposableBag.CreateBuilder();

        moveToFrontSub = GlobalMessagePipe.GetSubscriber<NormalMoveToFront>();
        moveToBackSub = GlobalMessagePipe.GetSubscriber<NormalMoveToBack>();

        moveToFrontSub.Subscribe(get =>
        {
            formationCharaCatalog[FormationScope.FormToListChara(get.user)].MoveToFront();
        }).AddTo(bag);

        moveToBackSub.Subscribe(get =>
        {
            formationCharaCatalog[FormationScope.FormToListChara(get.user)].MoveToBack();
        }).AddTo(bag);

        disposable = bag.Build();
        */

    }

    //Chara
    public string GetCharaName(int i)
    {
        return formationCharaCatalog[i].setChara.GetCharaName();
    }
    public bool GetParticipant(int i)
    {
        return formationCharaCatalog[i].participant;
    }

    public Sprite GetBattleImage(int i)
    {
        return formationCharaCatalog[i].setChara.charaImages.battleImage;
    }

    public MovePosition GetMovePosition(int i)
    {
        return formationCharaCatalog[i].currentPos;
    }

    public float GetCharaRatioOnHP(int i)
    {
        Debug.Log(formationCharaCatalog[i].GetRatioOnHP());
        return formationCharaCatalog[i].GetRatioOnHP();
    }

    //Enemy
    public string GetEnemyName(int i)
    {
        return formationEnemyCatalog[i].enemy.GetEnemyName();
    }

    public string GetSettedSkillName(int i)
    {
        return formationEnemyCatalog[i].GetSettedSkillName();
    }

    public bool GetEnemyParticipant(int i)
    {
        return formationEnemyCatalog[i].participant;
    }

    public float GetEnemyRatioOnHP(int i)
    {
        return formationEnemyCatalog[i].GetRatioOnHP();
    }
}
