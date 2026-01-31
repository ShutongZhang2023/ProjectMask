using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;

public class DialogueController : MonoBehaviour
{
    [Header("UI 组件")]
    public TextMeshProUGUI contentText;
    public Button nextButton;
    public Button prevButton;
    public Button pressButton;
    public Button exitButton;
    public RectTransform dialogueContainer;

    [Header("设置")]
    public float typingSpeed = 0.05f;

    [Header("当前加载的对话")]
    public DialogueData currentDialogue;

    private int currentIndex = 0;

    // 状态标记
    private bool isCrossExamination = false;
    private bool isTyping = false;

    private Coroutine typingCoroutine;

    void Start()
    {
        nextButton.onClick.AddListener(OnNextClicked);
        prevButton.onClick.AddListener(OnPrevClicked);
        pressButton.onClick.AddListener(OnPressClicked);
        exitButton.onClick.AddListener(() => { gameObject.SetActive(false); });

        // 初始隐藏功能按钮
        prevButton.gameObject.SetActive(false);
        pressButton.gameObject.SetActive(false);

        if (currentDialogue != null)
        {
            isCrossExamination = false;
            StartDialogue(currentDialogue);
        }
    }

    public void StartDialogue(DialogueData data)
    {
        currentDialogue = data;
        currentIndex = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        DialogueLine line = currentDialogue.lines[currentIndex];

        if (isCrossExamination)
        {
            // 询问模式：不打字，直接显示全
            contentText.text = line.content;
            contentText.maxVisibleCharacters = int.MaxValue;
            isTyping = false;
            UpdateButtonsState();
        }
        else
        {
            // 初次浏览：打字机效果
            typingCoroutine = StartCoroutine(TypewriterEffect(line.content));
        }
    }

    void UpdateButtonsState()
    {
        // 询问模式下：
        // 1. 上一句按钮：总是显示
        prevButton.gameObject.SetActive(isCrossExamination);

        // 2. 细说按钮：总是显示（哪怕是错误的句子也要显示，否则就是直接告诉玩家答案了）
        // 只有在打字时为了防误触才隐藏
        pressButton.gameObject.SetActive(isCrossExamination && !isTyping);

        nextButton.gameObject.SetActive(true);
    }

    IEnumerator TypewriterEffect(string content)
    {
        isTyping = true;
        // 打字时隐藏交互按钮
        pressButton.gameObject.SetActive(false);
        prevButton.gameObject.SetActive(false);

        contentText.text = content;
        contentText.maxVisibleCharacters = 0;

        int totalChars = content.Length;
        for (int i = 0; i <= totalChars; i++)
        {
            contentText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        FinishTyping();
    }

    void FinishTyping()
    {
        isTyping = false;
        // 核心：解除字符限制，确保显示完整
        contentText.maxVisibleCharacters = int.MaxValue;

        // 打字结束，刷新按钮状态
        UpdateButtonsState();
    }

    void OnNextClicked()
    {
        // 1. 正在打字 -> 瞬间显示全
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            FinishTyping();
            return;
        }

        // 2. 正常翻页
        if (currentIndex < currentDialogue.lines.Count - 1)
        {
            currentIndex++;
            UpdateUI();
        }
        else
        {
            // 到底了
            if (!isCrossExamination)
            {
                isCrossExamination = true; // 开启询问模式
                currentIndex = 0;
                UpdateUI();
            }
            else
            {
                // 询问模式循环回到第一句
                currentIndex = 0;
                UpdateUI();
            }
        }
    }

    void OnPrevClicked()
    {
        if (!isCrossExamination || isTyping) return;

        if (currentIndex > 0) currentIndex--;
        else currentIndex = currentDialogue.lines.Count - 1; // 循环

        UpdateUI();
    }

    void OnPressClicked()
    {
        DialogueLine currentLine = currentDialogue.lines[currentIndex];

        // 核心修改：极其简单的判定
        if (currentLine.nextStageDialogue != null)
        {
            // 切换数据
            currentDialogue = currentLine.nextStageDialogue;

            // 重置状态（新关卡要重新经历一遍只读模式）
            isCrossExamination = false;

            StartDialogue(currentDialogue);
        }
        else
        {
            TriggerShake();
        }
    }

    private void TriggerShake() {
        dialogueContainer.DOComplete();
        dialogueContainer.DOShakeAnchorPos(0.5f, new Vector2(10f, 10f), 20, 90f);
    }
}