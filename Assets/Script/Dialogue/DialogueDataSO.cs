using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
    [Header("Text")]
    [TextArea(3, 10)]
    public string content;

    [Header("Extend Text Data")]
    [Tooltip("如果这是关键句，拖入下一段主线对话。如果是普通句，留空。")]
    public DialogueData nextStageDialogue;
}
