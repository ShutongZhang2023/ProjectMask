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
        foreach (var npc in npcList)
        {
            if (npc != null) npc.SetActive(false);
        }
        currentIndex = 0;

        npcList[0].SetActive(true);
    }

    // 赶走当前 NPC
    public void DismissCurrentNPC()
    {
        if (curtainController == null || !curtainController.IsOpen)
        {
            Debug.Log("帘子还没拉开，不能赶人。");
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

        // oldNPC 彻底销毁
        if (oldNPC != null)
        {
            Destroy(oldNPC);
        }

        // 索引指向下一位
        currentIndex++;
        if (currentIndex < npcList.Count && npcList[currentIndex] != null)
        {
            npcList[currentIndex].SetActive(true);
        }
        else
        {
            Debug.Log("没有下一个 NPC 了，流程结束");
            //这里触发这个scene结束的逻辑
        }
    }
}
