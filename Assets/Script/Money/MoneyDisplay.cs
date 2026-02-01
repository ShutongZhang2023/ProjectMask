using UnityEngine;
using TMPro; // 确保引用了命名空间

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText; // 注意：这里去掉了 'UGUI'

    void OnEnable()
    {
        // 订阅金钱变化事件
        CurrencyManager.OnMoneyChanged += UpdateDisplay;
    }

    void OnDisable()
    {
        // 取消订阅，防止内存泄漏
        CurrencyManager.OnMoneyChanged -= UpdateDisplay;
    }

    void Start()
    {
        // 初始化显示当前的金额
        if (CurrencyManager.Instance != null)
        {
            UpdateDisplay(CurrencyManager.Instance.currentMoney);
        }
    }

    void UpdateDisplay(int amount)
    {
        if (moneyText != null)
        {
            moneyText.text = $"{amount}";
        }
    }
}