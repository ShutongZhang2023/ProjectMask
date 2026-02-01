using UnityEngine;

public class ClickAndOpen : MonoBehaviour
{
    [SerializeField] private GameObject a;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnMouseDown()
    {
        a.SetActive(true);
    }
}
