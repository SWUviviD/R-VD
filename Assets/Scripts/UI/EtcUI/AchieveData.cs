using System;
using UnityEngine;
using UnityEngine.Events;



[Serializable]
public class AchieveData : MonoBehaviour
{
    [field: SerializeField] public float duration { get; set; }
    [field: SerializeField] public Sprite image { get; set; }
    [field: SerializeField] public string type { get ; set; }
    [field: SerializeField] public string title {  get; set; }
    [SerializeField, TextArea(3, 4)] private string desc;
    [field: SerializeField] public UnityEvent callback {  get; set; }
    public string Desc => desc.Replace("\\n", "\n");
}
