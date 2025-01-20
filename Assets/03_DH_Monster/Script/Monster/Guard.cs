using UnityEngine;

public class Guard : MonoBehaviour
{
    private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>(); // 부모 오브젝트에서 EnemyHealth 가져오기
    }

    
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player.IsAttacking)
        {
            //플레이어 공격의 데미지와 영혼 데미지 받기
            //float damage = other.GetComponent<PlayerController>().damage;  // 플레이어 공격에서 데미지 가져오기
            //float soulDamage = other.GetComponent<PlayerController>().soulDamage; // 플레이어 공격에서 영혼 데미지 가져오기
            //float attackPower = other.GetComponent<PlayerController>().attackPower; // 공격력

            //가드 상태에서 데미지 처리(70 % 감소)
            //enemyHealth.TakeDamage(damage * 0.3f, soulDamage * 0.3f, attackPower);
        }
    }
}
