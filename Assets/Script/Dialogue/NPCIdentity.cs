using UnityEngine;

public class NPCIdentity : MonoBehaviour
{
    [Header("NPC 身份信息")]
    public string npcID;            // 在 GameManager 记录分支时使用的唯一 ID (如 "Beggar")
    public string requiredMaskID;   // 这个 NPC 想要的面具 ID (如 "Joy")
    public int moneyPaid;

    // 当对话开始时，调用这个方法把信息同步给 GameManager
    public void SendInfoToManager()
    {
        GameManager.Instance.currentTargetNPC = npcID;
        GameManager.Instance.currentRequiredMask = requiredMaskID;
        GameManager.Instance.moneyEarned = moneyPaid;
        Debug.Log($"[NPC] {npcID} 正在等待面具: {requiredMaskID}");
    }
}