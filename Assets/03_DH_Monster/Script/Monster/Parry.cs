using UnityEngine;

public class Parry : MonoBehaviour
{
    private EnemyStats enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponentInParent<EnemyStats>(); // �θ� ������Ʈ���� EnemyHealth ��������

    }

    // �и� �ݶ��̴��� �浹 ��
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player.IsAttacking)
        {
            // �÷��̾� ������ �������� ��ȥ ������ �ޱ�
            //float damage = other.GetComponent<PlayerController>().damage;
            //float soulDamage = other.GetComponent<PlayerController>().soulDamage;
            //float attackPower = other.GetComponent<PlayerController>().attackPower;

            // �и� ���¿��� ������ ������ ����
            //enemyHealth.Damaged(0f, 0f, 0f);
            //�÷��̾�� �� �׷α�� �ϱ�
        }
    }
}
