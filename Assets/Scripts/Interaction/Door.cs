using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable, IActivatable
{
    [Header("Door")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;

    public enum OpenDirection
    {
        Left,
        Right
    }

    [Header("Open Direction")]
    [SerializeField] private OpenDirection openDirection = OpenDirection.Right;

    [Header("Interaction text")]
    [SerializeField] private string openText = "Open Door";
    [SerializeField] private string closeText = "Close Door";

    private bool isOpen;
    private bool isMoving;

    private Quaternion closedRotation;
    private Quaternion openedRotation;

    private void Awake()
    {
        closedRotation = transform.rotation;

        float angle = openDirection == OpenDirection.Right ? openAngle : -openAngle;

        openedRotation = closedRotation * Quaternion.Euler(0f, angle, 0f);
    }

    public string GetInteractionText()
    {
        return isOpen ? closeText : openText;
    }

    public void Interact(PlayerMovement player)
    {
        Toggle();
    }

    // 문열기
    public void Open()
    {
        if (isMoving || isOpen)
            return;

        StartCoroutine(MoveDoor(true));
    }

    // 문닫기
    public void Close()
    {
        if (isMoving || !isOpen)
            return;

        StartCoroutine(MoveDoor(false));
    }

    // 문 상태 전환
    public void Toggle()
    {
        if (isMoving)
            return;

        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    // 외부 장치가 문을 활성화할 때 호출
    public void Activate()
    {
        Toggle();
    }

    private IEnumerator MoveDoor(bool open)
    {
        isMoving = true;

        Quaternion target = open ? openedRotation : closedRotation;

        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                target,
                openSpeed * Time.deltaTime);

            yield return null;
        }
        transform.rotation = target;

        isOpen = open;
        isMoving = false;
    }
}
