using UnityEngine;
using DG.Tweening;

public class QuestionMarkShake : MonoBehaviour
{
    public float scaleMultiplier = 1.2f;
    public float duration = 0.8f;

    void Start()
    {
        transform
            .DOScale(scaleMultiplier, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
