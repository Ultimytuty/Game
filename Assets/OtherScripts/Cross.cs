using UnityEngine;
using UnityEngine.InputSystem;

public class Cross : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Mouse.current != null)
            mousePos = Mouse.current.position.ReadValue();
        else
            mousePos = Input.mousePosition;
#else
        mousePos = Input.mousePosition;
#endif

        Camera cam = Camera.main;
        if (cam == null)
            return;

        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Mathf.Abs(cam.transform.position.z - transform.position.z)));
        worldPos.z = transform.position.z;
        transform.position = worldPos;
    }
}
