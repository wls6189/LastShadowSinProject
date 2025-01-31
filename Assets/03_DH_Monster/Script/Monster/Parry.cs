using UnityEngine;

public class Parry : MonoBehaviour
{
    private EnemyStats enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponentInParent<EnemyStats>(); // 부모 오브젝트에서 EnemyHealth 가져오기

    }

    // 패리 콜라이더와 충돌 시
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player.IsAttacking)
        {
            // 플레이어 공격의 데미지와 영혼 데미지 받기
            //float damage = other.GetComponent<PlayerController>().damage;
            //float soulDamage = other.GetComponent<PlayerController>().soulDamage;
            //float attackPower = other.GetComponent<PlayerController>().attackPower;

            // 패리 상태에서 데미지 완전히 차단
            //enemyHealth.Damaged(0f, 0f, 0f);
            //플레이어에게 너 그로기야 하기
        }
    }
}
