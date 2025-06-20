using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;


using MessagePipe;

using DungeonSceneMessage;
using Cysharp.Threading.Tasks;
#pragma warning disable CS4014 // disable warning
#pragma warning disable CS1998 // disable warning
public static class MapSize
{
    //変更したらBigMap内のGridサイズ, byteSizeを変更
    public static int size = 12;

    //size/2の切り上げ
    public static int byteSize = 2;

    public static int bigMapGridWidth = 60;
}

public class DungeonMapController : MonoBehaviour//, IPointerClickHandler
{
    //private int listNum;

    [SerializeField]
    private Image moveFrame;

    [SerializeField]
    private MSO_DungeonMapHolderSO holder;
    [SerializeField]
    private MSO_DungeonPositionHolderSO posHolder;

    [SerializeField]
    private GameObject mapGrid;

    [SerializeField]
    private GameObject compGroup;
    [SerializeField]
    private GameObject downStairs;
    private List<MiniMapDownStairs> downStairsCatalog = new List<MiniMapDownStairs>();
    private List<MiniMapDownStairs> upStairsCatalog = new List<MiniMapDownStairs>();

    //private Image image;

    //配列のサイズはstaticで
    private IMapGrid[][] grids = new IMapGrid[MapSize.size][];



    //private ISubscriber<MiniMapUpdateMessage> updateSub;

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        int size = MapSize.size;

        for(int i = 0; i < size; i++)
        {
            grids[i] = new IMapGrid[MapSize.size];

            for(int j = 0; j < size; j++)
            {
                var obj = Instantiate(mapGrid, transform, false);
                grids[i][j] = obj.GetComponent<DungeonMapGrid>(); 
                //iがy，jがxなので考慮して渡す
                grids[i][j].InitiatePosition(j, i);
            }
        }

        //image = GetComponent<Image>();

        var updateSub = GlobalMessagePipe.GetSubscriber<MiniMapUpdateMessage>();
        var posSub = GlobalMessagePipe.GetSubscriber<PosChangeMessage>();

        var bag = DisposableBag.CreateBuilder();

        updateSub.Subscribe(get =>
        {
            DungeonMapSet();
        }).AddTo(bag);


        //miniMapの位置調整
        posSub.Subscribe(get =>
        {
            moveFrame.rectTransform.anchoredPosition = new Vector2(posHolder.currentPos.x * -40, posHolder.currentPos.y * 40); 
        }).AddTo(bag);

        var mapSub = GlobalMessagePipe.GetSubscriber<DungeonMapMessage>();
        mapSub.Subscribe(get =>
        {
            moveFrame.rectTransform.anchoredPosition = new Vector2(posHolder.currentPos.x * -40, posHolder.currentPos.y * 40);

        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }

    /*
    public async void OnPointerClick(PointerEventData pointerEventData)
    {
        var expandPub = GlobalMessagePipe.GetPublisher<MapExpandMessage>();
        expandPub.Publish(new MapExpandMessage());

    }
    */

    private async UniTask DungeonMapSet()
    {
        var roadSub = GlobalMessagePipe.GetSubscriber<MapRoadMessage>();
        var bag = DisposableBag.CreateBuilder();
        roadSub.Subscribe(get =>
        {
            grids[get.pos.x][get.pos.y].SetGridState();
        }).AddTo(bag);

        var downStairSub = GlobalMessagePipe.GetSubscriber<DownStairsSetMessage>();
        int downStairsNum = 0;
        downStairSub.Subscribe(get =>
        {
            if(downStairsNum >= downStairsCatalog.Count)
            {
                var obj = Instantiate(downStairs, compGroup.transform, false);
                var stairs = obj.GetComponent<MiniMapDownStairs>();
                stairs.SetPosition(get.pos);
                stairs.SetSubscriber();

                downStairsCatalog.Add(stairs);
            }
            else
            {
                //Debug.Log(listNum + ""+ downStairsCatalog.Count);
                downStairsCatalog[downStairsNum].SetStairs(get.pos);

            }
            downStairsNum++;
        }).AddTo(bag);

        var upStairsSub = GlobalMessagePipe.GetSubscriber<UpStairsSetMessage>();
        int upStairsNum = 0;
        upStairsSub.Subscribe(get =>
        {
            if (upStairsNum >= upStairsCatalog.Count)
            {
                var obj = Instantiate(downStairs, transform, false);
                var stairs = obj.GetComponent<MiniMapDownStairs>();
                stairs.SetPosition(get.pos);
                stairs.SetSubscriber(); stairs.transform.Rotate(0, 0, -180f);

                upStairsCatalog.Add(stairs);
            }
            else
            {
                //Debug.Log(listNum + "" + upStairsCatalog.Count);
                downStairsCatalog[upStairsNum].SetStairs(get.pos);

            }
            upStairsNum++;
        }).AddTo(bag);

        var disposable = bag.Build();

        var resetPub = GlobalMessagePipe.GetPublisher<ResetMapMessage>();
        resetPub.Publish(new ResetMapMessage());
        await UniTask.NextFrame();

        holder.currentMap.MiniMapGenerate();
        disposable?.Dispose();
    }
}
