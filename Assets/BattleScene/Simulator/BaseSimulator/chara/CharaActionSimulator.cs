using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using MessagePipe;

public class CharaActionSimulator : MonoBehaviour
{
    private TMP_Text actionName;
    [SerializeField]
    private FormationEnumSO formNumSO;

    //private ActiveSkillHolderSO aSkillHolder;

    private ISubscriber<sbyte, ChangeActionMessage> changeActionSub;
    private System.IDisposable disposable;

    void Awake()
    {
        actionName = GetComponent<TMP_Text>();
        changeActionSub = GlobalMessagePipe.GetSubscriber<sbyte, ChangeActionMessage>();

        disposable = changeActionSub.Subscribe(formNumSO.formationNum, get =>
        {
           // aSkillHolder = get.aSkillHolder;
            actionName.SetText(get.aSkillHolder.GetSkillName());
        });
    }
}
