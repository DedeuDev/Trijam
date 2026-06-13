using UnityEngine;

public class EnemySphereCore : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Transform topHalf;
    [SerializeField] private Transform bottomHalf;
    [SerializeField] private Transform core;

    [Header("Open Settings")]
    [SerializeField] private float openDistance = 1.2f;
    [SerializeField] private float openSpeed = 4f;

    [Header("Core Settings")]
    [SerializeField] private bool hideCoreWhenClosed = true;

    private Vector3 topClosedPosition;
    private Vector3 bottomClosedPosition;
    private Vector3 coreClosedScale;

    private Vector3 topOpenPosition;
    private Vector3 bottomOpenPosition;

    private bool isOpen;

    private void Awake()
    {
        topClosedPosition = topHalf.localPosition;
        bottomClosedPosition = bottomHalf.localPosition;
        coreClosedScale = core.localScale;

        topOpenPosition = topClosedPosition + Vector3.up * openDistance;
        bottomOpenPosition = bottomClosedPosition + Vector3.down * openDistance;

        UpdateCoreVisibility();
    }

    private void Update()
    {
        MoveParts();
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleOpen();
        }
    }

    private void MoveParts()
    {
        Vector3 targetTopPosition = isOpen ? topOpenPosition : topClosedPosition;
        Vector3 targetBottomPosition = isOpen ? bottomOpenPosition : bottomClosedPosition;

        topHalf.localPosition = Vector3.Lerp(
            topHalf.localPosition,
            targetTopPosition,
            openSpeed * Time.deltaTime
        );

        bottomHalf.localPosition = Vector3.Lerp(
            bottomHalf.localPosition,
            targetBottomPosition,
            openSpeed * Time.deltaTime
        );
    }

    public void Open()
    {
        isOpen = true;
        UpdateCoreVisibility();
    }

    public void Close()
    {
        isOpen = false;
        UpdateCoreVisibility();
    }

    public void ToggleOpen()
    {
        isOpen = !isOpen;
        UpdateCoreVisibility();
    }

    private void UpdateCoreVisibility()
    {
        if (!hideCoreWhenClosed) return;

        core.gameObject.SetActive(isOpen);
    }
}