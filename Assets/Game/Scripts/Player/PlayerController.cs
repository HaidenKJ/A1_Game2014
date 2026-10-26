using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    Camera cam; 
    [SerializeField] Transform visual;


    [Tooltip("Max turn speed in degrees per second. Set to 0 for instant turning.")] // Learned earlier you can have tooltips, prob won't use it for this tho
    [SerializeField] float turnSpeed = 360f;
    [SerializeField] float spriteAngleOffset = -90f;

    [SerializeField] bool ignoreUITouches = true;
    public Vector2 AimDirection { get; private set; } = Vector2.up;
    public float AimAngle { get; private set; } = 90f;
    public Vector2 TargetPoint { get; private set; }

    bool hasTarget;
    int ignoredFingerId = -1;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (visual == null) visual = transform;

        ApplyRotation();
    }

    void Update()
    {
        ReadInput();
        RotateTowardTarget();
    }

    void ReadInput()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                ignoredFingerId = (ignoreUITouches && IsOverUI(t.fingerId)) ? t.fingerId : -1;
            }

            if (t.fingerId != ignoredFingerId && (t.phase == TouchPhase.Began || t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary))
            {
                SetTarget(t.position);
            }
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            ignoredFingerId = (ignoreUITouches && IsOverUI(-1)) ? 0 : -1;
        }

        if (Input.GetMouseButton(0) && ignoredFingerId == -1)
        {
            SetTarget(Input.mousePosition);
        }

    }

    bool IsOverUI(int fingerId)
    {
        if (EventSystem.current == null) return false;
        return fingerId < 0 ? EventSystem.current.IsPointerOverGameObject() : EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    void SetTarget(Vector2 screenPos)
    {
        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        TargetPoint = new Vector2(world.x, world.y);
        hasTarget = true;
    }

    void RotateTowardTarget()
    {
        if (!hasTarget) return;

        Vector2 to = TargetPoint - (Vector2)transform.position;
        if (to.sqrMagnitude < 0.0001f) return; // touch is right on top of the player

        float targetAngle = Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg;

        AimAngle = (turnSpeed <= 0f)
            ? targetAngle
            : Mathf.MoveTowardsAngle(AimAngle, targetAngle, turnSpeed * Time.deltaTime);

        ApplyRotation();
    }

    void ApplyRotation()
    {
        float rad = AimAngle * Mathf.Deg2Rad;
        AimDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        visual.rotation = Quaternion.Euler(0f, 0f, AimAngle + spriteAngleOffset);
    }
}