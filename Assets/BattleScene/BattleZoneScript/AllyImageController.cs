using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

using MessagePipe;
using BattleSceneMessage;

#pragma warning disable CS1998 // disable warning




[System.Serializable]
public class AllyImageUI
{
    private MovePosition imagePos;
    public Canvas frontCanvas;
    public Canvas backCanvas;
    public Image frontImage;
    public Image backImage;

    public TMP_Text moveSimu;
    public Canvas simuCanvas;

    public void SetImage(Sprite battleImage)
    {
        frontImage.sprite = battleImage;
        backImage.sprite = battleImage;
    }

    public void EnableFrontCanvas()
    {
        frontCanvas.enabled = true;
        backCanvas.enabled = false;
    }

    public void EnableBackCanvas()
    {
        frontCanvas.enabled = false;
        backCanvas.enabled = true;
    }

    public void DisableCanvas()
    {
        frontCanvas.enabled = false;
        backCanvas.enabled = false;
        simuCanvas.enabled = false;
    }

    public void SetMovePosition(MovePosition imagePos)
    {
        this.imagePos = imagePos;
        simuCanvas.enabled = false;
        if (imagePos == MovePosition.front)
        {
            EnableFrontCanvas();
        }
        else
        {
            EnableBackCanvas();
        }
    }
}

public class AllyImageController : MonoBehaviour
{
    [SerializeField]
    private List<AllyImageUI> allyCatalog = new List<AllyImageUI>();

    [SerializeField]
    private MSO_FormationCommander formationCommander;

    private IAsyncSubscriber<BattlePrepareMessage> prepareASub;

    private ISubscriber<MovePositionChangeMessage> positionChangeSub;

    private System.IDisposable disposableOnDestroy;


    //MoveSimulator
    private int listNum;

    private string backString = "後退";
    private string frontString = "前進";

    private ISubscriber<MoveToBackSimulate> toBackSimSub;
    private ISubscriber<MoveToFrontSimulate> toFrontSimSub;
    private ISubscriber<MoveWaitSimulate> waitSimSub;

    private System.IDisposable disposableDisable;

    private System.IDisposable disposableEnable;




    private ISubscriber<sbyte, ActionSelectStartMessage> selectStartSub;

    private System.IDisposable disposableSelectStart;


    void Awake()
    {
        prepareASub = GlobalMessagePipe.GetAsyncSubscriber<BattlePrepareMessage>();

        positionChangeSub = GlobalMessagePipe.GetSubscriber<MovePositionChangeMessage>();

        //MoveSimulator
        toBackSimSub = GlobalMessagePipe.GetSubscriber<MoveToBackSimulate>();
        toFrontSimSub = GlobalMessagePipe.GetSubscriber<MoveToFrontSimulate>();
        waitSimSub = GlobalMessagePipe.GetSubscriber<MoveWaitSimulate>();

        selectStartSub = GlobalMessagePipe.GetSubscriber<sbyte, ActionSelectStartMessage>();

        var bag = DisposableBag.CreateBuilder();

        prepareASub.Subscribe(async (get, ct) =>
        {
            //一度しか入っていない
            Debug.Log("prepare");
            int i = 0;
            sbyte s = FormationScope.FirstChara();
            disposableSelectStart?.Dispose();

            //Scene読み込みごとにSubが行われている
            //prepareASub  Pub側は一度しか読み込まれていないとかくにん
            //selectStartSub  Pub側は一度しか読み込まれていないとかくにん

            //bagSの生成およびBuildがforeachの中に入っていた
            //=>都度生成されてBuildされてないタイミングのものはリーク(残り続ける)
            var bagS = DisposableBag.CreateBuilder();

            foreach (var ally in allyCatalog)
            {

                if (!formationCommander.GetParticipant(i))
                {
                    i++;
                    s++;
                    continue;
                }
                ally.SetImage(formationCommander.GetBattleImage(i));
                ally.SetMovePosition(formationCommander.GetMovePosition(i));

                selectStartSub.Subscribe(s, get =>
                {
                    listNum = FormationScope.FormToListChara(get.charaForm);
                    disposableEnable?.Dispose();
                    //Debug.Log("allyImage" + listNum);

                    var bagE = DisposableBag.CreateBuilder();

                    toBackSimSub.Subscribe(get =>
                    {
                        disposableEnable?.Dispose();
                        allyCatalog[listNum].simuCanvas.enabled = true;
                    }).AddTo(bagE);
                    toFrontSimSub.Subscribe(get =>
                    {
                        disposableEnable?.Dispose();
                        allyCatalog[listNum].simuCanvas.enabled = true;
                    }).AddTo(bagE);

                    disposableEnable = bagE.Build();


                }).AddTo(bagS);


                i++;
                s++;

            }
            disposableSelectStart = bagS.Build();

        }).AddTo(bag);

        positionChangeSub.Subscribe(get =>
        {
            int i = FormationScope.FormToListChara(get.formNum);
            allyCatalog[i].SetMovePosition(get.currentPos);
        }).AddTo(bag);



        //moveSimulator
        toBackSimSub.Subscribe(get =>
        {
            allyCatalog[listNum].moveSimu.SetText(backString);
        }).AddTo(bag);

        //後ろから前へ
        toFrontSimSub.Subscribe(get =>
        {
            allyCatalog[listNum].moveSimu.SetText(frontString);
        }).AddTo(bag);

        //その場で待機。
        waitSimSub.Subscribe(get =>
        {
            disposableDisable?.Dispose();
            allyCatalog[listNum].simuCanvas.enabled = false;

            var bagD = DisposableBag.CreateBuilder();

            toBackSimSub.Subscribe(get =>
            {
                disposableDisable?.Dispose();
                allyCatalog[listNum].simuCanvas.enabled = true;
            }).AddTo(bagD);
            toFrontSimSub.Subscribe(get =>
            {
                disposableDisable?.Dispose();
                allyCatalog[listNum].simuCanvas.enabled = true;
            }).AddTo(bagD);

            disposableDisable = bagD.Build();

        }).AddTo(bag);



        var dropCharaSub = GlobalMessagePipe.GetSubscriber<DropCharaMessage>();

        dropCharaSub.Subscribe(get =>
        {
            allyCatalog[FormationScope.FormToListChara(get.pos)].DisableCanvas();
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        //Debug.Log("destroyAllyImageCont");
        disposableOnDestroy?.Dispose();
        disposableDisable?.Dispose();
        disposableEnable?.Dispose();
        disposableSelectStart?.Dispose();
        //Debug.Log("destroyAllyImageCont Complete");
    }
}