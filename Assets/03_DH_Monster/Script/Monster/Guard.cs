using UnityEngine;

public class Guard : MonoBehaviour
{
    private EnemyStats enemyHealth;

    private void Start()
    {
        enemyHealth = GetComponentInParent<EnemyStats>(); // �θ� ������Ʈ���� EnemyHealth ��������
    }

    
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player.IsAttacking)
        {
            
            //�÷��̾� ������ �������� ��ȥ ������ �ޱ�
            //float damage = other.GetComponent<PlayerController>().damage;  // �÷��̾� ���ݿ��� ������ ��������
            //float soulDamage = other.GetComponent<PlayerController>().soulDamage; // �÷��̾� ���ݿ��� ��ȥ ������ ��������
            //float attackPower = other.GetComponent<PlayerController>().attackPower; // ���ݷ�

            //���� ���¿��� ������ ó��(70 % ����)
            //enemyHealth.TakeDamage(damage * 0.3f, soulDamage * 0.3f, attackPower);
        }
    }
}
