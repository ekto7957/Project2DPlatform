using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerAttack attack;
    private PlayerHealth health;
    private PlayerAnimation animation; 

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attack = GetComponent<PlayerAttack>();
        health = GetComponent<PlayerHealth>();
        animation = GetComponent<PlayerAnimation>();
    }

    void Start()
    {

    }

    void Update()
    {
        // 입력감지는 update에서 처리
        //FixedUpdate(): 물리 계산 및 이동 처리
        movement.HandleMovement();

        if (Input.GetButtonDown("Fire1"))
        {
            attack.PerformAttack();
        }
    }
}