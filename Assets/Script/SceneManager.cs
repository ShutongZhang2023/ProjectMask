using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SceneManager : MonoBehaviour
{
    public CurtainController curtainController;
    public List<GameObject> npcList;

    private int currentIndex = 0;
    public static SceneManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        // 只在 Day 2 执行特殊逻辑 - 桌面清理大师
        if (GameManager.Instance != null && GameManager.Instance.currentDay == 3)
        {
            AdjustDay2NpcList();
        }        
        // 只在 Day 3 执行特殊逻辑 - 医生的悔改
        if (GameManager.Instance != null && GameManager.Instance.currentDay == 3)
        {
            AdjustDay3NpcList();
        }

        foreach (var npc in npcList)
        {
            if (npc != null) npc.SetActive(false);
        }
        currentIndex = 0;

        npcList[0].SetActive(true);
        npcList[0].GetComponent<NPCIdentity>().SendInfoToManager();
    }

    void AdjustDay2NpcList()
    {
        string checkKey = "Day1_1.2"; 
        string result = "";        
    }

    void AdjustDay3NpcList()
    {
        string checkKey = "Day2_2.2"; 
        string result = "";

        // 从 GameManager 的字典里获取结果
        if (GameManager.Instance.storyDecisions.TryGetValue(checkKey, out result))
        {
            if (result == "Success")
            {
                // 如果成功，保留 NPC3-2，移除 3-1 展示xiaoya
                RemoveNpcFromList("NPC3-1");
            }
            else
            {
                // 如果失败或拒绝，移除 NPC3-2，展示 邪教头
                RemoveNpcFromList("NPC3-2");
            }
        }
    }

    void RemoveNpcFromList(string npcName)
    {
        GameObject toRemove = npcList.Find(n => n.name == npcName);
        if (toRemove != null)
        {
            npcList.Remove(toRemove);
            toRemove.SetActive(false); // 确保它不会出现在场景中
        }
    }

    // ���ߵ�ǰ NPC
    public void DismissCurrentNPC()
    {
        if (curtainController == null || !curtainController.IsOpen)
        {
            Debug.Log("���ӻ�û���������ܸ��ˡ�");
            return;
        }

        StartCoroutine(SwitchNPCRoutine());
    }

    IEnumerator SwitchNPCRoutine()
    {
        GameObject oldNPC = null;
        if (currentIndex < npcList.Count)
        {
            oldNPC = npcList[currentIndex];
        }

        if (oldNPC != null)
        {
            var fader = oldNPC.GetComponent<NPCFade>();
            yield return fader.DismissAndDeactivate().WaitForCompletion();
        }

        curtainController.CloseCurtains();
        yield return new WaitForSeconds(curtainController.duration + 0.1f);

        // oldNPC ��������
        if (oldNPC != null)
        {
            Destroy(oldNPC);
        }

        // ����ָ����һλ
        currentIndex++;
        if (currentIndex < npcList.Count && npcList[currentIndex] != null)
        {
            npcList[currentIndex].SetActive(true);
            npcList[currentIndex].GetComponent<NPCIdentity>().SendInfoToManager();
        }
        // 在 SceneManager.cs 的 SwitchNPCRoutine 协程末尾
        else
        {
            Debug.Log("没有下一个 NPC 了，准备结束这一天");
            // 通知 GameManager 结束这一天
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndDay(); 
            }
        }
    }


}
