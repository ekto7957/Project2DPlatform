using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void SetWalking(bool isWalking)
    {
        animator.SetBool("IsWalking", isWalking);
    }

    public void SetJumping(bool isJumping)
    {
        animator.SetBool("IsJumping", isJumping);
       

    }

    public void SetFalling(bool isFalling)
    {
        animator.SetBool("IsFalling", isFalling);
    }

    public void PlayLanding()
    {
        animator.SetTrigger("Land");
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
    }

}