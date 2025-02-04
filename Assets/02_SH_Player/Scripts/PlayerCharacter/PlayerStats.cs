using UnityEngine;

public enum TenacityAndGroggyForce
{
    Low,
    Medium,
    High,
    Ultimate
}

public class PlayerStats : MonoBehaviour
{
    PlayerController player;

    // ü�� ���� �κ�
    [HideInInspector] public float MaxHealth;
    [HideInInspector] public float CurrentHealth;
    [HideInInspector] public float DamageReducePercentage; // �ʱⰪ�� 0. ��ȥ���ο� ���� �����Ǵ� ����
    [HideInInspector] public float GuardDamageReducePercentage; // ���� �� �÷��̾ �޴� ������ �������

    // ���ݷ� ���� �κ�
    
    public float AttackPower; // ���ݷ�
    [HideInInspector] public float AttackPowerIncreasePercentage;

    // Ÿ�ݷ� ���� �κ�
    [HideInInspector] public float ParryImpactForcePercentage;
    [HideInInspector] public float PenetrateImpactForcePercentage;
    [HideInInspector] public float SpiritParryImpactForcePercentage;

    // ��ȥ�� �ĵ� ���� �κ�
    [HideInInspector] public float MaxSpiritWave;
    [HideInInspector] public float CurrentSpiritWave;
    [HideInInspector] public float RegenerationSpiritWavePerSec; // �ʴ� ȸ����
    [HideInInspector] public float RegenerationSpiritWaveIncreasePercent; // �ʴ� ȸ����
    [HideInInspector] public float ParrySpiritWaveRegeneration;
    [HideInInspector] public float GuardSpiritWaveRegeneration;
    [HideInInspector] public float PenetrateSpiritWaveRegeneration;
    [HideInInspector] public float SpiritParrySpiritWaveRegeneration;

    // ��ȥ���η� ���� �κ�
     public float MaxSpiritMarkForce;
     public float CurrentSpiritMarkForce;
    [HideInInspector] public float SpiritMarkForceGainIncreasePercentage; // ��ȥ���η� ȹ�� ���� ����

    // ������ ���� �κ�
    [HideInInspector] public TenacityAndGroggyForce Tenacity;
    [HideInInspector] public TenacityAndGroggyForce GuardTenacity;
    [HideInInspector] public TenacityAndGroggyForce ParryTenacity;

    // �� �� �κ�
    [HideInInspector] public bool IsImmune; // ���ݿ� �鿪 �������� ����
    [HideInInspector] public bool IsSteadfast; // ���⵵ �и������� �Ǵ��� ����
    [HideInInspector] public bool IsRavenous; // ���ָ� ������ ��ȥ���� ���� ����. ����
    [HideInInspector] public bool IsEnthusiastic; // �������� ������ ��ȥ���� ���� ����. ��ȥ�� �ĵ� ȸ��
    [HideInInspector] public bool IsRagingOn; // ����ġ�� ������ ��ȥ���� ���� ����. ����
    [HideInInspector] public int RagingStack; // ����ġ�� ������ ��ȥ���� ���� ����
    [HideInInspector] public float SpiritAsh;
    
    void Awake()
    {
        TryGetComponent(out player);
    }
    void Start()
    {
        //InitializeStats(); // �� ������ �ʱ�ȭ�� ��ȥ���� �κ��丮���� ����
        LoadStatDataWhenQuit(); // ���� ü��, ��ȥ�� �ĵ�, ��ȥ���η� �� ����� ��ġ�� �ҷ�����
    }

    void Update()
    {
        ManageNaturalRegeneration();
        ManageClampStats();
    }

    void ManageNaturalRegeneration()
    {
        CurrentSpiritWave += RegenerationSpiritWavePerSec * Time.deltaTime;
    }
    void ManageClampStats()
    {
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        CurrentSpiritWave = Mathf.Clamp(CurrentSpiritWave, 0, MaxSpiritWave);
        CurrentSpiritMarkForce = Mathf.Clamp(CurrentSpiritMarkForce, 0, MaxSpiritMarkForce);
    }
    public void InitializeStats()
    {
        MaxHealth = 100;

        DamageReducePercentage = 0;
        GuardDamageReducePercentage = 0.9f;

        AttackPower = 10;

        ParryImpactForcePercentage = 0.3f;
        PenetrateImpactForcePercentage = 0.5f;
        SpiritParryImpactForcePercentage = 3f;

        MaxSpiritWave = 10;

        RegenerationSpiritWavePerSec = 0.05f;
        RegenerationSpiritWaveIncreasePercent = 0;
        GuardSpiritWaveRegeneration = 0.1f;
        ParrySpiritWaveRegeneration = 0.3f;
        PenetrateSpiritWaveRegeneration = 0.5f;
        SpiritParrySpiritWaveRegeneration = 3f;

        MaxSpiritMarkForce = 100f;
        SpiritMarkForceGainIncreasePercentage = 0;

        Tenacity = TenacityAndGroggyForce.Medium;
        GuardTenacity = TenacityAndGroggyForce.High;
        ParryTenacity = TenacityAndGroggyForce.Ultimate;

        IsImmune = false;
        //SpiritAsh = ���� ���̺꿡�� ��������
    }
    public void LoadStatDataWhenQuit()
    {
        CurrentHealth = DataManager.Instance.nowPlayer.CurrentHealth;
        CurrentSpiritWave = DataManager.Instance.nowPlayer.CurrentSpiritWave;
        CurrentSpiritMarkForce = DataManager.Instance.nowPlayer.CurrentSpiritMarkForce;
        SpiritAsh = DataManager.Instance.nowPlayer.SpiritAshAmount;
    }
    public void Damaged(float damage, TenacityAndGroggyForce groggyForce, AttackType type, EnemyStats enemyStats = null, bool isDirectAttack = false)
    {
        if (IsImmune) return; // �鿪 ���¶�� ��� ���� ����
        if (player.OnDashEffect) return; // �뽬 �߿��� ����

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

            CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
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

                        //enemyStats.CurrentWillpower -= AttackPower * ParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += ParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();

                        if (player.PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !player.IsRadicalESMAttackPosture) // �ش����� ������ ��ȥ���� ���� �ڼ� �� �и��� ������ �ο�
                        {
                            //enemyStats.CurrentHealth -= AttackPower * 0.2f; // ������ ������ ����
                        }
                    }
                    else if (player.IsAttackingParryColliderEnabled) // �и� �ɷ��� �ִ� ����
                    {
                        if (player.IsSpiritSwordDanceSecondAttack) // ��ȥ �˹��� 2��° �������� �и��� �����ϸ� ��ȥ ������ 3��° ������ �ٷ� �� �� �ְ� �ȴ�.
                        {
                            player.CanSpiritCleave3Combo = true;
                        }

                        if (groggyForce >= ParryTenacity) // ������ �ֻ��� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        //enemyStats.CurrentWillpower -= AttackPower * ParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += ParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
                    }
                    else if (IsSteadfast) // ���� �ε��� ������ ��ȥ���� ���� �ÿ� ����� �и�ó��. ����� �����
                    {
                        if (groggyForce >= ParryTenacity) // ������ �ֻ��� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                        else // ������ ��, ��, ���� ��� �и�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);
                        }

                        //enemyStats.CurrentWillpower -= AttackPower * ParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += ParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
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

                        CurrentHealth -= damage * (1 - GuardDamageReducePercentage) * (1 - DamageReducePercentage); // ����� ��������ŭ ü�� ����
                        CurrentSpiritWave += GuardSpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
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

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
                    }
                    
                    break;
                case AttackType.Piercing:
                    if (player.CurrentPlayerState == PlayerState.Penetrate)
                    {
                        //enemyStats.CurrentWillpower -= AttackPower * PenetrateImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += PenetrateSpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();

                        if (player.PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !player.IsRadicalESMAttackPosture) // �ش����� ������ ��ȥ���� ���� �ڼ� �� �и��� ������ �ο�
                        {
                            //enemyStats.CurrentHealth -= AttackPower * 0.2f; // ������ ������ ����
                        }
                    }
                    else if (IsSteadfast) // ���� �ε��� ������ ��ȥ���� ���� �ÿ� ����� ���� ó��. ����� �����
                    {
                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);

                        //enemyStats.CurrentWillpower -= AttackPower * PenetrateImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += PenetrateSpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
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

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
                        
                    }
                    
                    break;
                case AttackType.Spirit:
                    if (player.IsSpiritParring)
                    {
                        //enemyStats.CurrentWillpower -= AttackPower * SpiritParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += SpiritParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();

                        if (player.PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !player.IsRadicalESMAttackPosture) // �ش����� ������ ��ȥ���� ���� �ڼ� �� �и��� ������ �ο�
                        {
                            //enemyStats.CurrentHealth -= AttackPower * 0.2f; // ������ ������ ����
                        }
                    }
                    else if (IsSteadfast)  // ���� �ε��� ������ ��ȥ���� ���� �ÿ� ����� ��ȥ�и� ó��. ����� �����
                    {
                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);

                        //enemyStats.CurrentWillpower -= AttackPower * SpiritParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += SpiritParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
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

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
                    }
                    break;
                case AttackType.Grab:
                    if (player.CurrentPlayerState != PlayerState.Grabbed)
                    {
                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.grabbedState); // ���Ϳ��� ���� �� �� �ൿ �Ҵ� �����̻� ��ȯ�� ȣ�����ָ� ��.

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
                    }
                    else // �׷� ���� ���� ������ ������ ����
                    {
                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
                    }
                    break;
                case AttackType.Clash:
                    if (player.IsClashGuard)
                    {
                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.clashGuardState); // ���Ϳ� ��� ó������ ���� ����
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

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
                    }
                    break;
            }
        }

        if (CurrentHealth <= 0)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.deadState);
            GetComponent<PlayerInteraction>().PlayerDie();
        }
    }
}
