using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerAnimation playerAnimation;
    private Animator animator;
    private Vector3 originalPosition;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;

    [Header("�ִϸ��̼� ���� �̸�")]
    public string attackStateName = "Attack";

    [Header("���� ��� ����")]
    public float attackMoveDistance = 0.5f; // ���� �� ������ �̵��� �Ÿ�
    public float attackMoveDuration = 0.1f; // �̵� ���� �ð�

    [Header("��Ʈ�ڽ� ����")]
    public GameObject hitboxObject; // ���� ���� ������Ʈ

    [Header("��ƼŬ ȿ��")]
    public ParticleSystem attackParticle; // ���� ��ƼŬ
    public Transform particleSpawnPoint; // ��ƼŬ ���� ��ġ

    void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ��Ʈ�ڽ��� ������ �ʱ⿡ ��Ȱ��ȭ
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
        }

        // ��ƼŬ ���� ����Ʈ�� ������ ���� ������Ʈ ��ġ ���
        if (particleSpawnPoint == null)
        {
            particleSpawnPoint = transform;
        }
    }

    public void PerformAttack()
    {
        Debug.Log("���ݸ�� ����");
        if (isAttacking)
        {
            return;
        }

        // ���� ��ġ ����
        originalPosition = transform.position;

        if (playerAnimation != null)
        {
            playerAnimation.TriggerAttack();
            Debug.Log("���ݹߵ�");
        }

        StartCoroutine(AttackCooldownByAimation());
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ��� �޼���: ������ �̵�
    public void MoveForward()
    {
        // ĳ���Ͱ� �ٶ󺸴� �������� �̵� (SpriteRenderer.flipX ���)
        Vector3 direction = spriteRenderer.flipX ? Vector3.left : Vector3.right;

        // �� ��ǥ ��ġ ���
        Vector3 targetPosition = originalPosition + direction * attackMoveDistance;

        // �̵� �ڷ�ƾ ����
        StartCoroutine(MoveToPosition(targetPosition, attackMoveDuration));
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ��� �޼���: ���� ��ġ�� ���ư���
    public void MoveBack()
    {
        // ��Ȯ�� ���� ��ġ�� ���ư��� ���� ����� ����� �ƴ� ���� ����� ��ġ ���
        StartCoroutine(MoveToPosition(originalPosition, attackMoveDuration));
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ��� �޼���: ��Ʈ�ڽ� Ȱ��ȭ �� ��ƼŬ ����
    public void EnableHitbox()
    {
        if (hitboxObject != null)
        {
            // ��Ʈ�ڽ� ���� ���� (������Ʈ�� �ڽ��� ���)
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

        // ��ƼŬ ȿ�� ���
        PlayAttackParticle();
    }

    // �ִϸ��̼� �̺�Ʈ�� ȣ��� �޼���: ��Ʈ�ڽ� ��Ȱ��ȭ
    public void DisableHitbox()
    {
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
        }
    }

    // ��ƼŬ ȿ�� ���
    private void PlayAttackParticle()
    {
        if (attackParticle != null)
        {
            // ���⿡ ���� ��ƼŬ ���� ����
            Vector3 direction = spriteRenderer.flipX ? Vector3.left : Vector3.right;
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            // ������ �� ���� ��ƼŬ ȸ��
            if (spriteRenderer.flipX)
            {
                rotation *= Quaternion.Euler(0, 180, 0);
            }

            // ��ƼŬ ���� �� ���
            ParticleSystem newParticle = Instantiate(
                attackParticle,
                particleSpawnPoint.position,
                rotation
            );

            // �� �� �� ��ƼŬ ����
            Destroy(newParticle.gameObject, 2f);
        }
    }

    // Ư�� ��ġ�� �̵��ϴ� �ڷ�ƾ (����� �̵��� �ƴ� ���� ��ġ)
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

        // ��Ȯ�� ��ġ ����
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

        // ���� ���� �� ��Ȯ�ϰ� ���� ��ġ�� ���ư����� ����
        transform.position = originalPosition;
        isAttacking = false;
    }
}