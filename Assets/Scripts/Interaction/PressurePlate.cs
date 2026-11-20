using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 플레이어가 밟으면 연결된 오브젝트를 활성화하는 압력판

public class PressurePlate : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private List<MonoBehaviour> targets = new();

    [Header("Plate Visual")]
    [SerializeField] private Transform plateVisual;

    [SerializeField] private float pressedDistance = 0.08f;
    [SerializeField] private float pressSpeed = 8f;

    // 실제로 호출할 수 있는 활성화 대상 목록
    private readonly List<IActivatable> activatables = new();

    // 압력판의 초기 위치
    private Vector3 defaultLocalPosition;

    // 압력판이 눌린 위치
    private Vector3 pressedLocalPosition;

    // 한 번만 작동하도록 상태 관리
    private bool isActivated;

    // 압력판 애니메이션 중복 실행 방지
    private Coroutine pressCoroutine;

    private void Awake()
    {
        // Target 목록에서 IActivatble 구현 여부 확인
        foreach (MonoBehaviour target in targets)
        {
            if (target is IActivatable activatable)
            {
                activatables.Add(activatable);
            }
            else if (target != null)
            {
                Debug.LogError(
                    $"{name}: Target이 IActivatable을 구현하지 않았습니다. {target.name}을(를) 확인하세요.",
                    target);
            }
        }

        // 압력판 외형의 초기 위치 저장
        if (plateVisual != null)
        {
            defaultLocalPosition = plateVisual.localPosition;

            pressedLocalPosition =
                defaultLocalPosition + Vector3.down * pressedDistance;
        }
    }

    // 플레이어가 압력판 영역에 진입하면 호출
    private void OnTriggerEnter(Collider other)
    {
        // 이미 작동했다면 다시 실행하지 않음
        if (isActivated)
            return;

        // 플레이어인지 확인
        PlayerMovement player =
            other.GetComponent<PlayerMovement>();

        if (player == null)
            return;

        ActivatePlate();
    }

    // 압력판 작동
    private void ActivatePlate()
    {
        isActivated = true;

        // 연결된 모든 오브젝트 활성화
        foreach (IActivatable activatable in activatables)
        {
            activatable.Activate();
        }

        // 압력판 외형을 아래로 이동
        if (plateVisual != null)
        {
            if (pressCoroutine != null)
            {
                StopCoroutine(pressCoroutine);
            }
            pressCoroutine = StartCoroutine(PressAnimation());
        }
    }

    // 압력판을 부드럽게 누르는 애니메이션
    private IEnumerator PressAnimation()
      {
        while (Vector3.Distance(
            plateVisual.localPosition,
            pressedLocalPosition) > 0.001f)
        {
            plateVisual.localPosition = Vector3.Lerp(
                plateVisual.localPosition,
                pressedLocalPosition,
                pressSpeed * Time.deltaTime);
            yield return null;
        }

        plateVisual.localPosition = pressedLocalPosition;
        pressCoroutine = null;
       }
    }
