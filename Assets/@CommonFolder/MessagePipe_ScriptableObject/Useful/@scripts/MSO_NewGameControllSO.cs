using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning
using Cysharp.Threading.Tasks;


using MessagePipe;

[CreateAssetMenu(menuName = "allScene/NewGame")]
public class MSO_NewGameControllSO : MessageableScriptableObject
{
    [SerializeField]
    private List<DungeonMapDataSO> mapCatalog = new List<DungeonMapDataSO>();

    public bool played { get; private set; }
    private bool trigger;

    private System.IDisposable disposable;
    
    public override void MessageStart()
    {
#if UNITY_EDITOR
        trigger = false;
#endif
        string path = "Assets/BinaryData/NewGameBool.bytes";
        if (File.Exists(path))
        {
            using (var stream = File.Open(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    played = reader.ReadBoolean();
                }
            }
        }
        else
        {
            played = false;
        }




        var bag = DisposableBag.CreateBuilder();
        var sub = GlobalMessagePipe.GetAsyncSubscriber<NewGameMessage>();
        sub.Subscribe(async (get,ct) =>
        {
            string path = "Assets/BinaryData/NewGameBool.bytes";
            using (var stream = File.Open(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(true);

                }
            }

            //newGameをしたと記録する
            path = "Assets/BinaryData/NewGameMap.bytes";
            using (var stream = File.Open(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {

                    //map毎に
                    //byteArrayとして保存することも考えたが，読み書きの段階でキャストが発生してしまうはず
                    foreach (var map in mapCatalog)
                    {
                        writer.Write(true);
                        map.NewGameSet();
                    }
                }
            }
        }).AddTo(bag);


        //continueした時にこれまで踏み入ったmapを確認する
        var contSub = GlobalMessagePipe.GetAsyncSubscriber<GameContinueMessage>();
        contSub.Subscribe(async (get,ct) =>{
            string path = "Assets/BinaryData/NewGameMap.bytes";
            if (File.Exists(path))
            {

                using (var stream = File.Open(path, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        foreach (var map in mapCatalog)
                        {
                            map.newGame = reader.ReadBoolean();
                        }
                    }
                }

            }
        }).AddTo(bag);

        var triggerSub = GlobalMessagePipe.GetSubscriber<NewGameboolSaveTrigger>();
        triggerSub.Subscribe(get =>
        {
            trigger = true;
        }).AddTo(bag);

        var quitSub = GlobalMessagePipe.GetSubscriber<SaveCallMessage>();
        quitSub.Subscribe(get =>
        {
            //Debug.Log("save");
            if (trigger)
            {
                //全マップのnewgame(bool)のセーブ
                SaveMapNewBool();
            }

#if UNITY_EDITOR
           foreach (var map in mapCatalog)
           {
               map.firstly = true;
           }
#endif

        }).AddTo(bag);


        disposable = bag.Build();
    }

    private void SaveMapNewBool()
    {
        var path = "Assets/BinaryData/NewGameMap.bytes";
        using (var stream = File.Open(path, FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {

                //map毎に
                //byteArrayとして保存することも考えたが，読み書きの段階でキャストが発生してしまうはず
                foreach (var map in mapCatalog)
                {
                    writer.Write(map.newGame);
                    map.NewGameSet();
                }
            }
        }
    }

}
