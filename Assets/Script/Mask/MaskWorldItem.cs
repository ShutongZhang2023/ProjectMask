using UnityEngine;
using DG.Tweening; // 使用项目里的 DOTween

public class MaskWorldItem : MonoBehaviour
{
    public string myMaskID;
    private SpriteRenderer sr;
    private Vector3 originalPos;
    private Vector3 originalScale;
    private bool isSelected = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPos = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 1. 同步显隐逻辑（根据 GameManager 里的数据）
        var data = GameManager.Instance.allMasks.Find(m => m.maskID == myMaskID);
        if (data != null)
        {
            // 如果没解锁，连渲染器都关掉
            sr.enabled = data.isUnlocked;
            GetComponent<Collider2D>().enabled = data.isUnlocked;

            // 2. 视觉反馈：眼睛/面具变灰
            // 这里我们用 Color.Lerp 模拟。0和1是虚弱，2是满值
            float healthPercent = (data.health + data.hunger) / 4f;
            if (data.IsBroken) sr.color = Color.black;
            else sr.color = Color.Lerp(Color.gray, Color.white, healthPercent);
        }
    }

    // 当鼠标点击这个物体的 Collider 时触发
    private void OnMouseDown()
    {
        if (isSelected) return;

        // 执行动画效果
        PlaySelectionAnimation();
        Debug.Log("Nihao");
    }

    void PlaySelectionAnimation()
    {
        if (!SceneManager.instance.curtainController.IsOpen) return;
        isSelected = true;

        // 计算屏幕中心在世界空间的位置
        // 假设相机是 Camera.main，Z 轴设为 0
        Vector3 screenCenterWorld = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        screenCenterWorld.z = 0;

        // DOTween 动画序列
        Sequence seq = DOTween.Sequence();

        // 放大 2 倍并移到中央
        seq.Append(transform.DOMove(screenCenterWorld, 0.6f).SetEase(Ease.OutBack));
        seq.Join(transform.DOScale(originalScale * 3.5f, 0.6f));

        // 停留时间（展示面具）
        seq.AppendInterval(1.0f);

        // 动画结束后的逻辑
        seq.OnComplete(() =>
        {
            
            GameManager.Instance.SubmitMask(myMaskID, GameManager.Instance.currentRequiredMask, GameManager.Instance.currentTargetNPC);

            // 结束后回到原位
            ReturnToPosition();
        });
    }

    void ReturnToPosition()
    {
        transform.DOMove(originalPos, 0.4f);
        transform.DOScale(originalScale, 0.4f).OnComplete(() => isSelected = false);
    }
}