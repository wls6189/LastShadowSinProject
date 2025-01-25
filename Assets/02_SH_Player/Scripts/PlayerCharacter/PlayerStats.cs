using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    PlayerController player;

    // ü�� ���� �κ�
    public float MaxHealth;
    public float CurrentHealth; // ü��
    public float DamageReducePercent; // �÷��̾ �޴� ������ �������

    // ������ ���� �κ�
    public float MaxWillpower; 
    public float CurrentWillpower; // ������
    public float WillpowerRecoveryPerSec; // �ʴ� ������ ȸ����
    public float WillpowerRecoveryParry; // �и� �� ������ ȸ����
    public float WillpowerRecoveryPenetrate; // ���� �� ������ ȸ����
    public float WillpowerRecoverySpiritParry; // ��ȥ �и� �� ������ ȸ����
    public float PlayerWillpowerRecoveryIncreasePercent; // �÷��̾��� ������ ȸ�� ��������

    // ���ݷ� ���� �κ�
    public float AttackPower; // ���ݷ�

    // ��ȥ�� �ĵ� ���� �κ�
    public float MaxSpiritWave;
    public float CurrentSpiritWave; // ��ȥ�� �ĵ�

    // ������ ���� �κ�
    public int Tenacity; // 1, 2, 3, 4�� ����.
    public int GuardTenacity;
    public int ParryTenacity;

    // Ÿ�ݷ� ���� �κ�
    public float ImpactForceReducePercent; // �÷��̾ �޴� Ÿ�ݷ� �������
    public float PlayerImpactForceIncreasePercent; // �÷��̾ ���ϴ� Ÿ�ݷ� ��������
    
    void Awake()
    {
        TryGetComponent(out player);

        MaxHealth = 100; // �����ۿ� ���� ���� ����
        CurrentHealth = MaxHealth;
        DamageReducePercent = 0.9f;

        MaxWillpower = 150; // �����ۿ� ���� ���� ����
        CurrentWillpower = MaxWillpower;
        WillpowerRecoveryPerSec = 10f;
        WillpowerRecoveryParry = 3f;
        PlayerWillpowerRecoveryIncreasePercent = 0f;

        MaxSpiritWave = 12;
        CurrentSpiritWave = MaxSpiritWave; // ���� ���� �� ��ȥ�� �ĵ��� 0���� �ǰԲ� ���� ��

        Tenacity = 2;
        GuardTenacity = 3;
        ParryTenacity = 4;

        ImpactForceReducePercent = 0.7f;
    }

    void Update()
    {
        NaturalWillpowerRecovery();
        ManageMaxStats();
    }
    void NaturalWillpowerRecovery()
    {
        if (!player.IsInCombat)
        {
            CurrentWillpower += WillpowerRecoveryPerSec * Time.deltaTime;
        }
    }
    void ManageMaxStats()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        CurrentWillpower = Mathf.Clamp(CurrentWillpower, 0, MaxWillpower);
    }
    public void Damaged(float damage, float impactForce, int groggyForce, AttackType type, EnemyStats enemyStats = null, bool isDirectAttack = false)
    {
        if (isDirectAttack) // ���带 ���ؼ� ���� �÷��̾ ������ ��(������ �ݶ��̴��� Player �±װ� �ٷ� ���� ���)
        {
            if (groggyForce > Tenacity && !player.IsGrogging) // ������ �� �̻��� ��� �� �ൿ �Ҵ�
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
            }
            else if (groggyForce == Tenacity && !player.IsGrogging) // ������ ���� ��� ª�� �ൿ �Ҵ�
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
            }

            CurrentHealth -= damage; // ��������ŭ ü�� ����
            CurrentWillpower -= impactForce; // Ÿ�ݷ¸�ŭ ������ ����
        }
        else 
        {
            switch (type)
            {
                case AttackType.Normal:
                    if (player.IsParring || player.IsSpiritParring) // �и�
                    {
                        if (groggyForce >= ParryTenacity) // ������ �ֻ��� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                        else // ������ ��, ��, ���� ��� �и�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.parryState);
                        }

                        CurrentWillpower += (WillpowerRecoveryParry + WillpowerRecoveryParry * PlayerWillpowerRecoveryIncreasePercent); // ĳ������ ������ ȸ��
                        //enemyStats.CurrentWillpower -= 15; // ������ ������ ����
                    }
                    else if (player.IsAttackingParring) // �и� �ɷ��� �ִ� ����
                    {
                        if (groggyForce >= ParryTenacity) // ������ �ֻ��� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentWillpower += (WillpowerRecoveryParry + WillpowerRecoveryParry * PlayerWillpowerRecoveryIncreasePercent); // ĳ������ ������ ȸ��
                        //enemyStats.CurrentWillpower -= 15; // ������ ������ ����
                    }
                    else if (player.IsGuarding) // ����
                    {
                        if (groggyForce > GuardTenacity) // ������ �ֻ��� ��� �� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == GuardTenacity) // ������ ���� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                        else if (groggyForce < GuardTenacity) // ������ ��, ���� ��� ���� ��
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);
                        }

                        CurrentHealth -= (damage - damage * DamageReducePercent); // ����� ��������ŭ ü�� ����
                        CurrentWillpower -= (impactForce - impactForce * ImpactForceReducePercent); // ����� Ÿ�ݷ¸�ŭ ������ ����
                    }
                    else // �׳� �ǰ�(�����ϸ� isDirectAttack���� �ǰ��� ó�������� Ȥ�ó� ���� ��Ȳ�� ���)
                    {
                        if (groggyForce > Tenacity && !player.IsGrogging) // ������ �� �̻��� ��� �� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // ������ ���� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentHealth -= damage; // ��������ŭ ü�� ����
                        CurrentWillpower -= impactForce; // Ÿ�ݷ¸�ŭ ������ ����
                    }
                    break;
                case AttackType.Piercing:
                    if (player.CurrentPlayerState == PlayerState.Penetrate)
                    {
                        CurrentWillpower += (WillpowerRecoveryPenetrate + WillpowerRecoveryPenetrate * PlayerWillpowerRecoveryIncreasePercent); // ĳ������ ������ ȸ��
                        //enemyStats.CurrentWillpower -= 25; // ������ ������ ����

                    }
                    else // ���� ���� �ƴѵ� �ǰݵǸ� ������ �ǰ�
                    {
                        if (groggyForce > Tenacity && !player.IsGrogging) // ������ �� �̻��� ��� �� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // ������ ���� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentHealth -= damage; // ��������ŭ ü�� ����
                        CurrentWillpower -= impactForce; // Ÿ�ݷ¸�ŭ ������ ����
                    }
                    break;
                case AttackType.Spirit:
                    if (player.IsSpiritParring)
                    {
                        CurrentWillpower += (WillpowerRecoverySpiritParry + WillpowerRecoverySpiritParry * PlayerWillpowerRecoveryIncreasePercent); // ĳ������ ������ ȸ��
                        //enemyStats.CurrentWillpower -= 70; // ������ ������ ����
                    }
                    else // ��ȥ �и� ���� �ƴϸ� ������ �ǰ�
                    {
                        if (groggyForce > Tenacity && !player.IsGrogging) // ������ �� �̻��� ��� �� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // ������ ���� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentHealth -= damage; // ��������ŭ ü�� ����
                        CurrentWillpower -= impactForce; // Ÿ�ݷ¸�ŭ ������ ����
                    }
                    break;
                case AttackType.Grab:
                    player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.grabbedState); // ���Ϳ��� ���� �� �� �ൿ �Ҵ� �����̻� ��ȯ�� ȣ�����ָ� ��.
                    break;
                case AttackType.Clash:
                    if (player.IsClashGuard)
                    {

                    }
                    else
                    {
                        if (groggyForce > Tenacity && !player.IsGrogging) // ������ �� �̻��� ��� �� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // ������ ���� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentHealth -= damage; // ��������ŭ ü�� ����
                        CurrentWillpower -= impactForce; // Ÿ�ݷ¸�ŭ ������ ����
                    }
                    break;
            }
        }

        if (CurrentHealth <= 0)
        {
            player.CurrentPlayerState = PlayerState.Dead;
        }
    } 
}
