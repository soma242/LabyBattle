using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

#pragma warning disable EPC12 // disable warning

//using System.Runtime.InteropServices;

using DungeonSceneMessage;
using MessagePipe;

using Cysharp.Threading.Tasks;


/// <summary>
/// 
/// </summary>

public interface IDungeonMapDataPicker
{
    void MiniMapGenerate();
    //void ISetComponent();
    bool IGetWallBool(DungeonPos pos);
    void IChangeScreen(DungeonPos pos);
    IDisposable ISetScreenSave();
    IDisposable ISetComponentSave();

}

[System.Serializable]
public struct DungeonPos
{
    public int x;
    public int y;

    public DungeonPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public class DungeonSquare
{
    public List<bool> horizontal = new List<bool>();

    public DungeonSquare(List<bool> horizontal)
    {
        this.horizontal = horizontal;
    }
}

//Componentの種類， 現在の有効， 設置位置を内包
[System.Serializable]
public class DungeonComponentData
{
    public bool valid = true;
    public DungeonComponentSO comp;
    public DungeonPos pos;

    public System.IDisposable disposable;

#if UNITY_EDITOR
    public void Initialize()
    {
        if (comp.initValid)
        {
            valid = true;
        }
    }
#endif

    public void SetValid()
    {
        
        var bag = DisposableBag.CreateBuilder();

        comp.SetComponent(bag, pos);

        if (comp.OverLapBool())
        {
            var overLapSub = GlobalMessagePipe.GetSubscriber<DungeonPos, ComponentOverlapMessage>();

            overLapSub.Subscribe(pos, get =>
            {

                disposable.Dispose();
                valid = false;
                //Saveのトリガーをオンにする
                var compPub = GlobalMessagePipe.GetPublisher<ComponentValidMessage>();
                compPub.Publish(new ComponentValidMessage());


            }).AddTo(bag);
        }

        var resetSub = GlobalMessagePipe.GetSubscriber<ResetMapMessage>();
        resetSub.Subscribe(get =>
        {
            disposable?.Dispose();
        }).AddTo(bag);

        disposable = bag.Build() ;
    }
}

[System.Serializable]
public abstract class DungeonComponentSO : ScriptableObject
{
    public bool initValid = true;

    public abstract void SetComponent(DisposableBagBuilder bag, DungeonPos pos);
    //trueを返すと，重なった時にvalidがfalseになる
    public abstract bool OverLapBool();
}

/// <summary>
/// Dugeonのマップを渡すためのMessage
/// </summary>
[System.Serializable]
public class DungeonMapMessage
{
    public DungeonMapDataSO mapData;
    public DungeonPos setPos;
    public int setDirection;
    public bool setHorizon;

    public DungeonMapMessage(DungeonMapDataSO mapData, DungeonPos setPos, int setDirection, bool setHorizon)
    {
        this.mapData = mapData;
        this.setPos = setPos;
        this.setDirection = setDirection;
        this.setHorizon = setHorizon;
    }
}

public class AdvanceMapMessage 
{
    public int mapKey;
    public DungeonPos pos;
    public int direction;
    public bool horizon;

    public AdvanceMapMessage(int mapKey, DungeonPos pos, int direction, bool horizon)
    {
        this.mapKey = mapKey;
        this.pos = pos;
        this.direction = direction;
        this.horizon = horizon;
    }
}


//ScreenのPubSub格納用のClass
//最低限のIDisposableで管理するため
public class ScreenDisposableAdmin
{
    private System.IDisposable disposable;

    public ScreenDisposableAdmin(int x, int y)
    {
        var pos = new DungeonPos(x, y);

        var bag = DisposableBag.CreateBuilder();
        var resetSub = GlobalMessagePipe.GetSubscriber<ResetMapMessage>();
        var checkSub = GlobalMessagePipe.GetSubscriber<DungeonPos, ComponentCheckMessage>();


        resetSub.Subscribe(get =>
        {
            disposable?.Dispose();
        }).AddTo(bag);

        checkSub.Subscribe(pos, get =>
        {
            disposable?.Dispose();
            var truePub = GlobalMessagePipe.GetPublisher<ScreenTrueMessage>();
            truePub.Publish(new ScreenTrueMessage(pos));
            //Debug.Log("admin: " +pos.x + ":x, y:" + pos.y);
        }).AddTo(bag);


        disposable = bag.Build();
    }
}


/// <summary>
/// 本体
/// </summary>

[CreateAssetMenu(menuName = "dungeonScene/mapData")]
public class DungeonMapDataSO : ScriptableObject, IDungeonMapDataPicker
{
    public int mapKey;

    public List<DungeonSquare> vertical = new List<DungeonSquare>();
    //ダンジョン内の要素
    public List<DungeonComponentData> addition = new List<DungeonComponentData>();

    //interfaceに移す
    public EncounterEnemySO encountReference;
    public IEncounterEnemy encount;

    private IDisposable disposable;
    //private IDisposable disposableScreen;
    private IDisposable disposableSave;

    //初期化しなければLengthも確認できない
    private BitArray[] screens = new BitArray[MapSize.size];

    //[SerializeField]
    private int screenCount;
    //[SerializeField]
    private bool screenValid = true;

    //trueの場合にファイルデータ読み込み(ゲーム起動から初回読み込みの場合）
    public bool firstly = true;
    //trueの場合にゲーム終了時にファイルデータに書き込み
    //Disposableを一つにするためにMapHolderから変更する
    //compPub
    private bool save = false;
    //truePub
    private bool saveScreen = false;


    public bool newGame = true;


    //チェックが入ってないのがFalse(初期)



    //DungeonPositionHolder => DungeonMapControllerから起動
    public void MiniMapGenerate()
    {
        



        //Debug.Log(firstly);
        //Debug.Log(screenValid);

        //Debug.Log(newGame);

        //NewGameしてから最初にMapに入ったならば
        if (newGame)
        {
            newGame = false;
            var triggerPub = GlobalMessagePipe.GetPublisher<NewGameboolSaveTrigger>();
            triggerPub.Publish(new NewGameboolSaveTrigger());

            firstly = false;
            encount = encountReference;
            encount.ISetSum();

            screenValid = true;

            //save = true;

            saveScreen = true;
            int size = MapSize.size;

                screens = new BitArray[size];

            for (int i = 0; i < size; i++)
            {
                screens[i] = new BitArray(size, false);
            }

        }


        //ゲーム起動から最初にMapに入ったならば
        if (firstly)
        {
            firstly = false;
            encount = encountReference;
            //Debug.Log(encountReference.name);
            encount.ISetSum();
            //compのvalid及びscreenValidの読み込み
            var path = "Assets/BinaryData/" + name + "MapBooleans.bytes";
            if (File.Exists(path))
            {
                try
                {
                    using (var stream = File.Open(path, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                        {
                            /*
                            aspectRatio = reader.ReadSingle();
                            tempDirectory = reader.ReadString();
                            autoSaveTime = reader.ReadInt32();
                            showStatusBar = reader.ReadBoolean();
                            */
                            screenValid = reader.ReadBoolean();
                            foreach (var item in addition)
                            {
                                item.valid = reader.ReadBoolean();
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    save = true;
                    Debug.LogWarning(name + ": SaveFileError" + e.Message);
                    //throw e;
                }

            }
            else
            {
                save = true;
            }



            //screenの読み込み（or初期化）
            if (screenValid)
            {


                path = "Assets/BinaryData/" + name + "MapScreen.bytes";
                //Debug.Log(File.Exists(path));
                if (File.Exists(path))
                {

                    using (var stream = File.Open(path, FileMode.Open))
                    {
                        using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                        {
                            /*
                            aspectRatio = reader.ReadSingle();
                            tempDirectory = reader.ReadString();
                            autoSaveTime = reader.ReadInt32();
                            showStatusBar = reader.ReadBoolean();
                            */
                            int size = MapSize.size;

                            screens = new BitArray[size];

                            int byteSize = MapSize.byteSize;
                            for (int i = 0; i < size; i++)
                            {
                                screens[i] = new BitArray(reader.ReadBytes(byteSize));
                            }
                        }
                    }

                }
                else
                {
                    //Debug.Log("screenSet");
                    saveScreen = true;
                    int size = MapSize.size;
                    if (screens.Length != size)
                    {
                        screens = new BitArray[size];
                    }


                    for (int i = 0; i < size; i++)
                    {
                        screens[i] = new BitArray(size, false);
                    }

                }

            }

        }



        //Mapを生成するときに同時にScreenの処理もするかどうか
        if (screenValid)
        {
            screenCount = 0;
            var screenValidPub = GlobalMessagePipe.GetPublisher<ScreenValidMessage>();
            screenValidPub.Publish(new ScreenValidMessage());

            //Messageを受け取る


            //test++;
            //Debug.Log(test);
            var roadPub = GlobalMessagePipe.GetPublisher<MapRoadMessage>();
            var screenPub = GlobalMessagePipe.GetPublisher<ScreenDrawMessage>();

            for (int i = 0; i < vertical.Count; i++)
            {
                var horizontal = vertical[i].horizontal;
                //Debug.Log(horizontal.Count);
                for (int j = 0; j < horizontal.Count; j++)
                {
                    //壁か道か
                    if (horizontal[j])
                    {
                        roadPub.Publish(new MapRoadMessage(new DungeonPos(i, j)));
                    }

                    //Screenが有効か否か
                    //var posa = new DungeonPos(i, j);
                    //Debug.Log("x: " + posa.x + ", y :" + posa.y + ", bool:" + screens[i][j]);
                    if (!screens[i][j])
                    {
                        var pos = new DungeonPos(i, j);
                        screenPub.Publish(new ScreenDrawMessage(pos));
                        _ = new ScreenDisposableAdmin(j, i);
                        screenCount++;
                    }
                    
                }
            }
        }
        else
        {
            var roadPub = GlobalMessagePipe.GetPublisher<MapRoadMessage>();
            for (int i = 0; i < vertical.Count; i++)
            {
                var horizontal = vertical[i].horizontal;
                for (int j = 0; j < horizontal.Count; j++)
                {
                    //壁か道か
                    if (horizontal[j])
                    {
                        roadPub.Publish(new MapRoadMessage(new DungeonPos(i, j)));
                    }
                }
            }
        }

        SetComponent();
        var drawPub = GlobalMessagePipe.GetPublisher<DungeonDrawMessage>();
        drawPub.Publish(new DungeonDrawMessage());
    }


    public void SetComponent()
    {
        //var span = CollectionsMarshal.AsSpan(addition);





        var bag = DisposableBag.CreateBuilder();

        //Debug.Log(encount);
        encount.ISetEncount(bag);
        foreach (var item in addition)
        {
            if (!item.valid)
            {
                continue;
            }
            item.SetValid();
        }

        var resetSub = GlobalMessagePipe.GetSubscriber<ResetMapMessage>();


        if (screenValid)
        {
            resetSub.Subscribe(get =>
            {
                //Debug.Log("okok");
                disposable?.Dispose();
                if (screenCount < 1)
                {
                    screenValid = false;
                    save = true;
                }
            }).AddTo(bag);

            var trueSub = GlobalMessagePipe.GetSubscriber<ScreenTrueMessage>();
            trueSub.Subscribe(get =>
            {
                screenCount--;
                //mapScreen[get.pos.x][get.pos.y] = true;
            }).AddTo(bag);
        }
        else
        {
            resetSub.Subscribe(get =>
            {
                //Debug.Log(screenCount);
                disposable?.Dispose();
            }).AddTo(bag);
        }




        disposable = bag.Build();
    }

    public bool IGetWallBool(DungeonPos pos)
    {
        return vertical[pos.y].horizontal[pos.x];
    }

    //どこから読んでいるか
    //DungeonPostinoHolderの移動処理 => ScreenDisposableAdmin (ScreenTrueMessage)=> MSO_DungeonMapHolderSO
    public void IChangeScreen(DungeonPos pos)
    {
        //secondMapでsaveして始めるとfirstMapに移ったときにエラーを吐き出す
        //DungeonDrawerのDraw処理をUniTaskのasyncに変更し，map変更時にcts.Cancelを行うことでエラーのタイミングがゲーム終了時に変わった。
        //UniTaskのエラーハンドリングを忘れていた
        //=>ハンドリングしてもエラーが発生

        //firstlyでscreensに入れる前のnullのタイミングでここに入ってきてしまっている
        //=>*******
        //=>DungeonMapHolderのMapを受け取ったタイミングでdisposableSaveScreenのDispose()を忘れていた

        //Debug.Log("x:" + pos.x + ",y: "+ pos.y);
        //Debug.Log(screens[pos.x][pos.y]);
        //ここをなくすと発生しなくなるのは確認
        //Debug.Log(screens.Length);
        //Debug.Log(screens[pos.x]?.Length);
        //Debug.Log("changeSc");
        /*
        if (screens[pos.y] == null)
        {
            Debug.Log("null");
        }
        */

        screens[pos.y][pos.x] = true;


    }

    public IDisposable ISetScreenSave()
    {
        //Debug.Log("screenSaveSet");
        saveScreen = true;
        var quitSub = GlobalMessagePipe.GetSubscriber<SaveCallMessage>();
        var d = quitSub.Subscribe(get =>
        {
            SaveScreens();
        });
        return d;

    }
    public IDisposable ISetComponentSave()
    {
        save = true;
        var quitSub = GlobalMessagePipe.GetSubscriber<SaveCallMessage>();
        var d = quitSub.Subscribe(get =>
        {
            SaveBooleans();
        });
        return d;
    }

    //screen用
    //private void


    private void SaveBooleans()
    {
        if (save)
        {
            if(saveScreen)
            {
                if (screenCount < 1)
                {
                    screenValid = false;
                }
            }
            //Debug.Log("saveBool");
            var path = "Assets/BinaryData/" + name + "MapBooleans.bytes";
            using (var stream = File.Open(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(screenValid);
                    foreach (var item in addition)
                    {
                        writer.Write(item.valid);
                    }
                }
            }
        }
        
    }
    private void SaveScreens()
    {
        if (saveScreen)
        {
            saveScreen = false;
            Debug.Log("saveScreen");

            var path = "Assets/BinaryData/" + name + "MapScreen.bytes";
            using (var stream = File.Open(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    byte[] byteData = new byte[2];

                    foreach (var screen in screens)
                    {
                        screen.CopyTo(byteData, 0);
                        writer.Write(byteData);
                    }
                }
            }

        }
    }

    //newGameの記録はNewGameControllSOで行っている
    public void NewGameSet()
    {
        newGame = true;

#if UNITY_EDITOR
        foreach (var item in addition)
        {
            item.Initialize();
        }

        //once
        var path = "Assets/BinaryData/" + name + "MapBooleans.bytes";
        using (var stream = File.Open(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(screenValid);
                    foreach (var item in addition)
                    {
                        writer.Write(item.valid);
                    }
                }
            }
#endif
    }




    //ScriptableObjectはbuild後は起動ごとに初期化される
    //Editor上での
    /*
#if UNITY_EDITOR
    private void SaveNewGame()
    {
        var path = "Assets/BinaryData/" + name + "MapBooleans.bytes";
        using (var stream = File.Open(path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(newGame);
                    writer.Write(screenValid);
                    foreach (var item in addition)
                    {
                        writer.Write(item.valid);
                    }
                }
            }
    }
#endif
    */
}

