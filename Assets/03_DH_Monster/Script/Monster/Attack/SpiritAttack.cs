using UnityEngine;

public class SpiritAttack : MonoBehaviour
{
    public float damage = 10f; // 기본 공격력
    private bool parrySuccessful = false; // 패링 성공 여부


    private void OnTriggerEnter(Collider other)
    {

        PlayerController player = other.GetComponent<PlayerController>();
        //if (player != null && player.IsSpiritParrying)
        //{

        //    parrySuccessful = true; // 패링 성공 상태 기록
        //  //playerScript.OnParrySuccess(); // 플레이어에게 패링 성공 알림
        //    return; // 이후 처리 중단
        //}

        // Player 태그와 충돌 시
        if (other.CompareTag("Player"))
        {
            if (parrySuccessful)
            {
                Debug.Log("패링으로 인해 데미지가 무효화되었습니다.");
                parrySuccessful = false; // 상태 초기화
                return; // 데미지 처리 중단
            }

            Debug.Log("플레이어에게 데미지 " + damage);
            //other.GetComponent<Health>().Damage(damage); // 데미지 처리
        }
    }
}