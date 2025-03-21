using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using BattleSceneMessage;
using MessagePipe;

#pragma warning disable CS1998// disable warning


public class CharaNameSimulator : MonoBehaviour
{
    private TMP_Text nameText;
    [SerializeField]
    private MSO_FormationCharaSO formationSO;

    private IAsyncSubscriber<BattlePrepareMessage> prepareASub;
    private System.IDisposable disposable;

    void Awake()
    {
        nameText = GetComponent<TMP_Text>();
        prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattlePrepareMessage>();
        disposable = prepareASub.Subscribe(async (get, ct) =>
        {
            nameText.SetText(formationSO.setChara?.name);
        });

        //nameText.SetText("myNameIs");
    }

    void OnDestroy()
    {
        disposable.Dispose();
    }
}
