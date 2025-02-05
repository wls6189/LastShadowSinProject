using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using Unity.VisualScripting;

public class EnemyStats : MonoBehaviour
{
    public int monsterNumber; // ���ͺ� ���� ��ȣ
    public AudioClip[] monsterSfxClips;

    public float maxHealth;        // �ִ� ü��
    public float currentHealth;           // ���� ü��
    private Enemy enemy;
    public float maxWillpower;     // �ִ� ��ȥ ������
    public float currentWillpower;        // ���� ��ȥ ������
    
    private Animator animator;             // ���� �ִϸ�����
    public float attackPower ;
    public bool isGroggy = false;
    private bool isRecovering = false;
    public bool isDead = false;
    public TenacityAndGroggyForce tenacity;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image WillpowerBarImage;
    [SerializeField] private GameObject guardEffectPrefab;
    // �׷α� ���º� ���� �ð� ����



    private void Start()
    {
        animator = GetComponent<Animator>(); // �ִϸ����� �ʱ�ȭ
        enemy = GetComponent<Enemy>();
        currentHealth = maxHealth; // ���� ü���� �ִ� ü������ �ʱ�ȭ
        currentWillpower = maxWillpower; // ���� ��ȥ �������� �ִ� ��ȥ �������� �ʱ�ȭ
        isGroggy = false;

        AudioManager.instance.RegisterMonsterSfx(monsterNumber, monsterSfxClips);
    }
    private void Update()
    {
        healthBarImage.fillAmount = currentHealth / (float)maxHealth;
        WillpowerBarImage.fillAmount = currentWillpower / (float)maxWillpower;
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);


        if (currentState.IsName("Die") && currentState.normalizedTime >= 0.99f)
        {

            Destroy(gameObject, 5f); // 5�� �� ������Ʈ �ı�
        }
        else if (!animator.IsInTransition(0) &&
        (currentState.IsName("Knockdown") || currentState.IsName("ShortGroggy")) &&
        currentState.normalizedTime >= 0.9f)
        {
          
            isGroggy = false;
        }
        animator.SetFloat("Speed", enemy.navMeshAgent.velocity.magnitude);
    }

    public void PlayMonsterSfx(int sfxIndex)
    {
        AudioManager.instance.PlayMonsterSfx(monsterNumber, sfxIndex);
    }
    public void Damaged(float damage, float impactForce, TenacityAndGroggyForce groggyForce)
    {
        if (isDead || isRecovering || (enemy != null && enemy.currentState == Enemy.State.Parry))
        {
            return;
        }
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        // **�׷αⰡ �ƴϾ�߸� ���� ���� ����**
        if (enemy.isGuarding)
        {
            PlayMonsterSfx(0); // ��� ����
            AudioManager.instance.Playsfx(AudioManager.Sfx.Guard);
            animator.SetTrigger("Guard");
            damage *= 0.1f; // ������ 10%
            impactForce *= 0.3f; // �ҿ� ������ 30%
            isGroggy = false;
            if (guardEffectPrefab != null)
            {
                GameObject effect = Instantiate(guardEffectPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
                ParticleSystem particle = effect.GetComponent<ParticleSystem>();
                if (particle != null)
                {
                    particle.Play(); // ����Ʈ ����
                }
            }
            return;
        }


       
        currentHealth -= damage; // ü�� ����
        currentWillpower -= impactForce; // ��ȥ ������ ����
        PlayMonsterSfx(1); // �ǰ� ����
        AudioManager.instance.Playsfx(AudioManager.Sfx.Hit);
        if (currentHealth <= 0)
        {
            isGroggy = false;
            Die(); // ü���� 0�̸� ���� ó��
            return;
        }
        if (!isGroggy && !isDead) // �׾��� ���� �׷α� üũ X
        {
            DetermineGroggyState(groggyForce);
        }
        if (currentWillpower <= 0 && !isDead) // �׾��� ���� �׷α� ���� ���� X
        {
            EnterKnockdown();
        }
        // ü�°� ��ȥ �������� 0�� ���� �ʵ��� ����
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentWillpower = Mathf.Clamp(currentWillpower, 0, maxWillpower);
    }


    private void DetermineGroggyState(TenacityAndGroggyForce groggyForce)
    {
        if (groggyForce < tenacity) // Force�� �����Ժ��� ����
        {
            // �׷α� ���� ����
        }
        else if (groggyForce == tenacity) // Force�� �����԰� ����
        {
            animator.SetTrigger("ShortGroggy"); // ª�� �׷α�
            isGroggy = true;

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
        if (isDead) return;        
        isDead = true;
        PlayMonsterSfx(2); // ��� ����
        AudioManager.instance.Playsfx(AudioManager.Sfx.Dead);
        animator.ResetTrigger("Knockdown");
        animator.ResetTrigger("ShortGroggy");
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
        if (isRecovering)
        {
            return;
        }

        // ȸ�� ����
        isRecovering = true;
        StartCoroutine(RecoverHealthCoroutine());
    }
    private IEnumerator RecoverHealthCoroutine()
    {
        float recoveryDuration = 2f; // ȸ���� �ɸ��� �ð� (�� ����)
        float healthRecoverySpeed = (maxHealth - currentHealth) / recoveryDuration;
        float willpowerRecoverySpeed = (maxWillpower - currentWillpower) / recoveryDuration;

        // ü�°� ��ȥ �������� ������ ȸ��
        while (currentHealth < maxHealth || currentWillpower < maxWillpower)
        {
            currentHealth = Mathf.MoveTowards(currentHealth, maxHealth, healthRecoverySpeed * Time.deltaTime);
            currentWillpower = Mathf.MoveTowards(currentWillpower, maxWillpower, willpowerRecoverySpeed * Time.deltaTime);

           
            yield return null;
        }

        
        currentHealth = maxHealth;
        currentWillpower = maxWillpower;
        isRecovering = false;
    }
    public void OnDeathAnimationEnd()//�ִϸ��̼� �̺�Ʈ �߰�
    {
       
        Destroy(gameObject);
    }
   
}

