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
        npcList[0].GetComponent<NPCIdentity>().SendInfoToManager();
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
        else
        {
            GameManager.Instance.LoadNextLevel();
        }
    }


}
