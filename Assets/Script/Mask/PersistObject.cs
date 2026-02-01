using UnityEngine;

public class PersistObject : MonoBehaviour
{
    private void Awake()
    {
        // 核心设置：这个物体及其子物体在切换场景时不会被销毁
        DontDestroyOnLoad(this.gameObject);
    }
}