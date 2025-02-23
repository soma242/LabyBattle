using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MessagePipe;
using VContainer;
using VContainer.Unity;

public class TKeyOfMessagePipe : MonoBehaviour
{

    public int TKey;

    [Inject] private readonly IPublisher<int, int> _DownInputPublisher;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
