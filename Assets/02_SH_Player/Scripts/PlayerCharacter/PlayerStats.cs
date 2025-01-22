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
                    // �������� ���� ü�� ���� ����
                    // Ÿ�ݷ¿� ���� ��ȥ ������ ���� ����
                    // ���¿� ���� �׷α� ����
                    // groggyForce 4�� ���� �� DoParry�� �ƴ� DoHitShortGroggy.
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
