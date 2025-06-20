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

//Component�̎�ށC ���݂̗L���C �ݒu�ʒu�����
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
                //Save�̃g���K�[���I���ɂ���
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
    //true��Ԃ��ƁC�d�Ȃ�������valid��false�ɂȂ�
    public abstract bool OverLapBool();
}

/// <summary>
/// Dugeon�̃}�b�v��n�����߂�Message
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


//Screen��PubSub�i�[�p��Class
//�Œ����IDisposable�ŊǗ����邽��
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
/// �{��
/// </summary>

[CreateAssetMenu(menuName = "dungeonScene/mapData")]
public class DungeonMapDataSO : ScriptableObject, IDungeonMapDataPicker
{
    public int mapKey;

    public List<DungeonSquare> vertical = new List<DungeonSquare>();
    //�_���W�������̗v�f
    public List<DungeonComponentData> addition = new List<DungeonComponentData>();

    //interface�Ɉڂ�
    public EncounterEnemySO encountReference;
    public IEncounterEnemy encount;

    private IDisposable disposable;
    //private IDisposable disposableScreen;
    private IDisposable disposableSave;

    //���������Ȃ����Length���m�F�ł��Ȃ�
    private BitArray[] screens = new BitArray[MapSize.size];

    //[SerializeField]
    private int screenCount;
    //[SerializeField]
    private bool screenValid = true;

    //true�̏ꍇ�Ƀt�@�C���f�[�^�ǂݍ���(�Q�[���N�����珉��ǂݍ��݂̏ꍇ�j
    public bool firstly = true;
    //true�̏ꍇ�ɃQ�[���I�����Ƀt�@�C���f�[�^�ɏ�������
    //Disposable����ɂ��邽�߂�MapHolder����ύX����
    //compPub
    private bool save = false;
    //truePub
    private bool saveScreen = false;


    public bool newGame = true;


    //�`�F�b�N�������ĂȂ��̂�False(����)



    //DungeonPositionHolder => DungeonMapController����N��
    public void MiniMapGenerate()
    {
        



        //Debug.Log(firstly);
        //Debug.Log(screenValid);

        //Debug.Log(newGame);

        //NewGame���Ă���ŏ���Map�ɓ������Ȃ��
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


        //�Q�[���N������ŏ���Map�ɓ������Ȃ��
        if (firstly)
        {
            firstly = false;
            encount = encountReference;
            //Debug.Log(encountReference.name);
            encount.ISetSum();
            //comp��valid�y��screenValid�̓ǂݍ���
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



            //screen�̓ǂݍ��݁ior�������j
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



        //Map�𐶐�����Ƃ��ɓ�����Screen�̏��������邩�ǂ���
        if (screenValid)
        {
            screenCount = 0;
            var screenValidPub = GlobalMessagePipe.GetPublisher<ScreenValidMessage>();
            screenValidPub.Publish(new ScreenValidMessage());

            //Message���󂯎��


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
                    //�ǂ�����
                    if (horizontal[j])
                    {
                        roadPub.Publish(new MapRoadMessage(new DungeonPos(i, j)));
                    }

                    //Screen���L�����ۂ�
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
                    //�ǂ�����
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

    //�ǂ�����ǂ�ł��邩
    //DungeonPostinoHolder�̈ړ����� => ScreenDisposableAdmin (ScreenTrueMessage)=> MSO_DungeonMapHolderSO
    public void IChangeScreen(DungeonPos pos)
    {
        //secondMap��save���Ďn�߂��firstMap�Ɉڂ����Ƃ��ɃG���[��f���o��
        //DungeonDrawer��Draw������UniTask��async�ɕύX���Cmap�ύX����cts.Cancel���s�����ƂŃG���[�̃^�C�~���O���Q�[���I�����ɕς�����B
        //UniTask�̃G���[�n���h�����O��Y��Ă���
        //=>�n���h�����O���Ă��G���[������

        //firstly��screens�ɓ����O��null�̃^�C�~���O�ł����ɓ����Ă��Ă��܂��Ă���
        //=>*******
        //=>DungeonMapHolder��Map���󂯎�����^�C�~���O��disposableSaveScreen��Dispose()��Y��Ă���

        //Debug.Log("x:" + pos.x + ",y: "+ pos.y);
        //Debug.Log(screens[pos.x][pos.y]);
        //�������Ȃ����Ɣ������Ȃ��Ȃ�̂͊m�F
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

    //screen�p
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

    //newGame�̋L�^��NewGameControllSO�ōs���Ă���
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




    //ScriptableObject��build��͋N�����Ƃɏ����������
    //Editor��ł�
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

