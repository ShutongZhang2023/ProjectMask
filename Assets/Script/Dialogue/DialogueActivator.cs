using UnityEngine;

public class DialogueActivator : MonoBehaviour
{
    public GameObject  dialogueController;

    private void OnMouseDown()
    {
        bool isActive = dialogueController.activeSelf;
        dialogueController.SetActive(!isActive);
    }
}
