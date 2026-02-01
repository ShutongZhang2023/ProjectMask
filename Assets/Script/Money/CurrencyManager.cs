using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [Header("当前金金")]
    public int currentMoney = 0;

    // 定义一个事件，当钱数变化时通知 UI 更新，这样 UI 脚本不需要一直跑 Update
    public static event Action<int> OnMoneyChanged;

    void Awake()
    {
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

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney); // 广播：钱变多了！
        Debug.Log($"[金钱] 收入 {amount}，当前余额: {currentMoney}");
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney); // 广播：钱变少了！
            Debug.Log($"[金钱] 支出 {amount}，当前余额: {currentMoney}");
            return true;
        }
        else
        {
            Debug.Log("[金钱] 余额不足！");
            return false;
        }
    }
}