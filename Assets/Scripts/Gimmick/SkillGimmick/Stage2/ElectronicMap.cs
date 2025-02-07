using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronicMap : MonoBehaviour
{
    [SerializeField] private int mapIndex;

    [SerializeField] private Transform pinParent;
    [SerializeField] private GameObject pinPrefab;

    [SerializeField] private List<List<ElectronicPin>> pinList = new List<List<ElectronicPin>>();
    [SerializeField] private float PosOffset = 4.5f;

    [Serializable]
    private class AttachedElectronicObj
    {
        public ShockableObj obj;
        public int X;
        public int Y;
        public int Dir;
    }

    [SerializeField] private AttachedElectronicObj[] attached;
    private int startObj;

    private static bool[,] PIN_DIR = new bool[4, 4] {
        {true, true, true, true},   // Plus
        {false, true, false, true},   // Line
        {true, false, false, true},   // Curve
        {true, false, false, true}    // LongCurve
    };

    private static int[,] PIN_POS = new int[4, 2]
    {
        {0, 1 },   // Forward
        {1, 0 },   // Left
        {0, -1 },   // Backward
        {-1, 0 }    // Right
    };

    private static List<List<LDPinMapData>> MapDatas;
    private static void SetMapDatas()
    {
        MapDatas = new List<List<LDPinMapData>>();

        List<LDPinMapData> mapDatas;
        SerializeManager.Instance.LoadDataFile(out mapDatas, "LDPinMapData");
        if (mapDatas.Count <= 0)
            return;

        int index = -1;
        foreach(var data in mapDatas)
        {
            if(data.Index != index)
            {
                MapDatas.Add(new List<LDPinMapData>());
                index = data.Index;
            }

            MapDatas[index].Add(data);
        }
    }

    public void Awake()
    {
        if(MapDatas == null)
        {
            SetMapDatas();
        }

        Init();
    }

    private void Init()
    {
        if (MapDatas.Count == 0) return;
        if(mapIndex < 0 ||  mapIndex >= MapDatas.Count) return;

        List<LDPinMapData> mapDatas = MapDatas[mapIndex];

        int x = -1;
        foreach (var mapData in mapDatas)
        {
            if(x != mapData.X)
            {
                pinList.Add(new List<ElectronicPin>());
                x = mapData.X;
            }

            GameObject pin = Instantiate(pinPrefab, pinParent);
            ElectronicPin pinScript = pin.GetComponent<ElectronicPin>();
            
            pinList[x].Add(pinScript);
            pinScript.Init(this, mapData);

            pin.transform.localPosition = 
                Vector3.right * PosOffset * mapData.X + 
                Vector3.forward * PosOffset * mapData.Y;
        }

        for(int i = 0; i < attached.Length; ++i)
        {
            attached[i].obj.SetForMap(this, i);
        }
    }
    
    public bool CheckIfStillActive(ElectronicPin pin, ShockableObj shockFailObj)
    {
        if (shockFailObj == pin.GiveShockObj)
            return false;

        int x = pin.Data.X;
        int y = pin.Data.Y;

        int type = pin.Data.Type;
        int dir = (int)pin.CurrentDir;

        for(int i = 0; i < 4; ++i)
        {
            if (PIN_DIR[type, (dir + i) % 4] == false)
                continue;

            if(x + PIN_POS[i, 0] >= 0 && x + PIN_POS[i, 0] < pinList.Count
                && y + PIN_POS[i, 1] >= 0 && y + PIN_POS[i, 1] < pinList[0].Count)
            {
                ElectronicPin nextPin = pinList[x + PIN_POS[i, 0]][y + PIN_POS[i, 1]];
                if (pin.GiveShockObj != nextPin && (nextPin.CurrentState == ElectronicPin.State.Active))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckIfValid(ElectronicPin pin, int fromDir)
    {
        int x = pin.Data.X;
        int y = pin.Data.Y;

        int type = pin.Data.Type;
        int dir = (int)pin.CurrentDir;

        return PIN_DIR[type, (dir + (fromDir + 2)) % 4];
    }

    public void ShockPin(int index)
    {
        AttachedElectronicObj obj = attached[index];

        int x = obj.X + PIN_POS[obj.Dir, 0];
        int y = obj.Y + PIN_POS[obj.Dir, 1];

        pinList[x][y].OnShocked(obj.obj);
    }

    public void ShockFail(int index)
    {
        AttachedElectronicObj obj = attached[index];

        int x = obj.X + PIN_POS[obj.Dir, 0];
        int y = obj.Y + PIN_POS[obj.Dir, 1];

        pinList[x][y].ShockFailed(obj.obj);
    }

    public void ShockNextPin(ElectronicPin pin)
    {
        int x = pin.Data.X;
        int y = pin.Data.Y;

        int type = pin.Data.Type;
        int dir = (int)pin.CurrentDir;

        bool connected = false;
        for (int i = 0; i < 4; ++i)
        {
            if (PIN_DIR[type, (dir + i) % 4] == false)
                continue;

            if (x + PIN_POS[i, 0] >= 0 && x + PIN_POS[i, 0] < pinList.Count
                && y + PIN_POS[i, 1] >= 0 && y + PIN_POS[i, 1] < pinList[0].Count)
            {
                ElectronicPin nextPin = pinList[x + PIN_POS[i, 0]][y + PIN_POS[i, 1]];
                if(nextPin.CurrentState == ElectronicPin.State.Inactive && CheckIfValid(nextPin, i))
                {
                    connected = true;
                    nextPin.OnShocked(pin);
                }
            }
        }

        for(int i = 0; i < attached.Length; ++i)
        {
            AttachedElectronicObj obj = attached[i];
            if (obj.obj == pin.GiveShockObj)
                continue;

            if ((obj.X + PIN_POS[obj.Dir, 0]) == x &&
                (obj.Y + PIN_POS[obj.Dir, 1]) == y)
            {
                connected = true;
                obj.obj.OnShocked(pin);
            }
        }

        if (connected == false)
        {
            pin.ShockFailed();
        }
    }

    public void ShockFailNextPin(ElectronicPin pin)
    {
        int x = pin.Data.X;
        int y = pin.Data.Y;

        int type = pin.Data.Type;
        int dir = (int)pin.CurrentDir;

        for (int i = 0; i < 4; ++i)
        {
            if (PIN_DIR[type, (dir + i) % 4] == false)
                continue;

            if (x + PIN_POS[i, 0] >= 0 && x + PIN_POS[i, 0] < pinList.Count
                && y + PIN_POS[i, 1] >= 0 && y + PIN_POS[i, 1] < pinList[0].Count)
            {
                ElectronicPin nextPin = pinList[x + PIN_POS[i, 0]][y + PIN_POS[i, 1]];
                if (nextPin.CurrentState == ElectronicPin.State.Active && CheckIfValid(nextPin, i))
                {
                    nextPin.ShockFailed(pin);
                }
            }
        }

        foreach (var obj in attached)
        {
            if ((obj.X + PIN_POS[obj.Dir, 0]) == x && 
                (obj.Y + PIN_POS[obj.Dir, 1]) == y)
            {
                obj.obj.ShockFailed(pin);
            }
        }
    }
}
