using DG.Tweening;
using UnityEngine;
using System.Collections;

public class GetOff : MonoBehaviour
{
    [Header("�������")]
    [Tooltip("���������ϵ� Animator ���")]
    public Animator fanAnimator;
    [Tooltip("Animator �����õ� Trigger �������֣����� 'Wave'")]
    public string animTriggerName = "Wave";
    [Tooltip("���Ӷ�����Ŷ೤��(��) - ��������ô���ٳ���")]
    public float fanAnimDuration = 1.0f;

    [Header("�Ӿ���Ч")]
    [Tooltip("�Ǹ�'��'�ֵ� SpriteRenderer")]
    public SpriteRenderer gunWordSprite;
    [Tooltip("�ַ�����Ļ��Ҫ���")]
    public float wordZoomDuration = 0.5f;
    [Tooltip("�����շŴ�ı���")]
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

        GameManager.Instance.RefuseToGive(GameManager.Instance.currentTargetNPC);

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

        // ������������
        Sequence seq = DOTween.Sequence();
        seq.Append(gunWordSprite.DOFade(1f, 0.1f));
        seq.Join(gunWordSprite.transform.DOScale(maxScale, wordZoomDuration).SetEase(Ease.InExpo));
        seq.Insert(wordZoomDuration - 0.2f, gunWordSprite.DOFade(0f, 0.2f));
        yield return seq.WaitForCompletion();

        // �ƺ�
        gunWordSprite.gameObject.SetActive(false);

        //����
        SceneManager.instance.DismissCurrentNPC();

        isAnimating = false;
    }
}
