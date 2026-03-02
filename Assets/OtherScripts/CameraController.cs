using UnityEngine;

public class CameraFollow2DDeadZone : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public float smoothTime = 0.15f;
    public Vector2 deadZoneSize = new Vector2(3f, 2f);

    [Header("Zoom")]
    public float minZoom = 5f;          // zoom when on the ground
    public float maxZoom = 9f;          // zoom when high in the air
    public float zoomSpeed = 5f;        // how fast zoom changes
    public float maxHeightForZoom = 10f; // height at which max zoom is reached

    private Vector3 _velocity;
    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 camPos = transform.position;
        Vector3 targetPos = target.position;

        float halfWidth = deadZoneSize.x / 2f;
        float halfHeight = deadZoneSize.y / 2f;

        float left = camPos.x - halfWidth;
        float right = camPos.x + halfWidth;
        float bottom = camPos.y - halfHeight;
        float top = camPos.y + halfHeight;

        Vector3 desiredPos = camPos;

        if (targetPos.x < left)
            desiredPos.x = targetPos.x + halfWidth;
        else if (targetPos.x > right)
            desiredPos.x = targetPos.x - halfWidth;

        if (targetPos.y < bottom)
            desiredPos.y = targetPos.y + halfHeight;
        else if (targetPos.y > top)
            desiredPos.y = targetPos.y - halfHeight;

        transform.position = Vector3.SmoothDamp(
            camPos,
            new Vector3(desiredPos.x, desiredPos.y, camPos.z),
            ref _velocity,
            smoothTime
        );

        HandleZoom();
    }

    void HandleZoom()
    {
        // Distance above ground (y = 0)
        float height = Mathf.Max(0f, target.position.y);

        // Normalize height into 0–1 range
        float t = Mathf.Clamp01(height / maxHeightForZoom);

        // Lerp zoom between min and max
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, t);

        // Smooth zoom
        _cam.orthographicSize = Mathf.Lerp(
            _cam.orthographicSize,
            targetZoom,
            Time.deltaTime * zoomSpeed
        );
    }
}