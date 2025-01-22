using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Container", menuName = "Scriptable Object/Dialogue")]
public class DialogueContainer : ScriptableObject
{
    public List<Chat> Dialogues = new List<Chat>();

    [System.Serializable]
    public struct Chat
    {
        public int DialogueID;

        public string NpcName;

        [TextArea(5, 10)]
        public string[] Sentences;
    }
}
