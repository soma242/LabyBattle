using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DungeonMapCreater : MonoBehaviour
{
    [SerializeField]
    private DungeonMapDataSO map;

    private System.IDisposable disposable;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("MapSize: " + MapSize.size);
        bool f = false;
        bool o = true;
        /*
        int size = 8;
        map.vertical = new List<DungeonSquare>(size);
        //Debug.Log("size: " +size+", current: "+ map.vertical.Capacity);
        map.vertical[0].horizontal = new List<bool> (){ f, f, f, f, f, f, f, f };
        map.vertical[1].horizontal = new List<bool> { f, f, f, f, f, f, f, f };
        map.vertical[2].horizontal = new List<bool> { f, f, f, f, f, f, f, f };
        map.vertical[3].horizontal = new List<bool> { f, f, f, f, f, f, f, f };
        map.vertical[4].horizontal = new List<bool> { f, f, f, f, f, f, f, f };
        map.vertical[5].horizontal = new List<bool> { f, f, f, f, f, f, f, f };
        map.vertical[6].horizontal = new List<bool> { f, f, f, f, f, f, f, f };
        map.vertical[7].horizontal = new List<bool> { f, f, f, f, f, f, f, f };
        */

        List<DungeonSquare> vertical = new List<DungeonSquare>
        {
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f }),
            new DungeonSquare(new List<bool> { f, o, f, f, o, o, o, f }),
            new DungeonSquare(new List<bool> { f, o, f, f, o, f, f, f }),
            new DungeonSquare(new List<bool> { f, o, f, f, o, o, o, f }),
            new DungeonSquare(new List<bool> { f, o, f, o, o, f, o, f }),
            new DungeonSquare(new List<bool> { f, o, f, f, f, f, o, f }),
            new DungeonSquare(new List<bool> { f, o, o, o, o, o, o, f }),
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f })
         };

        map.vertical = vertical;

        disposable = map.ISetComponentSave();

    }


}

/*
 example
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f }),
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f }),
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f }),
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f }),
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f }),
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f }),
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f }),
            new DungeonSquare(new List<bool> { f, f, f, f, f, f, f, f })

    third


*/