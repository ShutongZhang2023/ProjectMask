using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("游戏状态")]
    public int currentDay = 1;

    [Header("面具库存")]
    // 在 Inspector 里把5个面具都配好，前3个勾选 isUnlocked，后2个不勾
    public List<MaskData> allMasks = new List<MaskData>();

    [Header("剧情分支记录")]
    // Key: "Day_NPCID" (例如 "Day1_Beggar"), Value: 结果 (Success, Fail, Ignored)
    public Dictionary<string, string> storyDecisions = new Dictionary<string, string>();

    void Awake()
    {
        // 单例模式 + 跨场景不销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 玩家提交面具
    // maskIndex: 玩家选了第几个面具
    // requiredMaskID: NPC 想要的面具 ID
    // npcID: 当前 NPC 的 ID，用于记录剧情
    public void SubmitMask(int maskIndex, string requiredMaskID, string npcID)
    {
        MaskData selectedMask = allMasks[maskIndex];

        // 1. 检查面具是否可用
        if (selectedMask.IsBroken)
        {
            Debug.Log("这个面具已经坏了，无法使用！");
            return;
        }

        // 2. 判断对错
        bool isCorrect = (selectedMask.maskID == requiredMaskID);

        // 3. 更新面具数值
        selectedMask.Use(isCorrect);

        // 4. 记录结果 & 处理剧情
        string resultKey = $"Day{currentDay}_{npcID}";
        string resultValue = isCorrect ? "Success" : "WrongMask";

        if (storyDecisions.ContainsKey(resultKey)) storyDecisions[resultKey] = resultValue;
        else storyDecisions.Add(resultKey, resultValue);

        Debug.Log($"使用了面具: {selectedMask.maskID}, 结果: {resultValue}, 剩余血量: {selectedMask.health}, 饥饿: {selectedMask.hunger}");

        // 这里可以通知 UI 更新，或者通知 DialogueSystem 播放对应分支
    }

    // 玩家选择“不给面具”
    public void RefuseToGive(string npcID)
    {
        string resultKey = $"Day{currentDay}_{npcID}";
        storyDecisions[resultKey] = "Refused";
        Debug.Log("玩家没有选择面具");
    }

    // --- 流程控制 ---

    // 结束这一天（通常在睡觉或切场景时调用）
    public void EndDay()
    {
        // 结算所有面具的 Hunger
        foreach (var mask in allMasks)
        {
            if (mask.isUnlocked)
            {
                mask.OnDayEnd();
            }
        }

        currentDay++;
        CheckNewMaskUnlocks(); // 检查是否有新面具在明天解锁

        Debug.Log($"进入第 {currentDay} 天，面具状态已更新。");
    }

    // 根据天数解锁新面具
    void CheckNewMaskUnlocks()
    {
        // 示例：第2天解锁第4个面具
        if (currentDay == 2) UnlockMask("Xiongbo");
        if (currentDay == 3) UnlockMask("Lanzhu");
    }

    public void UnlockMask(string id)
    {
        var mask = allMasks.Find(m => m.maskID == id);
        if (mask != null) mask.isUnlocked = true;
    }
}
