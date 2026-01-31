using DG.Tweening;
using UnityEngine;

public class NPCFade : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private SpriteRenderer sr;


    public Tween DismissAndDeactivate()
    {
        var dialogs = GetComponentsInChildren<DialogueController>();
        foreach (var d in dialogs)
        {
            if (d != null) d.deleteDialogue();
        }

        return sr.DOFade(0, fadeDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
