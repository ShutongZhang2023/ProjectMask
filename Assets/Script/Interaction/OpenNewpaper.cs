using UnityEngine;
using UnityEngine.UI;

public class OpenNewpaper : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;

    [Header("Condition")]
    [SerializeField] private string targetNpcID;
    [SerializeField] private string targetValue;


    [SerializeField] private GameObject targetCanvas;

    void Start()
    {
        UpdateImageByCondition();
    }

    void UpdateImageByCondition()
    {
        if (targetImage == null) return;

        bool condition = CheckCondition();

        targetImage.sprite = condition ? successSprite : failSprite;
    }

    private bool CheckCondition()
    {
        if (GameManager.Instance == null) return false;

        if (!GameManager.Instance.storyDecisions.ContainsKey(targetNpcID))
            return false;

        return GameManager.Instance.storyDecisions[targetNpcID] == targetValue;
    }

    private void OnMouseDown()
    {
        if (targetCanvas != null)
            targetCanvas.SetActive(true);
    }
}
