using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MessagePipe;

using DungeonSceneMessage;



public class BigMapScreenController : MonoBehaviour
{
    [SerializeField]
    private GameObject mapScreen;

    //private Image image;
    private Canvas canvas;



    //�z��̃T�C�Y��static��
    private IMapScreen[][] screens = new IMapScreen[MapSize.size][];

    private System.IDisposable disposableOnDestroy;

    void Awake()
    {
        int size = MapSize.size;

        for (int i = 0; i < size; i++)
        {
            screens[i] = new IMapScreen[MapSize.size];

            for (int j = 0; j < size; j++)
            {
                var obj = Instantiate(mapScreen, transform, false);
                screens[i][j] = obj.GetComponent<BigMapScreen>();

                //i��y�Cj��x�Ȃ̂ōl�����ēn��
                screens[i][j].InitiatePosition(i, j);
                //Debug.Log("aa");

            }
        }

        //image = GetComponent<Image>();
        canvas = GetComponent<Canvas>();

        //var updateSub = GlobalMessagePipe.GetSubscriber<MiniMapUpdateMessage>();
        var screenSub = GlobalMessagePipe.GetSubscriber<ScreenDrawMessage>();


        var bag = DisposableBag.CreateBuilder();

        screenSub.Subscribe(get =>
        {
            //Debug.Log("grid pos: " + get.pos.x+","+get.pos.y);
            screens[get.pos.y][get.pos.x].SetScreenState();
        }).AddTo(bag);

        //Pub���Ŋ���Sub���邩�ǂ����̎d����������ł���
        var trueSub = GlobalMessagePipe.GetSubscriber<ScreenTrueMessage>();
        trueSub.Subscribe(get =>
        {
            screens[get.pos.x][get.pos.y].ResetScreenState();
        }).AddTo(bag);

        disposableOnDestroy = bag.Build();



    }

    void OnDestroy()
    {
        disposableOnDestroy?.Dispose();
    }



}

