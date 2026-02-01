using UnityEngine;
using UnityEngine.UI; // 引用 UI
using TMPro; // 如果你用的是 TextMeshPro，如果用普通 Text 就去掉这行
using DG.Tweening;
using System.Collections;

public class MaskRepairController : MonoBehaviour
{
    [Header("UI 设置")]
    [Tooltip("拖入那个修复按钮的 GameObject")]
    public GameObject repairButtonObject;
    [Tooltip("拖入按钮上显示价格的文本框")]
    public TextMeshProUGUI costText; // 如果用旧版 Text，这里改成 Text

    [Header("价格配置")]
    public int costHealthMinor = 250; // Health=1 时的维修费
    public int costHealthMajor = 500; // Health=0 时的维修费
    public int costHungerFeed = 500;  // Hunger=0 时的喂食费

    private MaskWorldItem interactionScript;
    private MaskVisual visualScript;
    private int currentCost = 0;

    private Coroutine subscribeCo;

    void Awake()
    {
        interactionScript = GetComponent<MaskWorldItem>();
        visualScript = GetComponent<MaskVisual>();

        if (repairButtonObject != null) repairButtonObject.SetActive(false);
    }

    void OnEnable()
    {
        subscribeCo = StartCoroutine(SubscribeWhenReady());
    }

    void OnDisable()
    {
        if (subscribeCo != null)
        {
            StopCoroutine(subscribeCo);
            subscribeCo = null;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.OnDayDataUpdated -= CheckRepairStatus;
    }

    IEnumerator SubscribeWhenReady()
    {
        while (GameManager.Instance == null)
            yield return null;

        GameManager.Instance.OnDayDataUpdated += CheckRepairStatus;

        CheckRepairStatus(); // 立刻刷新一次 UI 状态
    }

    // --- 1. 检查是否需要显示按钮 ---
    void CheckRepairStatus()
    {
        Debug.Log("[面具修复] 检查修复状态");
        if (GameManager.Instance == null || interactionScript == null) return;

        // 获取数据
        var data = GameManager.Instance.allMasks.Find(m => m.maskID == interactionScript.myMaskID);
        if (data == null || !data.isUnlocked)
        {
            if (repairButtonObject != null) repairButtonObject.SetActive(false);
            return;
        }

        currentCost = 0;

        // === 叠加算法 ===

        // 1. 算血量维修费
        if (data.health == 1)
        {
            currentCost += costHealthMinor; // +250
        }
        else if (data.health <= 0)
        {
            currentCost += costHealthMajor; // +500
        }

        // 2. 算饥饿喂食费 (如果是 0)
        if (data.hunger <= 0)
        {
            currentCost += costHungerFeed; // +500
        }

        // === 结果判断 ===
        if (currentCost > 0)
        {
            // 需要修复：显示按钮，更新价格
            if (repairButtonObject != null)
            {
                repairButtonObject.SetActive(true);
                if (costText != null) costText.text = $"${currentCost}";
            }
        }
        else
        {
            // 不需要修复：隐藏按钮
            if (repairButtonObject != null) repairButtonObject.SetActive(false);
        }
    }

    // --- 2. 点击按钮执行修复 (绑定到 Button 的 OnClick) ---
    public void OnRepairClicked()
    {
        if (currentCost <= 0) return;

        // 检查钱够不够 (这里假设 CurrencyManager 存着总钱数，或者你用 GameManager 里的变量)
        // 下面这行代码请根据你实际存钱的变量修改
        int playerMoney =  CurrencyManager.Instance.currentMoney;

        if (playerMoney >= currentCost)
        {
            // 1. 扣钱
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.SpendMoney(currentCost);
            else
                Debug.Log($"假设扣除了 {currentCost} 元");

            // 2. 修复数据 (回满)
            var data = GameManager.Instance.allMasks.Find(m => m.maskID == interactionScript.myMaskID);
            if (data != null)
            {
                data.health = 2; // 修好
                data.hunger = 2; // 喂饱
                // 也可以根据实际情况只修血或只喂食，看你需求，这里是全家桶服务
            }

            // 3. 播放音效 (可选)
            Debug.Log("修复成功！");

            // 4. 隐藏按钮
            if (repairButtonObject != null) repairButtonObject.SetActive(false);

            // 5. 关键：手动刷新视觉 (让面具变回完好，眼睛亮起来)
            if (visualScript != null) visualScript.UpdateVisuals();
        }
        else
        {
            Debug.Log("钱不够，修不起！");
            repairButtonObject.transform.DOShakePosition(0.3f, 5f);
        }
    }
}