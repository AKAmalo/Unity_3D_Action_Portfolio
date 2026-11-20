using System.Collections;
using UnityEngine;

// 두 지점 사이를 이동하는 플랫폼
// IActivatable을 통해 Lever 등의 외부 장치로 작동시킬 수 있다.
public class MovingPlatform : MonoBehaviour, IActivatable
{
    [Header("Movement Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    // 현재 플랫폼이 이동 중인지 확인
    private bool isMoving;

    // 현재 플랫폼이 목적지에 도착했는지 확인
    private bool isAtEnd;

    // 현재 플랫폼 위에 있는 플레이어의 Rigidbody
    private Rigidbody playerRigidbody;

    private void Start()
    {
        // 시작 위치로 플랫폼 배치
        if (startPoint != null)
        {
            transform.position = startPoint.position;
        }
        else
        {
            Debug.LogError($"{name} : Start Point가 연결되지 않았습니다.");
        }

        if (endPoint == null)
        {
            Debug.LogError($"{name} : End Point가 연결되지 않았습니다.");
        }
    }

    // 외부 장치에 의해 플랫폼 이동
    public void Activate()
    {
        if (isMoving)
            return;

        StartCoroutine(MovePlatform());
    }

    // 플랫폼을 Start와 End 사이에서 이동
    private IEnumerator MovePlatform()
    {
        isMoving = true;

        Vector3 targetPosition =
            isAtEnd ? startPoint.position : endPoint.position;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            // 이동 전 플랫폼 위치 저장
            Vector3 previousPosition = transform.position;

            // 플랫폼 이동
            Vector3 nextPosition = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.fixedDeltaTime
            );

            transform.position = nextPosition;

            // 이번 물리 프레임 동안 플랫폼이 이동한 거리
            Vector3 platformDelta =
                transform.position - previousPosition;

            // 플레이어가 플랫폼 위에 있다면
            // 플랫폼이 이동한 만큼 플레이어도 이동
            if (playerRigidbody != null)
            {
                playerRigidbody.MovePosition(
                    playerRigidbody.position + platformDelta
                );
            }

            // 물리 프레임 기준으로 대기
            yield return new WaitForFixedUpdate();
        }

        transform.position = targetPosition;

        isAtEnd = !isAtEnd;
        isMoving = false;
    }

    // 플레이어가 플랫폼 위에 올라왔는지 감지
    private void OnCollisionEnter(Collision collision)
    {
        PlayerMovement player =
            collision.collider.GetComponentInParent<PlayerMovement>();

        if (player == null)
            return;

        playerRigidbody =
            player.GetComponent<Rigidbody>();
    }

    // 플레이어가 플랫폼에서 내려갔는지 감지
    private void OnCollisionExit(Collision collision)
    {
        PlayerMovement player =
            collision.collider.GetComponentInParent<PlayerMovement>();

        if (player == null)
            return;

        Rigidbody rigidbody =
            player.GetComponent<Rigidbody>();

        if (rigidbody == playerRigidbody)
        {
            playerRigidbody = null;
        }
    }

    // Scene View에서 이동 경로와 위치를 확인하기 위한 Gizmos
    private void OnDrawGizmos()
    {
        if (startPoint == null || endPoint == null)
            return;

        // 이동 경로
        Gizmos.DrawLine(
            startPoint.position,
            endPoint.position
        );

        // 시작 위치 표시
        Gizmos.DrawWireCube(
            startPoint.position,
            transform.localScale
        );

        // 끝 위치 표시
        Gizmos.DrawWireCube(
            endPoint.position,
            transform.localScale
        );

        // 시작 위치 중심
        Gizmos.DrawWireSphere(
            startPoint.position,
            0.25f
        );

        // 끝 위치 중심
        Gizmos.DrawWireSphere(
            endPoint.position,
            0.25f
        );
    }
}