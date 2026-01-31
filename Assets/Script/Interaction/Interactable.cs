using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Interactable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Outline Settings")]
    public string thicknessProperty = "_OutlineThickness";
    public float normalThickness = 0f;
    public float hoverThickness = 0.005f;
    public float fadeDuration = 0.5f;

    protected Material materialInstance;

    protected virtual void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        materialInstance = sr.material;
        materialInstance.SetFloat(thicknessProperty, normalThickness);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        materialInstance?.DOFloat(hoverThickness, thicknessProperty, fadeDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        materialInstance?.DOFloat(normalThickness, thicknessProperty, fadeDuration);
    }
}
