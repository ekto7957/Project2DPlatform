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
        // �Է°����� update���� ó��
        //FixedUpdate(): ���� ��� �� �̵� ó��
        movement.HandleMovement();

        if (Input.GetButtonDown("Fire1"))
        {
            attack.PerformAttack();
        }
    }
}