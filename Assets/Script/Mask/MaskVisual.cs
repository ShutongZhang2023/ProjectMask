using UnityEngine;
using UnityEngine.Rendering.Universal; // 引用 Light2D
using System.Collections;

public class MaskVisual : MonoBehaviour
{
    [Header("资源配置")]
    [Tooltip("按顺序: [0]=Health 2 (完好), [1]=Health 1 (裂纹), [2]=Health 0 (破碎)")]
    public Sprite[] healthSprites;

    [Header("眼睛设置")]
    [Tooltip("眼睛的 Sprite (眼珠子图片)")]
    public SpriteRenderer leftEyeSR;
    public SpriteRenderer rightEyeSR;

    [Tooltip("眼睛的光源 (Light 2D)")]
    public Light2D leftEyeLight;
    public Light2D rightEyeLight;

    private SpriteRenderer maskSr; // 面具本体的 SR
    private MaskWorldItem interactionScript; // 交互脚本引用

    void Awake()
    {
        maskSr = GetComponent<SpriteRenderer>();
        // 【重要】必须获取交互脚本，否则后面拿不到 ID 会报错
        interactionScript = GetComponent<MaskWorldItem>();
    }

    private IEnumerator Start()
    {
        maskSr = GetComponent<SpriteRenderer>();
        interactionScript = GetComponent<MaskWorldItem>();

        while (GameManager.Instance == null)
            yield return null;

        GameManager.Instance.OnDayDataUpdated += UpdateVisuals;

        UpdateVisuals();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDayDataUpdated -= UpdateVisuals;
    }

    public void UpdateVisuals()
    {
        Debug.Log("[面具] 更新视觉效果");
        if (GameManager.Instance == null || interactionScript == null) return;

        string myID = interactionScript.myMaskID;
        var data = GameManager.Instance.allMasks.Find(m => m.maskID == myID);

        if (data == null) return;

        // 1. 整体显隐（如果没解锁，直接全关）
        maskSr.enabled = data.isUnlocked;
        if (!data.isUnlocked)
        {
            SetEyeState(0, false); // 关灯，关眼睛图片
            return;
        }

        // 2. 面具本体 Sprite (Health)
        int spriteIndex = 0;
        if (data.health == 2) spriteIndex = 0;
        else if (data.health == 1) spriteIndex = 1;
        else spriteIndex = 2;

        if (healthSprites != null && spriteIndex < healthSprites.Length)
        {
            maskSr.sprite = healthSprites[spriteIndex];
        }

        // 3. 眼睛的状态 (Hunger)
        float intensity = 0f;
        bool showEyeSprite = false;

        if (data.hunger >= 2)
        {
            intensity = 120f;
            showEyeSprite = true; // 眼睛睁开
        }
        else if (data.hunger == 1)
        {
            intensity = 60f;
            showEyeSprite = true; // 眼睛睁开（光暗一点）
        }
        else
        {
            intensity = 0f;
            showEyeSprite = false; // 眼睛闭上/隐藏
        }

        SetEyeState(intensity, showEyeSprite);
    }

    // 辅助函数：同时控制 灯光 和 Sprite
    void SetEyeState(float lightVal, bool isVisible)
    {
        bool lightOn = lightVal > 0;

        // --- 左眼 ---
        if (leftEyeLight != null)
        {
            leftEyeLight.intensity = lightVal;
            leftEyeLight.enabled = lightOn;
        }
        if (leftEyeSR != null)
        {
            leftEyeSR.enabled = isVisible; // 控制贴图显隐
        }

        // --- 右眼 ---
        if (rightEyeLight != null)
        {
            rightEyeLight.intensity = lightVal;
            rightEyeLight.enabled = lightOn;
        }
        if (rightEyeSR != null)
        {
            rightEyeSR.enabled = isVisible; // 控制贴图显隐
        }
    }
}