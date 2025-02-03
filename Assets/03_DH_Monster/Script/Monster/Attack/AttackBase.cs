using UnityEngine;
using System.Collections.Generic;

public class AttackBase : MonoBehaviour
{
    public float damageMultiplier;
    public TenacityAndGroggyForce groggyForce;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    public AttackType currentAttackType;
    private bool isDirectAttack = false;
    //�⺻,��ȥ ���ݿ� ����
    private void OnTriggerEnter(Collider other)
    {
        GameObject target;
        
        // ���� �ݶ��̴����� Ȯ��
        if (other.CompareTag("PlayerGuard"))
        {
            target = other.transform.parent.gameObject; // �θ�(GameObject)�� ������
        }
        // ��ü �ݶ��̴����� Ȯ��
        else if (other.CompareTag("Player"))
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
            //   playerStats.Damaged(finalDamage, groggyForce, currentAttackType, enemyStats, isDirectAttack);
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