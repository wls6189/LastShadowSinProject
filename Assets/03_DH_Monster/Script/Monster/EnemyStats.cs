using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using Unity.VisualScripting;

public class EnemyStats : MonoBehaviour
{
    public float maxHealth;        // �ִ� ü��
    public float currentHealth;           // ���� ü��
    private Enemy enemy;
    public float maxWillpower;     // �ִ� ��ȥ ������
    public float currentWillpower;        // ���� ��ȥ ������
    public float tenacity ;          // ������ ������ (1��,2��,3��,4�ֻ�)
    private Animator animator;             // ���� �ִϸ�����
    public float attackPower ;
    private bool isGroggy = false;
    public TenacityAndGroggyForce groggyForce;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image WillpowerBarImage;

    // �׷α� ���º� ���� �ð� ����



    private void Start()
    {
        animator = GetComponent<Animator>(); // �ִϸ����� �ʱ�ȭ
        currentHealth = maxHealth; // ���� ü���� �ִ� ü������ �ʱ�ȭ
        currentWillpower = maxWillpower; // ���� ��ȥ �������� �ִ� ��ȥ �������� �ʱ�ȭ
        isGroggy = false;
    }
    private void Update()
    {
        healthBarImage.fillAmount = currentHealth / (float)maxHealth;
        WillpowerBarImage.fillAmount = currentWillpower/(float)maxWillpower;
    }


    public void Damaged(float damage, float impactForce, float groggyForce, bool isdirectattack)
    {
       
        if (enemy != null && enemy.currentState == Enemy.State.Parry)
        {
    
            return; 
        }


        if (!isdirectattack || (enemy != null && !enemy.isAttacking))
        {
            damage *= 0.1f; // ������ 10%
            impactForce *= 0.3f; // �ҿ� ������ 30%
        }
        if (!isGroggy)
        {
            DetermineGroggyState(groggyForce);
        }
        currentHealth -= damage; // ü�� ����
        currentWillpower -= impactForce; // ��ȥ ������ ����

        if (currentHealth <= 0)
        {
            Die(); // ü���� 0�̸� ���� ó��
        }

        if (currentWillpower <= 0)
        {
            EnterKnockdown(); // ��ȥ �������� 0�̸� �׷α� ����
        }

        // ü�°� ��ȥ �������� 0�� ���� �ʵ��� ����
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentWillpower = Mathf.Clamp(currentWillpower, 0, maxWillpower);
    }


    private void DetermineGroggyState(float groggyForce)
    {
        if (groggyForce < tenacity) // Force�� �����Ժ��� ����
        {
            // �׷α� ���� ����
        }
        else if (groggyForce == tenacity) // Force�� �����԰� ����
        {
            animator.SetTrigger("ShortGroggy"); // ª�� �׷α�
        }
        else if (groggyForce > tenacity) // Force�� �����Ժ��� ŭ
        {
            animator.SetTrigger("Knockdown"); // �˴ٿ�
            isGroggy = true;
        }
    }


    // ���� ó��
    private void Die()
    {
       
        animator.SetTrigger("Die");
        MonsterDrop monsterDrop = GetComponent<MonsterDrop>();
        if (monsterDrop != null)
        {
            monsterDrop.Drop(); // ��� ������ ����
        }
    }

    // ��ȥ ������ ȸ�� �޼���

    public void EnterKnockdown()//�ܺ�ȣ���
    {
        if (isGroggy) return;

        animator.SetTrigger("Knockdown");
        isGroggy = true;


        // �ҿ� �������� 0�� ���� ȸ�� (������ 0���� ���� Knockdown�� ���)
        if (currentWillpower <= 0)
        {
            currentWillpower = maxWillpower; // �ҿ� �������� �ִ밪���� ȸ��
            
        }
    }
    public void EnterShortGroggy()//�ܺ�ȣ���
    {
        if (isGroggy) return;
        animator.SetTrigger("ShortGroggy");
    }


   
    public void RecoverHealth()
    {
        
        currentHealth = maxHealth;
        currentWillpower = maxWillpower;
    }

    public void OnDeathAnimationEnd()//�ִϸ��̼� �̺�Ʈ �߰�
    {
        Destroy(gameObject);
    }
    public void ExitGroggy()
    {
        isGroggy = false;
    }

}

