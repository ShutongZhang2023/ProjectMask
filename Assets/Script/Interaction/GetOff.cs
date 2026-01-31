using DG.Tweening;
using UnityEngine;
using System.Collections;

public class GetOff : MonoBehaviour
{
    [Header("组件设置")]
    [Tooltip("扇子物体上的 Animator 组件")]
    public Animator fanAnimator;
    [Tooltip("Animator 里设置的 Trigger 参数名字，比如 'Wave'")]
    public string animTriggerName = "Wave";
    [Tooltip("扇子动画大概多长？(秒) - 程序会等这么久再出字")]
    public float fanAnimDuration = 1.0f;

    [Header("视觉特效")]
    [Tooltip("那个'滚'字的 SpriteRenderer")]
    public SpriteRenderer gunWordSprite;
    [Tooltip("字飞向屏幕需要多久")]
    public float wordZoomDuration = 0.5f;
    [Tooltip("字最终放大的倍数")]
    public float maxScale = 50f;
    private bool isAnimating = false;

    void Start()
    {
        gunWordSprite.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (isAnimating) return;
        if (!SceneManager.instance.curtainController.IsOpen) return;

        StartCoroutine(PlaySequenceRoutine());
    }

    IEnumerator PlaySequenceRoutine()
    {
        isAnimating = true;

        fanAnimator.SetTrigger(animTriggerName);
        yield return new WaitForSeconds(fanAnimDuration);

        gunWordSprite.gameObject.SetActive(true);
        gunWordSprite.transform.localScale = Vector3.one;
        Color c = gunWordSprite.color; c.a = 0f; gunWordSprite.color = c;

        // 创建动画序列
        Sequence seq = DOTween.Sequence();
        seq.Append(gunWordSprite.DOFade(1f, 0.1f));
        seq.Join(gunWordSprite.transform.DOScale(maxScale, wordZoomDuration).SetEase(Ease.InExpo));
        seq.Insert(wordZoomDuration - 0.2f, gunWordSprite.DOFade(0f, 0.2f));
        yield return seq.WaitForCompletion();

        // 善后
        gunWordSprite.gameObject.SetActive(false);

        //赶人
        SceneManager.instance.DismissCurrentNPC();

        isAnimating = false;
    }
}
