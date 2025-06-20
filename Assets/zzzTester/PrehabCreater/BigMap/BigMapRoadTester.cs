using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MessagePipe;

using DungeonSceneMessage;

public class BigMapRoadTester : MonoBehaviour
{
    [SerializeField]
    private GameObject mapGrid;

    private Image image;

    //�z��̃T�C�Y��static��
    private IMapGrid[][] grids = new IMapGrid[MapSize.size][];

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        int size = MapSize.size;

        for (int i = 0; i < size; i++)
        {
            grids[i] = new IMapGrid[MapSize.size];

            for (int j = 0; j < size; j++)
            {
                var obj = Instantiate(mapGrid, transform, false);
                grids[i][j] = obj.GetComponent<BigMapGridTester>();

                //i��y�Cj��x�Ȃ̂ōl�����ēn��
                grids[i][j].InitiatePosition(j, i);
                //Debug.Log("aa");

            }
        }

        image = GetComponent<Image>();

        //var updateSub = GlobalMessagePipe.GetSubscriber<MiniMapUpdateMessage>();
        var roadSub = GlobalMessagePipe.GetSubscriber<MapRoadMessage>();


        var bag = DisposableBag.CreateBuilder();

        roadSub.Subscribe(get =>
        {
            grids[get.pos.x][get.pos.y].SetGridState();
        }).AddTo(bag);




        disposableOnDestroy = bag.Build();
    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }



}
