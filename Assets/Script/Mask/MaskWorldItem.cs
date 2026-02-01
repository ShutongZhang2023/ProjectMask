using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MaskWorldItem : MonoBehaviour
{
    public string myMaskID;
    private SpriteRenderer sr;

    // 记录原始位置和大小
    private Vector3 originalPos;
    private Vector3 originalScale;

    // 记录原始层级
    private string originalLayerName;
    private int originalOrder;

    private bool isSelected = false;

    [Header("展示设置")]
    public GameObject presentationBackground; // 黑色背景

    [Header("层级设置")]
    [Tooltip("放大展示时，把面具放到哪个 Sorting Layer？确保这个 Layer 在背景之上")]
    public string focusLayerName = "Default";
    [Tooltip("在展示层里的 Order，设大一点确保最顶层")]
    public int focusOrder = 100;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPos = transform.position;
        originalScale = transform.localScale;

        // 1. 记下它现在的层级，方便以后还回去
        originalLayerName = sr.sortingLayerName;
        originalOrder = sr.sortingOrder;

        if (presentationBackground != null) presentationBackground.SetActive(false);
    }

    void Update()
    {
        // ... (保持之前的 Update 逻辑不变) ...
        if (GameManager.Instance == null) return;
        var data = GameManager.Instance.allMasks.Find(m => m.maskID == myMaskID);
        if (data != null)
        {
            sr.enabled = data.isUnlocked;
            var col = GetComponent<Collider2D>();
            if (col) col.enabled = data.isUnlocked;

            float healthPercent = (data.health + data.hunger) / 4f;
            if (data.IsBroken) sr.color = Color.black;
            else sr.color = Color.Lerp(Color.gray, Color.white, healthPercent);
        }
    }

    private void OnMouseDown()
    {
        if (isSelected) return;
        if (SceneManager.instance != null && !SceneManager.instance.curtainController.IsOpen) return;

        PlaySelectionAnimation();
    }

    void PlaySelectionAnimation()
    {
        isSelected = true;

        // 2. 关键点：在开始动画前，直接把层级提上来！
        // 这样它就会覆盖在所有东西上面
        sr.sortingLayerName = focusLayerName;
        sr.sortingOrder = focusOrder;

        Vector3 screenCenterWorld = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        screenCenterWorld.z = 0;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(screenCenterWorld, 0.3f).SetEase(Ease.OutExpo));
        seq.Join(transform.DOScale(originalScale * 3.5f, 0.3f).SetEase(Ease.OutExpo));

        // 2. 到位的一瞬间，摄像机震动 (0.2秒，强度0.5)
        seq.AppendCallback(() => {
            Camera.main.transform.DOShakePosition(0.2f, 0.5f, 20, 90, false, true);
        });

        seq.OnComplete(() =>
        {
            StartCoroutine(ShowBackgroundRoutine());
        });
    }

    IEnumerator ShowBackgroundRoutine()
    {
        if (presentationBackground != null)
        {
            // 背景出现。因为我们在 PlaySelectionAnimation 里已经把 sr 提到了 focusOrder (100)
            // 只要背景的 Order 小于 100，面具就会在背景上面。
            presentationBackground.SetActive(true);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SubmitMask(
                myMaskID,
                GameManager.Instance.currentRequiredMask,
                GameManager.Instance.currentTargetNPC
            );
        }

        yield return new WaitForSeconds(3.0f);

        // 复位
        ResetMaskInstantly();

        if (presentationBackground != null) presentationBackground.SetActive(false);

        if (SceneManager.instance != null)
        {
            SceneManager.instance.DismissCurrentNPC();
        }
    }

    void ResetMaskInstantly()
    {
        transform.position = originalPos;
        transform.localScale = originalScale;

        // 3. 关键点：归位时，把层级改回原来的，否则它会一直浮在所有东西上面
        sr.sortingLayerName = originalLayerName;
        sr.sortingOrder = originalOrder;

        isSelected = false;
    }
}