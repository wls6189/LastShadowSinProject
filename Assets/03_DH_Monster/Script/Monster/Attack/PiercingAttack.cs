using System.Collections.Generic;
using UnityEngine;

public class PiercingAttack : MonoBehaviour
{
    public float damageMultiplier;
    public float impactForce;
    public float groggyForce;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public AttackType currentAttackType;
    private bool isDirectAttack = false;
    //기본,영혼 공격에 쓸거
    private void OnTriggerEnter(Collider other)
    {
        GameObject target;     
        // 본체 콜라이더인지 확인
        if (other.CompareTag("Player"))
        {
            target = other.gameObject; // 바로 대상
            isDirectAttack = true;
        }
        else
        {
            return; // 해당 태그가 아닌 경우 처리하지 않음
        }

        // 이미 맞춘 대상인지 확인
        if (hitTargets.Contains(target))
        {
            return; // 이미 처리된 대상은 무시
        }

        // 대상 기록
        hitTargets.Add(target);

        // EnemyStats 컴포넌트 가져오기
        EnemyStats enemyStats = GetComponent<EnemyStats>();

        if (enemyStats != null)
        {
            float finalDamage = enemyStats.attackPower * damageMultiplier;


            PlayerStats playerStats = target.GetComponent<PlayerStats>();
            //if (playerStats != null)
            //{
            //playerStats.Damaged(damage, impactForce, groggyForce,AttackType type, enemyStats, isDirectAttack);
            //}
        }
    }

    private void OnDisable()
    {
        // 공격 콜라이더가 꺼질 때 기록 초기화
        hitTargets.Clear();
        isDirectAttack = false;
    }
}