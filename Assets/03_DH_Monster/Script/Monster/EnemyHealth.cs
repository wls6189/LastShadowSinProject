using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;        // 최대 체력
    public float currentHealth;           // 현재 체력
    private Enemy enemy;
    public float maxSoulGauge;     // 최대 영혼 게이지
    public float currentSoulGauge;        // 현재 영혼 게이지
    public float toughness = 10f;          // 몬스터의 강인함 (예: 10) 
    private Animator animator;             // 몬스터 애니메이터
   

    // 그로기 상태별 지속 시간 설정
    
    [System.Serializable]
    public class ItemDrop
    {
        public GameObject itemPrefab; // 드랍할 아이템 프리팹
        public float dropChance; // 드랍 확률
    }
    public ItemDrop[] itemDrops; // 드랍할 아이템들

    private void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 초기화
        currentHealth = maxHealth; // 현재 체력을 최대 체력으로 초기화
        currentSoulGauge = maxSoulGauge; // 현재 영혼 게이지를 최대 영혼 게이지로 초기화
    }
    // 데미지를 받았을 때 호출되는 메서드
    public void TakeDamage(float damage, float soulDamage, float attackPower)
    {
        DetermineGroggyState(attackPower); // 그로기 상태 결정

        currentHealth -= damage;          // 체력 감소
        currentSoulGauge -= soulDamage;   // 영혼 게이지 감소

        if (currentHealth <= 0)
        {
            Die();  // 체력이 0이면 죽음 처리
        }

        if (currentSoulGauge <= 0)
        {
            EnterKnockdown() ;  // 영혼 게이지가 0이면 그로기 상태
        }

        currentHealth = Mathf.Max(currentHealth, 0);
        currentSoulGauge = Mathf.Max(currentSoulGauge, 0);
    }

  

    private void DetermineGroggyState(float attackPower)
    {
        if (attackPower <= toughness * 0.25f)
        {
            
        }
        else if (attackPower <= toughness * 0.5f)
        {
            animator.SetTrigger("ShortGroggy");
        }
        else
        {
            animator.SetTrigger("Knockdown");
        }
    }


    // 죽음 처리
    private void Die()
    {
        DropItem();
        animator.SetTrigger("Die");
    }

    // 영혼 게이지 회복 메서드

    public void EnterKnockdown()//외부호출용
    {
        animator.SetTrigger("Knockdown");



        // 소울 게이지가 0일 때만 회복 (게이지 0으로 인한 Knockdown일 경우)
        if (currentSoulGauge <= 0)
        {
            currentSoulGauge = maxSoulGauge; // 소울 게이지를 최대값으로 회복
            
        }
    }
    public void EnterShortGroggy()//외부호출용
    {
        animator.SetTrigger("ShortGroggy");
    }


    private void DropItem()
    {
        foreach (var itemDrop in itemDrops)
        {
            if (Random.value <= itemDrop.dropChance) // 각 아이템별 드랍 확률 체크
            {
                Instantiate(itemDrop.itemPrefab, transform.position, Quaternion.identity);
                
            }
        }
    }
   
    
    public void OnDeathAnimationEnd()//애니메이션 이벤트 추가
    {
        Destroy(gameObject);
    }
  
}

