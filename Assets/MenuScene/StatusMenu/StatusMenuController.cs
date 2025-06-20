using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using MessagePipe;
using MenuScene;

using Cysharp.Text;


public class StatusMenuController : MonoBehaviour
{
    [SerializeField]
    private MenuSelectHolderSO holder;

    [SerializeField]
    private MSO_FormationCommander forms;

    [SerializeField]
    private List<StatusMemberHolder> members = new List<StatusMemberHolder>();
    //text, image

    private CharacterDataSO chara;

    private int memberNum;

    private bool skillField;

    private IPublisher<StatusMemberChangeMessage> changePub;

    private System.IDisposable disposableOnDestroy;
    private System.IDisposable disposableStatus;

    private Utf16PreparedFormat<int> hpStatus = ZString.PrepareUtf16<int>("MaxHP: {0}");
    [SerializeField] private TMP_Text hp;

    private Utf16PreparedFormat<int> attackStatus = ZString.PrepareUtf16<int>("Attack: {0}");
    [SerializeField] private TMP_Text attack;

    private Utf16PreparedFormat<int> defenceStatus = ZString.PrepareUtf16<int>("Defence: {0}");
    [SerializeField] private TMP_Text defence;

    private Utf16PreparedFormat<int> magicStatus = ZString.PrepareUtf16<int>("Magic: {0}");
    [SerializeField] private TMP_Text magic;

    private Utf16PreparedFormat<int> magicDefenceStatus = ZString.PrepareUtf16<int>("MagicDefence: {0}");
    [SerializeField] private TMP_Text magicDefence;
    private Utf16PreparedFormat<int> agilityStatus = ZString.PrepareUtf16<int>("Agility: {0}");
    [SerializeField] private TMP_Text agility;

    private Utf16PreparedFormat<int> targetRateStatus = ZString.PrepareUtf16<int>("TargetRate: {0}");
    [SerializeField] private TMP_Text targetRate;


    void Awake()
    {
        changePub = GlobalMessagePipe.GetPublisher<StatusMemberChangeMessage>();

        skillField = false;
        memberNum = 0;
        foreach (var member in members)
        {
            member.id = memberNum;
            member.text.SetText(forms.GetCharaName(memberNum));
            memberNum++;
        }

        var bag = DisposableBag.CreateBuilder();

        var statusSub = GlobalMessagePipe.GetSubscriber<MainToStatusMessage>();
         statusSub.Subscribe(get =>
        {
            Debug.Log("status");

            memberNum = 0;
            SetSelecting();

            var bagS = DisposableBag.CreateBuilder();
            holder.upSub.Subscribe(holder.statusLayer, get =>
            {
                ResetSelecting();

                if(memberNum == 0)
                {
                    memberNum = members.Count - 1;
                }
                else
                {
                    memberNum--;
                }
                SetSelecting();

            }).AddTo(bagS);

            holder.downSub.Subscribe(holder.statusLayer, get =>
            {
                ResetSelecting();

                if(memberNum == members.Count - 1)
                {
                    memberNum = 0;
                }
                else
                {
                    memberNum++;
                }

                SetSelecting();
            }).AddTo(bagS);

            holder.leftSub.Subscribe(holder.statusLayer, get =>
            {
                ResetSelecting();
                var backMainPub = GlobalMessagePipe.GetPublisher<StatusToMainMessage>();
                backMainPub.Publish(new StatusToMainMessage());
            }).AddTo(bagS);

            holder.rightSub.Subscribe(holder.statusLayer, get =>
            {
                skillField = true;
                holder.layerPub.Publish(new InputLayer(holder.skillLayer));
                var skillPub = GlobalMessagePipe.GetPublisher<StatusToSkillMessage>();
                skillPub.Publish(new StatusToSkillMessage());
            }).AddTo(bag);

            holder.menuSub.Subscribe(holder.statusLayer, get =>
            {
                ResetSelecting();
                var backMainPub = GlobalMessagePipe.GetPublisher<StatusToMainMessage>();
                backMainPub.Publish(new StatusToMainMessage());
            }).AddTo(bag);

            holder.enterSub.Subscribe(holder.statusLayer, get =>
            {
                skillField = true;
                holder.layerPub.Publish(new InputLayer(holder.skillLayer));
                var skillPub = GlobalMessagePipe.GetPublisher<StatusToSkillMessage>();
                skillPub.Publish(new StatusToSkillMessage());
            }).AddTo(bag);

            var fromSkillSub = GlobalMessagePipe.GetSubscriber<SkillToStatusMessage>();
            fromSkillSub.Subscribe(get =>
            {
                skillField = false;
            }).AddTo(bag);

            var clickSub = GlobalMessagePipe.GetSubscriber<MemberHolderClickMessage>();
            clickSub.Subscribe(get =>
            {
                if (skillField)
                {
                    holder.layerPub.Publish(new InputLayer(holder.statusLayer));
                    skillField = false;
                }
                if (members[get.id].selecting)
                {
                    return;
                }
                ResetSelecting();
                memberNum = get.id;
                SetSelecting();
            }).AddTo(bagS);

            disposableStatus = bagS.Build();
        }).AddTo(bag);

        var mainSub = GlobalMessagePipe.GetSubscriber<StatusToMainMessage>();
        mainSub.Subscribe(targetRate =>
        {
            members[memberNum].image.sprite = holder.sourceImageSO.offSelect;

            disposableStatus?.Dispose();
            holder.pointerPub.Publish(new MenuIPointerMessage());
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();

        SetStatusText(0);

    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }

    private void SetStatusText(int num)
    {
        chara = forms.GetCharaData(num);
        hp.SetText(hpStatus.Format(chara.GetMaxHP()));
        attack.SetText(attackStatus.Format(chara.GetAttack()));
        defence.SetText(defenceStatus.Format(chara.GetDefence()));
        magic.SetText(magicStatus.Format(chara.GetMagic()));
        magicDefence.SetText(magicDefenceStatus.Format(chara.GetMagicDefence()));
        agility.SetText(agilityStatus.Format(chara.GetAgility()));
        targetRate.SetText(targetRateStatus.Format(chara.GetTargetRate()));
    }

    private void SetSelecting()
    {
        members[memberNum].image.sprite = holder.sourceImageSO.onSelect;
        members[memberNum].selecting = true;
        SetStatusText(memberNum);
        //To_SkillFieldCont
        changePub.Publish(new StatusMemberChangeMessage(memberNum));
    }

    private void ResetSelecting()
    {
        members[memberNum].image.sprite = holder.sourceImageSO.offSelect;
        members[memberNum].selecting = false;
    }
}
