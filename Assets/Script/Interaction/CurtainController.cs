using UnityEngine;
using DG.Tweening;

public class CurtainController : MonoBehaviour
{
    [Header("Curtain")]
    public Transform leftCurtain;
    public Transform rightCurtain;

    [Header("Movement Setting")]
    public float moveDistance = 2.0f;
    public float duration = 1.5f; 
    public Ease easeType = Ease.InOutQuad;

    private bool isOpen = false;
    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    public bool IsOpen => isOpen;

    void Start()
    {
        leftStartPos = leftCurtain.localPosition;
        rightStartPos = rightCurtain.localPosition;
    }

    public void OpenCurtains()
    {
        isOpen = true;

        Vector3 leftTarget = leftStartPos + new Vector3(-moveDistance, 0, 0);
        Vector3 rightTarget = rightStartPos + new Vector3(moveDistance, 0, 0);

        leftCurtain.DOLocalMove(leftTarget, duration).SetEase(easeType);
        rightCurtain.DOLocalMove(rightTarget, duration).SetEase(easeType);

        SetCollidersState(false);
    }


    public void CloseCurtains()
    {
        if (!isOpen) return; 
        isOpen = false;

        leftCurtain.DOLocalMove(leftStartPos, duration).SetEase(easeType);
        rightCurtain.DOLocalMove(rightStartPos, duration).SetEase(easeType);

        SetCollidersState(true);
    }
    void SetCollidersState(bool isEnabled)
    {
        var col2D_L = leftCurtain.GetComponent<Collider2D>();
        if (col2D_L) col2D_L.enabled = isEnabled;
        var col2D_R = rightCurtain.GetComponent<Collider2D>();
        if (col2D_R) col2D_R.enabled = isEnabled;
    }
}
