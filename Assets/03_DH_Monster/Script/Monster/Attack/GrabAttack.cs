using UnityEngine;

public class GrabAttack : MonoBehaviour
{
    public Transform grabPoint; // 플레이어를 고정할 위치
    public float grabDuration = 2.0f; // 잡기 지속 시간
    private Animator animator; // Animator 컴포넌트 참조
    public float damage = 20f; // 잡기 공격 데미지

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
                player.OnGrabbed(grabPoint, grabDuration);
                other.GetComponent<Health>().Damage(damage); // 데미지 처리
            }
        }
    }
}