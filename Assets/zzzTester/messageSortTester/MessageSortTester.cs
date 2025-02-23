/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

#pragma warning disable CS4014 // disable warning


//AsyncPub��AsyncSub�ł����󂯂��Ȃ��B
public class MessageSortTester : MonoBehaviour
{
    private IAsyncPublisher<SortMessage> sortPub;
    private IAsyncSubscriber<SortValueMessage> valueSub;

    private System.IDisposable dispo;

    int result;

    private List<SortValueHolder> holderList = new List<SortValueHolder>();

    // Start is called before the first frame update
    async UniTask Awake()
    {
        result = 0;

        sortPub = GlobalMessagePipe.GetAsyncPublisher<SortMessage>();
        valueSub  = GlobalMessagePipe.GetAsyncSubscriber<SortValueMessage>();

        dispo = valueSub.Subscribe(async (get,ctos) =>
        {
            Debug.Log("valueSub");
            if (get.value > result)
            {
                result = get.value;
            }
        });

        await InstanceCreate();

        await PubComp();
        Debug.Log("result: " + result);

    }


    

    private async UniTask InstanceCreate()
    {

        for (int i = 1; i < 100; i++)
        {
            holderList.Add(new SortValueHolder(i));
        }
        await UniTask.Delay(0);
        //�S�̃C���X�^���X�����͊m�F�BSubscribe�������Ă��Ȃ��B
        //Debug.Log(holderList.Count);
    }

    private async UniTask PubComp()
    {
        //Debug.Log("pubcomp");
        await sortPub.PublishAsync(new SortMessage(), AsyncPublishStrategy.Sequential);

    }



}

public struct SortMessage { }
public struct SortValueMessage
{
    public int value;
    public SortValueMessage(int value)
    {
        this.value = value;
    }
}

public class SortValueHolder
{
    public int value;
    private IAsyncSubscriber<SortMessage> sortSub;
    private IAsyncPublisher<SortValueMessage> valuePub;
    private System.IDisposable disposable;

    public SortValueHolder(int value)
    {
        this.value = value;
        this.sortSub = GlobalMessagePipe.GetAsyncSubscriber<SortMessage>();
        this.valuePub = GlobalMessagePipe.GetAsyncPublisher<SortValueMessage>();
        //Debug.Log("new");
        this.disposable = sortSub.Subscribe(async (get,ctos) =>
        {
            //�����܂ł͓����Ă���B
            //Debug.Log(value);
            //await PublishSVM();
            await PuvComp();
        });

    }

    private async UniTask PuvComp()
    {
        await valuePub.PublishAsync(new SortValueMessage(value));
        //Debug.Log("wait");
        await UniTask.NextFrame();

    }


}
*/