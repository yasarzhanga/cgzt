using UnityEngine;
using UnityEngine.InputSystem;

public class SHS_DeBUG : MonoBehaviour
{
    public GameObject obj;

    void Update()
    {
        // 适配新 Input System：使用 Keyboard.current 轮询
        if (Keyboard.current != null && Keyboard.current.backquoteKey.wasPressedThisFrame)
        {
            if (obj.transform.localScale.x == 0)
            {
                obj.transform.localScale = Vector3.one;
            }
        }
    }
}
