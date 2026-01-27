using StaticData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

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
        public enum DirType
        {
            Left,
            Forward,
            Right,
            Backward
        }

        public ShockableObj obj;
        public int X;
        public int Y;
        [SerializeField] private DirType dir;
        public int Dir => (int)dir;
    }

    [SerializeField] private AttachedElectronicObj[] attached;
    private int startObj;
    
    private enum PinType
    {
        Plus,
        Line,
        Curve,
        LongCurve,
        MAX
    }

    private static bool[,] PIN_DIR = new bool[4, 4] {
        {true, true, true, true},   // Plus
        {true, false, true, false},   // Line
        {true, true, false, false},   // Curve
        {true, true, false, false},    // LongCurve
    };

    private static int[,] PIN_POS = new int[4, 2]
    {
        {-1, 0 },   // Left
        {0, 1 },   // Forward
        {1, 0 },   // Right
        {0, -1 }    // Backward
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

        int maxX = -1;
        int maxY = -1;
        foreach (var data in mapDatas)
        {
            if (data.X > maxX) maxX = data.X;
            if (data.Y > maxY) maxY = data.Y;
        }

        pinList.Clear();
        for(int x = 0; x <= maxX; ++x)
        {
            var l = new List<ElectronicPin>();
            for(int y = 0; y <= maxY; ++y)
            {
                l.Add(null);
            }
            pinList.Add(l);
        }

        foreach (var mapData in mapDatas)
        {
            GameObject pin = Instantiate(pinPrefab, pinParent);
            ElectronicPin pinScript = pin.GetComponent<ElectronicPin>();

            pin.name = $"Pin({mapData.X}/{mapData.Y})";
            pinScript.Init(this, mapData);

            pin.transform.localPosition = 
                Vector3.right * PosOffset * mapData.X + 
                Vector3.forward * PosOffset * mapData.Y;


            pinList[mapData.X][mapData.Y] = pinScript;
        }

        for(int i = 0; i < attached.Length; ++i)
        {
            attached[i].obj.SetForMap(this, i);
        }
    }
    
    public bool CheckIfStillActiveAfterThisShockFail(ElectronicPin pin, ShockableObj shockFailObj)
    {
        // 단 방향은 무조건 shockFail
        switch((PinType) pin.Data.Type)
        {
            case PinType.Line:
            case PinType.Curve:
            case PinType.LongCurve:
                return false;
        }

        int x = pin.Data.X;
        int y = pin.Data.Y;

        int type = pin.Data.Type;
        int dir = (int)pin.CurrentDir;

        int connectedCount = 0;

        for (int i = 0; i < 4; ++i)
        {
            if (HasConnection(pin, i) == false)
                continue;

            int nx = x + PIN_POS[i, 0];
            int ny = y + PIN_POS[i, 1];

            if (nx < 0 || nx >= pinList.Count ||
                ny < 0 || ny >= pinList[0].Count)
                continue;

            ElectronicPin nextPin = pinList[nx][ny];

            if (nextPin != shockFailObj && IsConnectedBetween(pin, nextPin, i) && 
                nextPin.CurrentState == ElectronicPin.State.Active)
            {
                ++connectedCount;
                if (connectedCount > 1)
                    return true;
            }
        }

        return false;
    }

    private ShockableObj GetSourcePin(ElectronicPin pin, ShockableObj shockFailObj)
    {
        ShockableObj parent = pin.PowerSourceObj;
        if (parent == shockFailObj)
        {
            return parent;
        }

        while (parent is ElectronicPin && 
            (parent as ElectronicPin).Data.Type == (int)PinType.Plus &&
            (parent as ElectronicPin).CurrentState == ElectronicPin.State.Active)
        {
            parent = parent.PowerSourceObj;
        }

        return parent;
    }

    public bool CheckIfValid(ElectronicPin pin, int fromDir)
    {
        return HasConnection(pin, (fromDir + 2) % 4);
    }

    public void ShockPinFromOutside(int index)
    {
        AttachedElectronicObj obj = attached[index];

        int x = obj.X + PIN_POS[obj.Dir, 0];
        int y = obj.Y + PIN_POS[obj.Dir, 1];

        var nextPin = pinList[x][y];

        int opp = (obj.Dir + 2) % 4;
        if (HasConnection(nextPin, opp) == false)
        {
            obj.obj.ShockFailed();
            return;
        }
        
        nextPin.OnShocked(obj.obj);
    }

    public void ShockFailFromOutside(int index)
    {
        AttachedElectronicObj obj = attached[index];

        int x = obj.X + PIN_POS[obj.Dir, 0];
        int y = obj.Y + PIN_POS[obj.Dir, 1];

        if (x < 0 || x >= pinList.Count ||
            y < 0 || y >= pinList[0].Count)
            return;

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
            if (HasConnection(pin, i) == false)
                continue;

            int nx = x + PIN_POS[i, 0];
            int ny = y + PIN_POS[i, 1];

            if (nx < 0 || nx >= pinList.Count ||
                ny < 0 || ny >= pinList[0].Count)
                continue;

            ElectronicPin nextPin = pinList[nx][ny];

            if(nextPin.CurrentState == ElectronicPin.State.Inactive &&
                IsConnectedBetween(pin, nextPin, i))
            {
                connected = true;
                nextPin.OnShocked(pin);
            }
        }

        for(int i = 0; i < attached.Length; ++i)
        {
            AttachedElectronicObj obj = attached[i];
            if (obj.obj == pin.PowerSourceObj)
                continue;

            if ((obj.X + PIN_POS[obj.Dir, 0]) == x &&
                (obj.Y + PIN_POS[obj.Dir, 1]) == y &&
                IsConnectedBetween(pin, obj))
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
            if (HasConnection(pin, i) == false)
                continue;

            int nx = x + PIN_POS[i, 0];
            int ny = y + PIN_POS[i, 1];

            if (nx < 0 || nx >= pinList.Count ||
                ny < 0 || ny >= pinList[0].Count)
                continue;

            ElectronicPin nextPin = pinList[nx][ny];

            if (nextPin.CurrentState == ElectronicPin.State.Active
                && IsConnectedBetween(pin, nextPin, i))
            {
                nextPin.ShockFailed(pin);
            }
        }

        foreach (var obj in attached)
        {
            if ((obj.X + PIN_POS[obj.Dir, 0]) == x &&
                (obj.Y + PIN_POS[obj.Dir, 1]) == y &&
                IsConnectedBetween(pin, obj))
            {
                obj.obj.ShockFailed(pin);
            }
        }
    }

    public bool HasConnection(ElectronicPin pin, int worldDir)
    {
        int type = pin.Data.Type;
        int dir = (int)pin.CurrentDir;

        return PIN_DIR[type, (worldDir + 4 - dir) % 4];
    }

    private bool IsConnectedBetween(ElectronicPin fromPin, ElectronicPin toPin, int worldDir)
    {
        int opp = (worldDir + 2) % 4;
        return HasConnection(fromPin, worldDir) && HasConnection(toPin, opp);
    }
    private bool IsConnectedBetween(ElectronicPin fromPin, AttachedElectronicObj toPin)
    {
        int opp = (toPin.Dir + 2) % 4;
        return HasConnection(fromPin, opp);
    }
}
