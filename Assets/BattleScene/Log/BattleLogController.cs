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
    private InputLayerSO battleOptionLayer;
    [SerializeField]
    private InputLayerSO battleLogLayer;

    private BattleLogComponent logComp;

    private float height;

    private ISubscriber<DamageNoticeMessage> damageSub;

    private ISubscriber<DisableLogMessage> disableLogSub;

    //private IAsyncSubscriber<ActionSelectEndMessage> selectEndASub;

    [Inject] private readonly ISubscriber<InputLayerSO, ScrollInput> scrollSub;

    private ISubscriber<NewLogSizeMessage> sizeSub;


    private ISubscriber<InputLayerSO, InputLayerChanged> layerChangeSub;



    private System.IDisposable disposable;
    private System.IDisposable disposableSwitch;
    private System.IDisposable disposableNewLog;
    private System.IDisposable disposableCanvas;


    Utf16PreparedFormat<string, int> damageLog = ZString.PrepareUtf16<string, int>("{0}に{1}のダメージ");
    Utf16PreparedFormat<string> knockOutLog = ZString.PrepareUtf16<string>("{0}が倒れた");

    void Awake()
    {
        damageSub = GlobalMessagePipe.GetSubscriber<DamageNoticeMessage>();

        disableLogSub = GlobalMessagePipe.GetSubscriber<DisableLogMessage>();

        //selectEndASub = GlobalMessagePipe.GetAsyncSubscriber<ActionSelectEndMessage>();

        sizeSub = GlobalMessagePipe.GetSubscriber<NewLogSizeMessage>();


        layerChangeSub = GlobalMessagePipe.GetSubscriber<InputLayerSO, InputLayerChanged>();

        image = GetComponent<Image>();


        var bag = DisposableBag.CreateBuilder();

        SetCreateLogSub();

        height = 480f;

        //次のログに使いまわすコンポーネントを受け取る
        disableLogSub.Subscribe(get =>
        {
            logComp = get.comp;
            height += get.height;

        }).AddTo(bag);

        //生成から使いまわしにスイッチ
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

        layerChangeSub.Subscribe(battleLogLayer, get =>
        {
            canvas.enabled = true;
            disposableCanvas = scrollSub.Subscribe(battleLogLayer, get =>
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

        layerChangeSub.Subscribe(battleOptionLayer, get =>
        {
            disposableCanvas?.Dispose();
            canvas.enabled = false;
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


    //ログの受信を以下の二つの関数で行う
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
                comp.InstantiateThisComp(damageLog.Format(GetCharaName(get.target), get.damage));

            }
            else
            {
                comp.InstantiateThisComp(damageLog.Format(GetEnemyName(get.target), get.damage));
            }
            //text.SetText(damageLog.Format(get.name, get.damage));

        }).AddTo(bag);

        var dropEnemySub = GlobalMessagePipe.GetSubscriber<DropEnemyMessage>();
        dropEnemySub.Subscribe(get =>
        {
            var obj = Instantiate(battleLog, transform, false);
            //obj.transform.SetParent(transform);

            //var text = obj.GetComponent<TMP_Text>();
            var comp = obj.GetComponent<BattleLogComponent>();
            comp.SetReference();
            comp.InstantiateThisComp(knockOutLog.Format(GetEnemyName(get.pos)));
        }).AddTo(bag);

        var dropCharaSub = GlobalMessagePipe.GetSubscriber<DropCharaMessage>();
        dropCharaSub.Subscribe(get =>
        {
            var obj = Instantiate(battleLog, transform, false);
            //obj.transform.SetParent(transform);

            //var text = obj.GetComponent<TMP_Text>();
            var comp = obj.GetComponent<BattleLogComponent>();
            comp.SetReference();
            comp.InstantiateThisComp(knockOutLog.Format(GetCharaName(get.pos)));
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
                logComp.InstantiateThisComp(damageLog.Format(GetCharaName(get.target), get.damage));

            }
            else
            {
                logComp.InstantiateThisComp(damageLog.Format(GetEnemyName(get.target), get.damage));

            }


            //Debug.Log("enable");
            //text.SetText(damageLog.Format(get.name, get.damage));

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
