using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string currentTargetNPC;
    public string currentRequiredMask;

    [Header("��Ϸ״̬")]
    public int currentDay = 1;

    [Header("��߿��")]
    // �� Inspector ���5����߶���ã�ǰ3����ѡ isUnlocked����2������
    public List<MaskData> allMasks = new List<MaskData>();

    [Header("�����֧��¼")]
    // Key: "Day_NPCID" (���� "Day1_Beggar"), Value: ��� (Success, Fail, Ignored)
    public Dictionary<string, string> storyDecisions = new Dictionary<string, string>();

    void Awake()
    {
        // ����ģʽ + �糡��������
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

    // ����ύ���
    // maskIndex: ���ѡ�˵ڼ������
    // requiredMaskID: NPC ��Ҫ����� ID
    // npcID: ��ǰ NPC �� ID�����ڼ�¼����
    public void SubmitMask(string selectedMaskID, string requiredMaskID, string npcID)
    {
        MaskData selectedMask = allMasks.Find(m => m.maskID == selectedMaskID);

        if (selectedMask == null) return;

        // 1. �������Ƿ����
        if (selectedMask.IsBroken)
        {
            Debug.Log("�������Ѿ����ˣ��޷�ʹ�ã�");
            return;
        }

        // 2. �ж϶Դ�
        bool isCorrect = (selectedMaskID == requiredMaskID);

        CurrencyManager.Instance.AddMoney(100); 
        Debug.Log("100 money added");


        // ALL HARDCODED FOR MONEY SYSTEM
        // 注意：苏晴的分支 2 是在第二天扣钱，我们看情况 B
        // 检查昨天的结果
        string suQingResult = "";
        storyDecisions.TryGetValue($"Day{currentDay}_SuQing", out suQingResult);

        if (suQingResult == "Success")
        {
            CurrencyManager.Instance.SpendMoney(50); // 桌面清理大师 -200
            Debug.Log("苏晴不爽，砸了你的店！-200");
        }

        // 处理xiaoya的感谢信
        string xiaoyaResult = "";
        storyDecisions.TryGetValue($"Day{currentDay}_Xiaoya", out xiaoyaResult);
        if (xiaoyaResult == "Success")
        {
            CurrencyManager.Instance.SpendMoney(50); //
        }    

        // 3. ���������ֵ
        selectedMask.Use(isCorrect);

        // 4. ��¼��� & ��������
        string resultKey = $"Day{currentDay}_{npcID}";
        string resultValue = isCorrect ? "Success" : "WrongMask";

        if (storyDecisions.ContainsKey(resultKey)) storyDecisions[resultKey] = resultValue;
        else storyDecisions.Add(resultKey, resultValue);

        Debug.Log($"ʹ�������: {selectedMask.maskID}, ���: {resultValue}, ʣ��Ѫ��: {selectedMask.health}, ����: {selectedMask.hunger}");

        // �������֪ͨ UI ���£�����֪ͨ DialogueSystem ���Ŷ�Ӧ��֧
    }

    // ���ѡ�񡰲�����ߡ�
    public void RefuseToGive(string npcID)
    {
        string resultKey = $"Day{currentDay}_{npcID}";
        storyDecisions[resultKey] = "Refused";
        Debug.Log("fuuuuuuck off");
    }

    // --- ���̿��� ---

    // ������һ�죨ͨ����˯�����г���ʱ���ã�
    public void EndDay()
    {
        // ����������ߵ� Hunger
        foreach (var mask in allMasks)
        {
            if (mask.isUnlocked)
            {
                mask.OnDayEnd();
            }
        }

        currentDay++;
        CheckNewMaskUnlocks(); // ����Ƿ�����������������

        Debug.Log($"����� {currentDay} �죬���״̬�Ѹ��¡�");
    }

    // �����������������
    void CheckNewMaskUnlocks()
    {
        // ʾ������2�������4�����
        if (currentDay == 2) UnlockMask("Xiongbo");
        if (currentDay == 3) UnlockMask("Lanzhu");
    }

    public void UnlockMask(string id)
    {
        var mask = allMasks.Find(m => m.maskID == id);
        if (mask != null) mask.isUnlocked = true;
    }
}
