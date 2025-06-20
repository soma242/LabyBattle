using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

using UnityEditor;

using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning

using MessagePipe;
using DungeonSceneMessage;

[CreateAssetMenu(menuName = "MessageableSO/Component/Commander/map")]
public class MSO_DungeonMapCommander : MessageableScriptableObject
{
    [SerializeField] private List<DungeonMapDataSO> mapCatalog = new List<DungeonMapDataSO>();

    [HideInInspector]
    public DungeonMapMessage settedMap;
    [SerializeField]
    private DungeonMapMessage newGameMap;

    [SerializeField]
    private MSO_DungeonPositionHolderSO posHolder;


    private System.IDisposable disposable;


    public override void MessageStart()
    {
#if UNITY_EDITOR
        SortEnum();
#endif

        var bag = DisposableBag.CreateBuilder();

        //DungeonSceneを開いていないタイミングでのMap変更を補助する
        //現時点ではNewGameとContinue
        var adMapSub = GlobalMessagePipe.GetSubscriber<AdvanceMapMessage>();
        adMapSub.Subscribe(get =>
        {
            settedMap = new DungeonMapMessage(mapCatalog[get.mapKey], get.pos, get.direction, get.horizon);
            //var mapPub = GlobalMessagePipe.GetPublisher<DungeonMapMessage>();
            //mapPub.Publish(new DungeonMapMessage(mapCatalog[get.mapKey], get.pos, get.direction, get.horizon));
        }).AddTo(bag);

        var newGameASub = GlobalMessagePipe.GetAsyncSubscriber<NewGameMessage>();
        newGameASub.Subscribe(async (get, ct) =>
        {
            settedMap = newGameMap;
            //var mapPub = GlobalMessagePipe.GetPublisher<DungeonMapMessage>();
            //mapPub.Publish(newGameMap);
        }).AddTo(bag);

        var continueASub = GlobalMessagePipe.GetAsyncSubscriber<GameContinueMessage>();
        continueASub.Subscribe(async (get,ct) =>
        {
            string path = "Assets/BinaryData/ContinuePosition.bytes";
            if (File.Exists(path))
            {
                using (var stream = File.Open(path, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        int mapKey = reader.ReadInt32();
                        int posX = reader.ReadInt32();
                        int posY = reader.ReadInt32();
                        int direction = reader.ReadInt32();
                        bool horizon = reader.ReadBoolean();

                        settedMap = new DungeonMapMessage(mapCatalog[mapKey], new DungeonPos(posX, posY), direction, horizon);

                        //var mapPub = GlobalMessagePipe.GetPublisher<DungeonMapMessage>();
                        //mapPub.Publish(new DungeonMapMessage(mapCatalog[mapKey], new DungeonPos(posX,posY), direction, horizon));
                    }
                }
            }
        }).AddTo(bag);

        var saveSub = GlobalMessagePipe.GetSubscriber<SaveCallMessage>();
        saveSub.Subscribe(get =>
        {
            
            string path = "Assets/BinaryData/ContinuePosition.bytes";
            using (var stream = File.Open(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(posHolder.currentKey);
                    writer.Write(posHolder.currentPos.x);
                    writer.Write(posHolder.currentPos.y);
                    writer.Write(posHolder.currentDirection);
                    writer.Write(posHolder.horizon);
                }
            }
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
            foreach (DungeonMapDataSO map in mapCatalog)
            {
                map.mapKey = j;
                map.firstly = true;
                j++;
            }

            mapCatalog.TrimExcess();

        }
    }

#endif
}
