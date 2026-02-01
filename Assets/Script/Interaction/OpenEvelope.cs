using UnityEngine;

public class OpenEvelope : MonoBehaviour
{
    [SerializeField] private GameObject targetCanvas;
    [SerializeField] private string targetNpcID;
    [SerializeField] private string targetValue;
    [SerializeField] private int MoneyEarm;

    void Start()
    {
        if (!CheckCondition())
        {
            gameObject.SetActive(false);
        }
    }

    private bool CheckCondition()
    {
        if (GameManager.Instance == null) return false;

        if (GameManager.Instance.storyDecisions[targetNpcID] == targetValue)
        {
            return true;
        } else {
            return false;
        }
    }

    private void OnMouseDown()
    {
        CurrencyManager.Instance.AddMoney(MoneyEarm);
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(true);
        }
    }
}
