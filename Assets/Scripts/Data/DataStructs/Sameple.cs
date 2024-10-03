using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MemoryPack;

namespace StaticData
{
    [MemoryPackable]
    [Serializable]
    public partial class sample : DataBase
    {
         
        [MemoryPackIgnore] public Type TYPE { get => type; set => type = value; }
        [MemoryPackInclude] [SerializeField] private Type type;
        [MemoryPackIgnore] public int YEAR { get => year; set => year = value; }
        [MemoryPackInclude] [SerializeField] private int year;
        [MemoryPackIgnore] public int MONTH { get => month; set => month = value; }
        [MemoryPackInclude] [SerializeField] private int month;
        [MemoryPackIgnore] public int DAY { get => day; set => day = value; }
        [MemoryPackInclude] [SerializeField] private int day;
        [MemoryPackIgnore] public int HOUR { get => hour; set => hour = value; }
        [MemoryPackInclude] [SerializeField] private int hour;
        [MemoryPackIgnore] public int HALF_HOUR { get => half_hour; set => half_hour = value; }
        [MemoryPackInclude] [SerializeField] private int half_hour;

        [MemoryPackIgnore] public int LINE_ID { get => line_id; set => line_id = value; }
        [MemoryPackInclude] [SerializeField] private int line_id;
        [MemoryPackIgnore] public string LINE_NM { get => line_nm; set => line_nm = value; }
        [MemoryPackInclude] [SerializeField] private string line_nm;

        [MemoryPackIgnore] public int STATION_ID { get => station_id; set => station_id = value; }
        [MemoryPackInclude] [SerializeField] private int station_id;
        [MemoryPackIgnore] public string STATION_NM { get => station_nm; set => station_nm = value; }
        [MemoryPackInclude] [SerializeField] private string station_nm;

        [MemoryPackIgnore] public int GETON_CNT { get => getOff_cnt; set => getOff_cnt = value; }
        [MemoryPackInclude] [SerializeField] private int getOn_cnt;
        [MemoryPackIgnore] public int GETOFF_CNT { get => getOff_cnt; set => getOff_cnt = value; }
        [MemoryPackInclude] [SerializeField] private int getOff_cnt;

        [Serializable]
        public enum Type
        {
            Type1,
            Type2,
        }

        public sample()
        {

        }

        [MemoryPackConstructor]
        public sample(int id, string id_str, Type type, int year,int month, int day, int hour, int half_hour, 
            int line_id, string line_nm, int station_id, string station_nm, int getOn_cnt, int getOff_cnt)
        {
            this.ID = id;
            this.ID_str = id_str;
            this.type = type;
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.half_hour = half_hour;
            this.line_id = line_id;
            this.line_nm = line_nm;
            this.station_id = station_id;
            this.station_nm = station_nm;
            this.getOn_cnt = getOn_cnt;
            this.getOff_cnt = getOff_cnt;
        }
    }
}
