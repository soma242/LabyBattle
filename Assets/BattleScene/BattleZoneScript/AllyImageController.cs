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
            int i = 0;
            sbyte s = FormationScope.FirstChara();
            foreach(var ally in allyCatalog)
            {
                if (!formationCommander.GetParticipant(i))
                {
                    i++;
                    s++;
                    continue;
                }
                ally.SetImage(formationCommander.GetBattleImage(i));
                ally.SetMovePosition(formationCommander.GetMovePosition(i));

                var bagS = DisposableBag.CreateBuilder();
                selectStartSub.Subscribe(s, get =>
                {
                    listNum = FormationScope.FormToListChara(get.charaForm);
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

                disposableSelectStart = bagS.Build();

                i++;
                s++;

            }
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
        disposableOnDestroy?.Dispose();
    }

    /*
    [SerializeField] private MSO_FormationCharaSO formationSO;
    [SerializeField]
    private MovePosition imagePos;
    private MovePosition currentPos;

    private ISubscriber<sbyte, ChangeBattleImage> changeImageSub;
    private ISubscriber<sbyte, MovePositionChangeMessage> positionChangeSub;

    private System.IDisposable disposableOnDestroy;








    private Image image;
    //private ISubscriber<>


    //private System.IDisposable disposable;

    void Awake()
    {

        image = GetComponent<Image>();

        var bag = DisposableBag.CreateBuilder();

        changeImageSub = GlobalMessagePipe.GetSubscriber<sbyte, ChangeBattleImage>();
        positionChangeSub = GlobalMessagePipe.GetSubscriber<sbyte, MovePositionChangeMessage>();



        changeImageSub.Subscribe(formationSO.GetFormNum(), get =>
        {
            image.sprite = formationSO.setChara.charaImages.battleImage;

        }).AddTo(bag);

        positionChangeSub.Subscribe(formationSO.GetFormNum(),get =>
        {
            ChangePos(get.currentPos);
        }).AddTo(bag);





        disposableOnDestroy = bag.Build();
    }
    void OnDestroy()
    {
        disposableOnDestroy.Dispose();
    }

    private void ChangePos(MovePosition position)
    {
        if (imagePos == position)
        {
            image.enabled = true;

        }
        else
        {
            image.enabled = false;
        }
    }
    */

}
