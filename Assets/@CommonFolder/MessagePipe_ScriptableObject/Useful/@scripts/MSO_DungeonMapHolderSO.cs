using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;

using DungeonSceneMessage;








[CreateAssetMenu(menuName = "dungeonScene/mapHolder")]
public class MSO_DungeonMapHolderSO : MessageableScriptableObject
{
    
    public IDungeonMapDataPicker currentMap;




    //private ISubscriber<DungeonMapMessage> mapSub;

    //private IPublisher<DungeonDrawMessage> drawPub;

    private System.IDisposable disposableDestroy;
    private System.IDisposable disposableSaveScreen;
    private System.IDisposable disposableSaveComponent;

    private System.IDisposable disposableSave;

    public override void MessageStart()
    {
        //drawPub = GlobalMessagePipe.GetPublisher<DungeonDrawMessage>();

        var bag = DisposableBag.CreateBuilder();

        //bool test = System.Convert.ToBoolean(0);
        //Debug.Log(test);

        var screenSub = GlobalMessagePipe.GetSubscriber<ScreenValidMessage>();
        //map変更時にそのMapのscreenValidが有効だった場合にPubされる
        screenSub.Subscribe(get =>
        {
            Debug.Log("screenvalid");
            disposableSaveScreen?.Dispose();
            var trueSub = GlobalMessagePipe.GetSubscriber<ScreenTrueMessage>();
            disposableSaveScreen = trueSub.Subscribe(get =>
            {
                disposableSaveScreen?.Dispose();
                currentMap.IChangeScreen(get.pos);
                var d = currentMap.ISetScreenSave();
                disposableSave = DisposableBag.Create(disposableSave, d); // combine disposable.
                disposableSaveScreen = trueSub.Subscribe(get =>
                {
                    currentMap.IChangeScreen(get.pos);
                });
            });
        }).AddTo(bag);


        var mapSub = GlobalMessagePipe.GetSubscriber<DungeonMapMessage>();
        mapSub.Subscribe(get =>
        {
            currentMap = get.mapData;

            disposableSaveScreen?.Dispose();
            disposableSaveComponent?.Dispose();
            
            var compSub = GlobalMessagePipe.GetSubscriber<ComponentValidMessage>();
            disposableSaveComponent = compSub.Subscribe(get =>
            {
                disposableSaveComponent?.Dispose();
                var d = currentMap.ISetComponentSave();

                //disposableの結合において，そのDisposeはNull許容型でない
                if(disposableSave is not null)
                {
                    disposableSave = DisposableBag.Create(disposableSave, d); // combine disposable.
                }
                else
                {
                    disposableSave = d;
                }

            });


            //Debug.Log(get.mapData);

            //描画の開始
            //drawPub.Publish(new DungeonDrawMessage());

        }).AddTo(bag);



        var titleSub = GlobalMessagePipe.GetSubscriber<ToTitleMessage>();
        titleSub.Subscribe(get =>
        {
            disposableSaveScreen?.Dispose();
            disposableSaveComponent?.Dispose();
        }).AddTo(bag);

        disposableDestroy = bag.Build();
    }



    void OnDestroy()
    {
        disposableDestroy?.Dispose();
        disposableSaveScreen?.Dispose();
        disposableSaveComponent?.Dispose();
        disposableSave?.Dispose();
    }

}

