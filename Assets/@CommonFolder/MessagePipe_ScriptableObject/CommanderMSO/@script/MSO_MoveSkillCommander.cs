using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using MessagePipe;
using SkillStruct;

[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/skill/move")]
public class MSO_MoveSkillCommander : MessageableScriptableObject
{
    //Hashsetは要素番号からの直接呼出しが見つからなかったため，listを用いる。
    [SerializeField] private List<MSO_MoveSkillSO> moveCatalog = new List<MSO_MoveSkillSO>();

    private ISubscriber<MoveSkillCommand> commandSub;
    private ISubscriber<TargetingMoveSkillCommand> targetingCommandSub;
    private System.IDisposable disposable;


    public override void MessageStart()
    {

#if UNITY_EDITOR
        SortEnum();
#endif
        var bag = DisposableBag.CreateBuilder();
        
        commandSub = GlobalMessagePipe.GetSubscriber<MoveSkillCommand>();
        targetingCommandSub = GlobalMessagePipe.GetSubscriber<TargetingMoveSkillCommand>();

        commandSub.Subscribe(i => {
            moveCatalog[i.moveNum].MoveSkillBoot(i.formNum);
        }).AddTo(bag);

        targetingCommandSub.Subscribe(i => {
            moveCatalog[i.moveNum].MoveSkillBoot(i.formNum, i.targetNum);
        }).AddTo(bag);

        disposable = bag.Build();

    }

#if UNITY_EDITOR

    private void SortEnum()
    {
        //プレイ時間外に働いたか確認用(OnEnableの作動タイミング確認につかった)
        //Debug.Log("enableRun");
        //OnEnable，Awake共に，インスペクタ上でこのSOを開いているかシーン上に参照を作らなければ走らなかった。
        //@zzzにアタッチしたSetProviderReferenceから参照を作っている。
        //実際に使用するときは最初に起動するシーンに参照を作ること。
        //一度動けばいいので他のシーンからの参照は消す

        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            int j = 0;
                      //Debug.Log("Unity Editor");


            //MessageableScriptableObjectの種類ごとに対応したInjectListから行う。
            //activeSkillSO.MessageDependencyInjection();
            foreach (MSO_MoveSkillSO moveSkill in moveCatalog)
            {
                moveSkill.moveKey.SetMoveNum(j);
                j++;
            }

            moveCatalog.TrimExcess();


        }
    }

#endif
}
