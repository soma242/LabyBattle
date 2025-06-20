using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using DungeonSceneMessage;

public class BigMapCompController : MonoBehaviour
{
    [SerializeField]
    private GameObject downStairs;


    private List<BigMapDownStairs> downStairsCatalog = new List<BigMapDownStairs>();
    private int downStairsNum = 0;
    
    private List<BigMapDownStairs> upStairsCatalog = new List<BigMapDownStairs>();
    private int upStairsNum = 0;

    private System.IDisposable disposableOnDestroy;

    //Componentのサイズと，その親のサイズを合わせる
    //=>そうしないと反転時に位置がずれてしまう

    void Awake()
    {
        var bag = DisposableBag.CreateBuilder();

        var downStairsSub = GlobalMessagePipe.GetSubscriber<DownStairsSetMessage>();
        downStairsSub.Subscribe(get =>
        {
            if (downStairsNum >= downStairsCatalog.Count)
            {
                var obj = Instantiate(downStairs, transform, false);
                var stairs = obj.GetComponent<BigMapDownStairs>();
                stairs.SetDownStairs(get.pos);

                downStairsCatalog.Add(stairs);
            }
            else
            {
                //Debug.Log(listNum + "" + downStairsCatalog.Count);
                downStairsCatalog[downStairsNum].SetDownStairs(get.pos);

            }
            downStairsNum++;
        }).AddTo(bag);

        var upStairsSub = GlobalMessagePipe.GetSubscriber<UpStairsSetMessage>();
        upStairsSub.Subscribe(get =>
        {
            if (upStairsNum >= upStairsCatalog.Count)
            {
                var obj = Instantiate(downStairs, transform, false);
                var stairs = obj.GetComponent<BigMapDownStairs>();
                stairs.SetDownStairs(get.pos);
                stairs.transform.Rotate(0, 0, -180f);

                upStairsCatalog.Add(stairs);
            }
            else
            {
                //Debug.Log(listNum + "" + upStairsCatalog.Count);
                upStairsCatalog[upStairsNum].SetDownStairs(get.pos);

            }
            upStairsNum++;
        }).AddTo(bag);


        var resetSub = GlobalMessagePipe.GetSubscriber<MiniMapResetMessage>();
        resetSub.Subscribe(get =>
        {
            downStairsNum = 0;
            upStairsNum = 0;
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();
    }
    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }
}
