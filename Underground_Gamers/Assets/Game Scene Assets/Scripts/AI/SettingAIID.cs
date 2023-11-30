using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingAIID : MonoBehaviour
{
    public AIManager aiManager;
    private List<Color> colors = new List<Color>();
    public float outlineWidth;

    public void Awake()
    {
        int pcID = 1;
        int npcID = 1;
        colors.Add(new Color(0, 0, 1));
        colors.Add(new Color(0, 1, 0));
        colors.Add(new Color(1, 0.41f, 0.71f));
        colors.Add(new Color(0.5f, 0, 0.5f));
        colors.Add(new Color(1, 0.92f, 0.016f));
        foreach (var ai in aiManager.pc)
        {
            ai.aiType.outlineWidth = outlineWidth;
            ai.aiType.outlineColor = Color.white;
            ai.aiType.color = colors[pcID - 1];
            ai.colorIndex = pcID - 1;
            ai.aiType.text = $"PC{pcID++}";
        }
        foreach (var ai in aiManager.npc)
        {
            ai.aiType.outlineWidth = outlineWidth;
            ai.aiType.outlineColor = Color.black;
            ai.aiType.color = colors[npcID - 1];
            ai.colorIndex = npcID - 1;
            ai.aiType.text = $"NPC{npcID++}";
        }
    }
}