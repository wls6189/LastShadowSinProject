using UnityEngine;
public enum EnemyAttackType
{
    Normal,
    Piercing,
}

public class PlayerStats : MonoBehaviour
{
    PlayerController player;

    float health;
    float spirit;
    float attackPower;
    void Awake()
    {
        TryGetComponent(out player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damaged(float damage, float impactForce, int groggyForce, EnemyAttackType type)
    {
        switch (type)
        {
            case EnemyAttackType.Normal:
                if (player.IsParring)
                {
                    player.Animator.SetTrigger("DoParry");
                    player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.groggyState);
                    // 데미지에 따른 체력 감소 로직
                    // 타격력에 따른 영혼 게이지 감소 로직
                    // 위력에 따른 그로기 여부
                    // groggyForce 4인 공격 시 DoParry가 아닌 DoHitShortGroggy.
                }
                else if (!player.IsParring && player.IsGuarding)
                {
                    player.Animator.SetTrigger("DoGuardHit");
                    player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.groggyState);
                }
                else
                {
                    player.Animator.SetTrigger("DoHitLongGroggy");
                    player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.groggyState);
                }
                break;
            case EnemyAttackType.Piercing:

                break;
        }
    }
}
