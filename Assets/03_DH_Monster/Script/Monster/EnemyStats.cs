using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    public float maxHealth;        // �ִ� ü��
    public float currentHealth;           // ���� ü��
    private Enemy enemy;
    public float maxSoulGauge;     // �ִ� ��ȥ ������
    public float currentSoulGauge;        // ���� ��ȥ ������
    public float toughness ;          // ������ ������ (1��,2��,3��,4�ֻ�)
    private Animator animator;             // ���� �ִϸ�����
    public float attackPower ;

    // �׷α� ���º� ���� �ð� ����

    [System.Serializable]
    public class ItemDrop
    {
        public GameObject itemPrefab; // ����� ������ ������
        public float dropChance; // ��� Ȯ��
    }
    public ItemDrop[] itemDrops; // ����� �����۵�

    private void Start()
    {
        animator = GetComponent<Animator>(); // �ִϸ����� �ʱ�ȭ
        currentHealth = maxHealth; // ���� ü���� �ִ� ü������ �ʱ�ȭ
        currentSoulGauge = maxSoulGauge; // ���� ��ȥ �������� �ִ� ��ȥ �������� �ʱ�ȭ
    }
    // �������� �޾��� �� ȣ��Ǵ� �޼���
    public void TakeDamage(float damage, float soulDamage, float playerForce)
    {
        DetermineGroggyState(playerForce); // �׷α� ���� ����

        currentHealth -= damage;          // ü�� ����
        currentSoulGauge -= soulDamage;   // ��ȥ ������ ����

        if (currentHealth <= 0)
        {
            Die();  // ü���� 0�̸� ���� ó��
        }

        if (currentSoulGauge <= 0)
        {
            EnterKnockdown() ;  // ��ȥ �������� 0�̸� �׷α� ����
        }

        currentHealth = Mathf.Max(currentHealth, 0);
        currentSoulGauge = Mathf.Max(currentSoulGauge, 0);
    }



    private void DetermineGroggyState(float playerForce)
    {
        if (playerForce < toughness) // Force�� �����Ժ��� ����
        {
            // �׷α� ���� ����
        }
        else if (playerForce == toughness) // Force�� �����԰� ����
        {
            animator.SetTrigger("ShortGroggy"); // ª�� �׷α�
        }
        else if (playerForce > toughness) // Force�� �����Ժ��� ŭ
        {
            animator.SetTrigger("Knockdown"); // �˴ٿ�
        }
    }


    // ���� ó��
    private void Die()
    {
        DropItem();
        animator.SetTrigger("Die");
    }

    // ��ȥ ������ ȸ�� �޼���

    public void EnterKnockdown()//�ܺ�ȣ���
    {
        animator.SetTrigger("Knockdown");



        // �ҿ� �������� 0�� ���� ȸ�� (������ 0���� ���� Knockdown�� ���)
        if (currentSoulGauge <= 0)
        {
            currentSoulGauge = maxSoulGauge; // �ҿ� �������� �ִ밪���� ȸ��
            
        }
    }
    public void EnterShortGroggy()//�ܺ�ȣ���
    {
        animator.SetTrigger("ShortGroggy");
    }


    private void DropItem()
    {
        foreach (var itemDrop in itemDrops)
        {
            if (Random.value <= itemDrop.dropChance) // �� �����ۺ� ��� Ȯ�� üũ
            {
                Instantiate(itemDrop.itemPrefab, transform.position, Quaternion.identity);
                
            }
        }
    }
   
    
    public void OnDeathAnimationEnd()//�ִϸ��̼� �̺�Ʈ �߰�
    {
        Destroy(gameObject);
    }
  
}

