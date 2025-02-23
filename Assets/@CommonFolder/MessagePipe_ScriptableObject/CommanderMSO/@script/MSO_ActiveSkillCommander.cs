using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using MessagePipe;
using SkillStruct;


//継承元のスクリプタブルオブジェクトで検索できるので，「t:MSO_ActiveSkillSO」で検索してリストに入れる。


[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/skill/active")]
public class MSO_ActiveSkillCommander : MessageableScriptableObject
{
    //Hashsetは要素番号からの直接呼出しが見つからなかったため，listを用いる。
    [SerializeField] private List<MSO_ActiveSkillSO> activeCatalog = new List<MSO_ActiveSkillSO>();

    private ISubscriber<ActiveSkillCommand> commandSubscriber;
    private System.IDisposable disposable;


    public override void MessageStart() {

#if UNITY_EDITOR
        SortEnum();
#endif

        commandSubscriber = GlobalMessagePipe.GetSubscriber<ActiveSkillCommand>();

        disposable = commandSubscriber.Subscribe(i =>{
            activeCatalog[i.activeNum].ActiveSkillBoot(i.activePos);
        });

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
            foreach (MSO_ActiveSkillSO activeSkill in activeCatalog)
            {
                activeSkill.activeKey.activeNum = j;
                j++;
            }

            activeCatalog.TrimExcess();


        }
    }

#endif
}
