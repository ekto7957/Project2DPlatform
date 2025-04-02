using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerAnimation playerAnimation;
    private Animator animator;
    private Vector3 originalPosition;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;

    [Header("애니메이션 상태 이름")]
    public string attackStateName = "Attack";

    [Header("공격 모션 설정")]
    public float attackMoveDistance = 0.5f; // 공격 시 앞으로 이동할 거리
    public float attackMoveDuration = 0.1f; // 이동 지속 시간

    [Header("히트박스 설정")]
    public GameObject hitboxObject; // 공격 판정 오브젝트

    [Header("파티클 효과")]
    public ParticleSystem attackParticle; // 공격 파티클
    public Transform particleSpawnPoint; // 파티클 생성 위치

    void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 히트박스가 있으면 초기에 비활성화
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
        }

        // 파티클 스폰 포인트가 없으면 현재 오브젝트 위치 사용
        if (particleSpawnPoint == null)
        {
            particleSpawnPoint = transform;
        }
    }

    public void PerformAttack()
    {
        Debug.Log("공격명령 진입");
        if (isAttacking)
        {
            return;
        }

        // 현재 위치 저장
        originalPosition = transform.position;

        if (playerAnimation != null)
        {
            playerAnimation.TriggerAttack();
            Debug.Log("공격발동");
        }

        StartCoroutine(AttackCooldownByAimation());
    }

    // 애니메이션 이벤트로 호출될 메서드: 앞으로 이동
    public void MoveForward()
    {
        // 캐릭터가 바라보는 방향으로 이동 (SpriteRenderer.flipX 사용)
        Vector3 direction = spriteRenderer.flipX ? Vector3.left : Vector3.right;

        // 새 목표 위치 계산
        Vector3 targetPosition = originalPosition + direction * attackMoveDistance;

        // 이동 코루틴 시작
        StartCoroutine(MoveToPosition(targetPosition, attackMoveDuration));
    }

    // 애니메이션 이벤트로 호출될 메서드: 원래 위치로 돌아가기
    public void MoveBack()
    {
        // 정확히 원래 위치로 돌아가기 위해 상대적 계산이 아닌 직접 저장된 위치 사용
        StartCoroutine(MoveToPosition(originalPosition, attackMoveDuration));
    }

    // 애니메이션 이벤트로 호출될 메서드: 히트박스 활성화 및 파티클 생성
    public void EnableHitbox()
    {
        if (hitboxObject != null)
        {
            // 히트박스 방향 조정 (오브젝트가 자식일 경우)
            Vector3 hitboxPosition = hitboxObject.transform.localPosition;
            if (spriteRenderer.flipX)
            {
                hitboxPosition.x = -Mathf.Abs(hitboxPosition.x);
            }
            else
            {
                hitboxPosition.x = Mathf.Abs(hitboxPosition.x);
            }
            hitboxObject.transform.localPosition = hitboxPosition;

            hitboxObject.SetActive(true);
        }

        // 파티클 효과 재생
        PlayAttackParticle();
    }

    // 애니메이션 이벤트로 호출될 메서드: 히트박스 비활성화
    public void DisableHitbox()
    {
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
        }
    }

    // 파티클 효과 재생
    private void PlayAttackParticle()
    {
        if (attackParticle != null)
        {
            // 방향에 따라 파티클 방향 설정
            Vector3 direction = spriteRenderer.flipX ? Vector3.left : Vector3.right;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            // 왼쪽을 볼 때는 파티클 회전
            if (spriteRenderer.flipX)
            {
                rotation *= Quaternion.Euler(0, 180, 0);
            }

            // 파티클 생성 및 재생
            ParticleSystem newParticle = Instantiate(
                attackParticle,
                particleSpawnPoint.position,
                rotation
            );

            // 몇 초 후 파티클 삭제
            Destroy(newParticle.gameObject, 2f);
        }
    }

    // 특정 위치로 이동하는 코루틴 (상대적 이동이 아닌 절대 위치)
    private IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 정확한 위치 설정
        transform.position = targetPos;
    }

    private IEnumerator AttackCooldownByAimation()
    {
        isAttacking = true;

        yield return null;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(attackStateName))
        {
            float animationLength = stateInfo.length;
            yield return new WaitForSeconds(animationLength);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 공격 종료 시 정확하게 원래 위치로 돌아가도록 보장
        transform.position = originalPosition;
        isAttacking = false;
    }
}