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
    //�⺻,��ȥ ���ݿ� ����
    private void OnTriggerEnter(Collider other)
    {
        GameObject target;     
        // ��ü �ݶ��̴����� Ȯ��
        if (other.CompareTag("Player"))
        {
            target = other.gameObject; // �ٷ� ���
            isDirectAttack = true;
        }
        else
        {
            return; // �ش� �±װ� �ƴ� ��� ó������ ����
        }

        // �̹� ���� ������� Ȯ��
        if (hitTargets.Contains(target))
        {
            return; // �̹� ó���� ����� ����
        }

        // ��� ���
        hitTargets.Add(target);

        // EnemyStats ������Ʈ ��������
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
        // ���� �ݶ��̴��� ���� �� ��� �ʱ�ȭ
        hitTargets.Clear();
        isDirectAttack = false;
    }
}