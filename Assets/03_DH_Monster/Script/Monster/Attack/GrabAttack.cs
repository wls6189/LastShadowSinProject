using UnityEngine;

public class GrabAttack : MonoBehaviour
{
    public Transform grabPoint; // 플레이어를 고정할 위치
    public float grabDuration = 2.0f; // 잡기 지속 시간
    private Animator animator; // Animator 컴포넌트 참조
    public float damageMultiplier;

    private void Start()
    {      
        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어인지 확인
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // 애니메이션 트리거 활성화
                animator.SetTrigger("GrabSuccess");
                // 플레이어 잡기 실행
                //player.OnGrabbed(grabPoint, grabDuration);
                //EnemyStats enemyStats = GetComponent<EnemyStats>();
                //if (enemyStats != null)
                //{
                //    // 최종 데미지 계산
                //    float finalDamage = enemyStats.attackPower * damageMultiplier;

                //    // 플레이어의 PlayerStats 가져오기
                //    PlayerStats playerStats = other.GetComponent<PlayerStats>();
                //    if (playerStats != null)
                //    {
                //        // 플레이어에게 최종 데미지 적용
                //        playerStats.Damaged(finalDamage);
                    }
                }
            }
        }
    