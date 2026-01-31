using UnityEngine;

public class Curtain : MonoBehaviour
{
    [SerializeField] private CurtainController controller;

    private void OnMouseDown()
    {
        controller.OpenCurtains();
    }
}
