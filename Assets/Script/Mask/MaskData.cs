using UnityEngine;

[System.Serializable]
public class MaskData
{
    [Header("Basic Info")]
    public string maskID;
    public bool isUnlocked;

    [Header("data")]
    public int hunger = 2;
    public int health = 2;
    public bool hasBeenUsedToday = false;

    public bool IsBroken => hunger <= 0 || health <= 0;

    // 当玩家选择这个面具时调用
    // isCorrect: 玩家选的对不对
    public void Use(bool isCorrect)
    {
        if (IsBroken) return;

        hasBeenUsedToday = true;

        hunger = Mathf.Clamp(hunger + 1, 0, 2);
        if (!isCorrect)
        {
            health = Mathf.Clamp(health - 1, 0, 2);
        }
    }

    // 每天结束时调用
    public void OnDayEnd()
    {
        if (IsBroken) return;
        if (!hasBeenUsedToday)
        {
            hunger = Mathf.Clamp(hunger - 1, 0, 2);
        }
        hasBeenUsedToday = false;
    }
}