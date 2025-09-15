using System;
using UnityEngine;



[Serializable]
public class AchieveData : MonoBehaviour
{
    [SerializeField] public float duration;
    [SerializeField] public Sprite image;
    [SerializeField] public string type;
    [SerializeField] public string title;
    [SerializeField] private string desc;
    public string Desc => desc.Replace("\\n", "\n");
}
