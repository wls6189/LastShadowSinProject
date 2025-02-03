using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using Unity.VisualScripting;

public class EnemyStats : MonoBehaviour
{
    public float maxHealth;        // 최대 체력
    public float currentHealth;           // 현재 체력
    private Enemy enemy;
    public float maxWillpower;     // 최대 영혼 게이지
    public float currentWillpower;        // 현재 영혼 게이지
    
    private Animator animator;             // 몬스터 애니메이터
    public float attackPower ;
    private bool isGroggy = false;
    private bool isRecovering = false;
    public TenacityAndGroggyForce tenacity;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image WillpowerBarImage;

    // 그로기 상태별 지속 시간 설정



    private void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 초기화
        currentHealth = maxHealth; // 현재 체력을 최대 체력으로 초기화
        currentWillpower = maxWillpower; // 현재 영혼 게이지를 최대 영혼 게이지로 초기화
        isGroggy = false;
    }
    private void Update()
    {
        healthBarImage.fillAmount = currentHealth / (float)maxHealth;
        WillpowerBarImage.fillAmount = currentWillpower/(float)maxWillpower;
    }


    public void Damaged(float damage, float impactForce, TenacityAndGroggyForce groggyForce, bool isdirectattack)
    {
        if (isRecovering)
        {
            return;
        }
        if (enemy != null && enemy.currentState == Enemy.State.Parry)
        {
    
            return; 
        }

        if (enemy.isGuarding)
        {
            animator.SetTrigger("Guard");
        }
            if (!isdirectattack || (enemy != null && !enemy.isAttacking))
        {
            damage *= 0.1f; // 데미지 10%
            impactForce *= 0.3f; // 소울 데미지 30%
        }
        if (!isGroggy)
        {
            DetermineGroggyState(groggyForce);
        }
        currentHealth -= damage; // 체력 감소
        currentWillpower -= impactForce; // 영혼 게이지 감소

        if (currentHealth <= 0)
        {
            Die(); // 체력이 0이면 죽음 처리
        }

        if (currentWillpower <= 0)
        {
            EnterKnockdown(); // 영혼 게이지가 0이면 그로기 상태
        }

        // 체력과 영혼 게이지가 0을 넘지 않도록 보정
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentWillpower = Mathf.Clamp(currentWillpower, 0, maxWillpower);
    }


    private void DetermineGroggyState(TenacityAndGroggyForce groggyForce)
    {
        if (groggyForce < tenacity) // Force가 강인함보다 낮음
        {
            // 그로기 상태 없음
        }
        else if (groggyForce == tenacity) // Force가 강인함과 같음
        {
            animator.SetTrigger("ShortGroggy"); // 짧은 그로기
        }
        else if (groggyForce > tenacity) // Force가 강인함보다 큼
        {
            animator.SetTrigger("Knockdown"); // 넉다운
            isGroggy = true;
        }
    }


    // 죽음 처리
    private void Die()
    {
       
        animator.SetTrigger("Die");
        MonsterDrop monsterDrop = GetComponent<MonsterDrop>();
        if (monsterDrop != null)
        {
            monsterDrop.Drop(); // 드롭 아이템 생성
        }
    }

    // 영혼 게이지 회복 메서드

    public void EnterKnockdown()//외부호출용
    {
        if (isGroggy) return;

        animator.SetTrigger("Knockdown");
        isGroggy = true;


        // 소울 게이지가 0일 때만 회복 (게이지 0으로 인한 Knockdown일 경우)
        if (currentWillpower <= 0)
        {
            currentWillpower = maxWillpower; // 소울 게이지를 최대값으로 회복
            
        }
    }
    public void EnterShortGroggy()//외부호출용
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

        // 회복 시작
        isRecovering = true;
        StartCoroutine(RecoverHealthCoroutine());
    }
    private IEnumerator RecoverHealthCoroutine()
    {
        float recoveryDuration = 2f; // 회복에 걸리는 시간 (초 단위)
        float healthRecoverySpeed = (maxHealth - currentHealth) / recoveryDuration;
        float willpowerRecoverySpeed = (maxWillpower - currentWillpower) / recoveryDuration;

        // 체력과 영혼 게이지를 서서히 회복
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
    public void OnDeathAnimationEnd()//애니메이션 이벤트 추가
    {
        Destroy(gameObject,5f);
    }
    public void ExitGroggy()
    {
        isGroggy = false;
    }

}

