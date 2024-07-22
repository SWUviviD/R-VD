using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaticData
{
    [Serializable]
    public class sample : DataBase
    {
        public int YEAR { get => year; set => year = value; }
        [SerializeField] private int year;
        public int MONTH { get => month; set => month = value; }
        [SerializeField] private int month;
        public int DAY { get => day; set => day = value; }
        [SerializeField] private int day;
        public int HOUR { get => hour; set => hour = value; }
        [SerializeField] private int hour;
        public int HALF_HOUR { get => half_hour; set => half_hour = value; }
        [SerializeField] private int half_hour;

        public int LINE_ID { get => line_id; set => line_id = value; }
        [SerializeField] private int line_id;
        public string LINE_NM { get => line_nm; set => line_nm = value; }
        [SerializeField] private string line_nm;

        public int STATION_ID { get => station_id; set => station_id = value; }
        [SerializeField] private int station_id;
        public string STATION_NM { get => station_nm; set => station_nm = value; }
        [SerializeField] private string station_nm;

        public int GETON_CNT { get => getOff_cnt; set => getOff_cnt = value; }
        [SerializeField] private int getOn_cnt;
        public int GETOFF_CNT { get => getOff_cnt; set => getOff_cnt = value; }
        [SerializeField] private int getOff_cnt;
    }
}
