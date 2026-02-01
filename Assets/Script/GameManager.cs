using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mesh;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string currentTargetNPC;
    public string currentRequiredMask;
    public int moneyEarned;

    public int currentDay = 1;

    public List<MaskData> allMasks = new List<MaskData>();

    public Dictionary<string, string> storyDecisions = new Dictionary<string, string>();

    [Header("Fade Setting")]
    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 0.5f;
    public List<string> sceneNames = new List<string>();
    private string currentLoadedSceneName;

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

    private void Start()
    {
        string firstLevel = sceneNames[0];

        if (UnityEngine.SceneManagement.SceneManager.GetSceneByName(firstLevel).isLoaded == false)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(firstLevel, LoadSceneMode.Additive);
            currentLoadedSceneName = firstLevel;
        }
        else
        {
            currentLoadedSceneName = firstLevel;
        }
    }

    public void SubmitMask(string selectedMaskID, string requiredMaskID, string npcID)
    {
        MaskData selectedMask = allMasks.Find(m => m.maskID == selectedMaskID);

        if (selectedMask == null) return;

        // 1. �������Ƿ����
        if (selectedMask.IsBroken)
        {
            return;
        }

        // 2. �ж϶Դ�
        bool isCorrect = (selectedMaskID == requiredMaskID);

        // 3. ���������ֵ
        selectedMask.Use(isCorrect);

        // 4. ��¼��� & ��������
        string resultKey = npcID;
        string resultValue = isCorrect ? "Success" : "WrongMask";

        if (storyDecisions.ContainsKey(resultKey)) storyDecisions[resultKey] = resultValue;
        else storyDecisions.Add(resultKey, resultValue);

        CurrencyManager.Instance.AddMoney(moneyEarned);

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

        if (currentDay == 2)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Day2");
            Debug.Log("Load scene Day 2");
        }
        else if (currentDay == 3)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Day3");
            Debug.Log("Load scene Day 3");
        }

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

    public void LoadNextLevel()
    {
        EndDay();

        int currentIndex = sceneNames.IndexOf(currentLoadedSceneName);
        if (currentIndex == -1)
        {
            Debug.LogError($"[GameManager] 找不到当前场景 '{currentLoadedSceneName}' 在列表里的位置！无法计算下一关。");
            return;
        }

        int nextIndex = currentIndex + 1;
        if (nextIndex < sceneNames.Count)
        {
            string nextSceneName = sceneNames[nextIndex];
            SwitchScene(nextSceneName);
        }
        else
        {
            Debug.Log("[GameManager] End of the game");
        }
    }

    public void SwitchScene(string nextSceneName)
    {
        StartCoroutine(TransitionRoutine(nextSceneName));
    }

    IEnumerator TransitionRoutine(string nextSceneName)
    {
        if (faderCanvasGroup != null)
        {
            faderCanvasGroup.blocksRaycasts = true;
            yield return faderCanvasGroup.DOFade(1f, fadeDuration).WaitForCompletion();
        }

        if (!string.IsNullOrEmpty(currentLoadedSceneName))
        {
            yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentLoadedSceneName);
        }

        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        currentLoadedSceneName = nextSceneName;

        Scene newScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(nextSceneName);
        if (newScene.IsValid())
        {
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(newScene);
        }
        yield return new WaitForSeconds(0.2f);
        if (faderCanvasGroup != null)
        {
            yield return faderCanvasGroup.DOFade(0f, fadeDuration).WaitForCompletion();
            faderCanvasGroup.blocksRaycasts = false;
        }
    }
}
