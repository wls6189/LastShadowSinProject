using System.Collections;
using UnityEngine;

public class CrashAttack : MonoBehaviour
{
    public Collider aoeCollider; // 광역 데미지 콜라이더
    public float aoeDamage = 50f; // 광역 데미지
    public float aoeActiveTime = 0.5f; // 광역 데미지 활성화 시간
    public float clashDuration = 3.0f; // 격돌 진행 시간

    private bool isClashActive = false; // 현재 격돌 상태

    private void Start()
    {
        // 광역 데미지 콜라이더 초기화
        if (aoeCollider == null)
        {
            Debug.LogError("AOE Collider is not assigned!");
            return;
        }

        aoeCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; // 플레이어가 아닌 경우 무시

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.IsReadyForClash()) // 플레이어가 격돌 준비 상태인지 확인
        {
            Debug.Log("Player is ready for Clash. Starting Clash Event...");
            StartClash(player);
        }
        else
        {
            Debug.Log("Player failed to defend Clash!");
            FailClash(player);
        }
    }

    private void StartClash(PlayerController player)
    {
        if (isClashActive) return; // 중복 실행 방지
        isClashActive = true;
        PlayerController.clashSuccess = false; // 격돌 성공 플래그 초기화
        // 플레이어와 격돌 이벤트 처리
        player.StartClashEvent(this);


        // 제한 시간 후 격돌 결과 확인
        StartCoroutine(DelayedCheckClashResult(player, clashDuration));
    }
    private IEnumerator DelayedCheckClashResult(PlayerController player, float delay)
    {
        yield return new WaitForSeconds(delay); // 지정된 시간만큼 대기
        CheckClashResult(player); // 결과 확인 메서드 호출
    }

    private void CheckClashResult(PlayerController player)
    {
        if (!isClashActive) return; // 격돌이 활성화되지 않았다면 무시
        isClashActive = false; // 격돌 종료

        if (PlayerController.clashSuccess)
        {
            Debug.Log("Player defended the Clash Attack successfully!");
            EndClash(true);
        }
        else
        {
            Debug.Log("Player failed to defend the Clash Attack!");
            FailClash(player); // 이미 참조된 플레이어를 전달
        }
    }

    private void EndClash(bool success)
    {
        if (success)
        {
            Debug.Log("Clash successfully defended! Enemy enters Groggy State.");
            // 적 그로기 상태 처리 로직 추가
        }
    }

    private void FailClash(PlayerController player)
    {
        if (player == null) return;

        Debug.Log("Clash Failed! Applying AOE Damage to Player.");

        if (!aoeCollider.enabled) // 중복 활성화 방지
        {
            ToggleAOECollider(true);

            // 일정 시간 후 광역 데미지 비활성화
            Invoke(nameof(DisableAOEDamage), aoeActiveTime);
        }

        // 플레이어에게 데미지 적용
        player.TakeDamage(aoeDamage);
    }

    private void ToggleAOECollider(bool state)
    {
        if (aoeCollider != null)
        {
            aoeCollider.enabled = state;
            Debug.Log($"AOE Collider {(state ? "Activated" : "Deactivated")}!");
        }
    }

    private void DisableAOEDamage()
    {
        ToggleAOECollider(false);
    }
}
