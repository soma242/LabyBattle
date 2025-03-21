using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using MessagePipe;
 using VContainer;
using VContainer.Unity;
using BattleSceneMessage;

using Cysharp.Text;

#pragma warning disable CS1998 // disable warning


public class BattleLogController : MonoBehaviour
{
    [SerializeField]
    private GameObject battleLog;

    [SerializeField]
    private MSO_FormationCommander formCommander;

    //親のマスク
    [SerializeField]
    private Canvas canvas;

    private Image image;

    [SerializeField]
    private InputLayerSO layerSO;

    private BattleLogComponent logComp;

    private float height;

    private ISubscriber<DamageNoticeMessage> damageSub;

    private ISubscriber<DisableLogMessage> disableLogSub;

    private IAsyncSubscriber<ActionSelectEndMessage> selectEndASub;

    [Inject] private readonly ISubscriber<InputLayerSO, ScrollInput> scrollSub;

    private ISubscriber<NewLogSizeMessage> sizeSub;



    private System.IDisposable disposable;
    private System.IDisposable disposableSwitch;
    private System.IDisposable disposableNewLog;
    private System.IDisposable disposableCanvas;


    Utf16PreparedFormat<string, int> prepared = ZString.PrepareUtf16<string, int>("{0}に{1}のダメージ");

    void Awake()
    {
        damageSub = GlobalMessagePipe.GetSubscriber<DamageNoticeMessage>();

        disableLogSub = GlobalMessagePipe.GetSubscriber<DisableLogMessage>();

        selectEndASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectEndMessage>();

        sizeSub = GlobalMessagePipe.GetSubscriber<NewLogSizeMessage>();


        image = GetComponent<Image>();


        var bag = DisposableBag.CreateBuilder();

        SetCreateLogSub();

        height = 480f;

        disableLogSub.Subscribe(get =>
        {
            logComp = get.comp;
            height += get.height;

        }).AddTo(bag);

        disposableSwitch = disableLogSub.Subscribe(get =>
        {
            disposableSwitch?.Dispose();
            disposableNewLog?.Dispose();
            SetEnableLogSub();

        });

        sizeSub.Subscribe(get =>
        {
            height -= get.move.y;
            //Debug.Log(height);
        }).AddTo(bag);

        selectEndASub.Subscribe(async (get, ct) =>
        {
            canvas.enabled = true;
            disposableCanvas = scrollSub.Subscribe(layerSO, get =>
            {
                //Debug.Log(this.name);
                if(get.sign && image.rectTransform.anchoredPosition.y >= 0)
                {
                    return;
                }else if(!get.sign  && image.rectTransform.anchoredPosition.y <= height)
                {
                    return;
                }

                image.rectTransform.anchoredPosition += get.value;


            });
        }).AddTo(bag);

        disposable = bag.Build();
    }

    void OnDestroy()
    {
        disposable?.Dispose();
        disposableSwitch?.Dispose();
        disposableNewLog?.Dispose();
        disposableCanvas?.Dispose();
    }

    private void SetCreateLogSub()
    {
        var bag = DisposableBag.CreateBuilder();

        damageSub.Subscribe(get =>
        {


            var obj = Instantiate(battleLog, transform, false);
            //obj.transform.SetParent(transform);

            //var text = obj.GetComponent<TMP_Text>();
            var comp = obj.GetComponent<BattleLogComponent>();
            comp.SetReference();
            if (get.chara)
            {
                comp.InstantiateThisComp(prepared.Format(GetCharaName(get.target), get.damage));

            }
            else
            {
                comp.InstantiateThisComp(prepared.Format(GetEnemyName(get.target), get.damage));
            }
            //text.SetText(prepared.Format(get.name, get.damage));

        }).AddTo(bag);

        disposableNewLog = bag.Build();
    }

    private void SetEnableLogSub()
    {
        var bag = DisposableBag.CreateBuilder();

        damageSub.Subscribe(get =>
        {
            logComp.SetReference();

            if (get.chara)
            {
                logComp.InstantiateThisComp(prepared.Format(GetCharaName(get.target), get.damage));

            }
            else
            {
                logComp.InstantiateThisComp(prepared.Format(GetEnemyName(get.target), get.damage));

            }


            //Debug.Log("enable");
            //text.SetText(prepared.Format(get.name, get.damage));

        }).AddTo(bag);

        disposableNewLog = bag.Build();
    }


    private string GetCharaName(sbyte target)
    {
        return formCommander.GetCharaName(FormationScope.FormToListChara(target));
    }
    private string GetEnemyName(sbyte target)
    {
        return formCommander.GetEnemyName(FormationScope.FormToListEnemy(target));
    }

}
