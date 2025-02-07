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
    [HideInInspector] public float MaxHealthReducePercentage; // �ִ�ü�� ���� ����
    [HideInInspector] public float MaxHealthIncreaseAmount; // �ִ�ü�� ������

    // ���ݷ� ���� �κ�

    public float AttackPower; // ���ݷ�
    [HideInInspector] public float AttackPowerIncreasePercentage;
    [HideInInspector] public float AttackPowerIncreaseAmount;

    // Ÿ�ݷ� ���� �κ�
    [HideInInspector] public float ParryImpactForcePercentage;
    [HideInInspector] public float PenetrateImpactForcePercentage;
    [HideInInspector] public float SpiritParryImpactForcePercentage;

    // ��ȥ�� �ĵ� ���� �κ�
    [HideInInspector] public float MaxSpiritWave;
    [HideInInspector] public float CurrentSpiritWave;
    [HideInInspector] public float RegenerationSpiritWavePerSec; // �ʴ� ȸ����
    [HideInInspector] public float RegenerationSpiritWaveIncreasePercent; // ��ȥ�� �ĵ� ȸ���� ������
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
        // ü��
        MaxHealth = 100;
        DamageReducePercentage = 0;
        GuardDamageReducePercentage = 0.9f;
        MaxHealthReducePercentage = 0;
        MaxHealthIncreaseAmount = 0;

        // ���ݷ�
        AttackPower = 10;
        AttackPowerIncreaseAmount = 0;
        AttackPowerIncreasePercentage = 0;

        // Ÿ�ݷ�(�� ���ݷ¿� ���� ����)
        ParryImpactForcePercentage = 0.3f;
        PenetrateImpactForcePercentage = 0.5f;
        SpiritParryImpactForcePercentage = 3f;

        // ��ȥ�� �ĵ�
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

        // �ΰ����� ��
        player.PlayerChaliceOfAtonement.RecoveryAmountReducePercentage = 0;
    }
    public void CalculateStatsChange()
    {
        AttackPower += AttackPowerIncreaseAmount;
        AttackPower += (AttackPower * AttackPowerIncreasePercentage);
        MaxHealth += MaxHealthIncreaseAmount;
        MaxHealth = MaxHealth * GuardDamageReducePercentage;
        player.PlayerChaliceOfAtonement.LoadCOAData();
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
            CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
            AudioManager.instance.Playsfx(AudioManager.Sfx.PlayerDamageVoice);
            if (groggyForce > Tenacity && player.CurrentPlayerState != PlayerState.HitLongGroggy) // ������ �� �̻��� ��� �� �ൿ �Ҵ�
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
            }
            else if (groggyForce == Tenacity) // ������ ���� ��� ª�� �ൿ �Ҵ�
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
            }
        }
        else 
        {
            switch (type)
            {
                case AttackType.Normal:
                    if (player.IsParring || player.IsSpiritParring) // �и�
                    {
                        enemyStats.currentWillpower -= AttackPower * ParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += ParrySpiritWaveRegeneration + (ParrySpiritWaveRegeneration * RegenerationSpiritWaveIncreasePercent);
                        player.CallWhenGuarding?.Invoke();
                        AudioManager.instance.Playsfx(AudioManager.Sfx.Parry1);

                        if (player.PlayerESMInventory.EquipedESM != null && player.PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !player.IsRadicalESMAttackPosture) // �ش����� ������ ��ȥ���� ���� �ڼ� �� �и��� ������ �ο�
                        {
                            enemyStats.currentHealth -= AttackPower * 0.2f;
                        }

                        if (groggyForce >= ParryTenacity) // ������ �ֻ��� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                        else // ������ ��, ��, ���� ��� �и�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.parryState);
                        }
                    }
                    else if (player.IsAttackingParryColliderEnabled) // �и� �ɷ��� �ִ� ����
                    {
                        enemyStats.currentWillpower -= AttackPower * ParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += ParrySpiritWaveRegeneration + (ParrySpiritWaveRegeneration * RegenerationSpiritWaveIncreasePercent);
                        player.CallWhenGuarding?.Invoke();
                        AudioManager.instance.Playsfx(AudioManager.Sfx.Parry2);

                        if (player.IsSpiritSwordDanceSecondAttack) // ��ȥ �˹��� 2��° �������� �и��� �����ϸ� ��ȥ ������ 3��° ������ �ٷ� �� �� �ְ� �ȴ�.
                        {
                            player.CanSpiritCleave3Combo = true;
                        }

                        if (groggyForce >= ParryTenacity) // ������ �ֻ��� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                    }
                    else if (IsSteadfast) // ���� �ε��� ������ ��ȥ���� ���� �ÿ� ����� �и�ó��. ����� �����
                    {
                        enemyStats.currentWillpower -= AttackPower * ParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += ParrySpiritWaveRegeneration + (ParrySpiritWaveRegeneration * RegenerationSpiritWaveIncreasePercent);
                        player.CallWhenGuarding?.Invoke();
                        AudioManager.instance.Playsfx(AudioManager.Sfx.Guard);

                        if (groggyForce >= ParryTenacity) // ������ �ֻ��� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                        else // ������ ��, ��, ���� ��� �и�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);
                        }
                    }
                    else if (player.IsGuarding) // ����
                    {
                        CurrentHealth -= damage * (1 - GuardDamageReducePercentage) * (1 - DamageReducePercentage); // ����� ��������ŭ ü�� ����
                        CurrentSpiritWave += GuardSpiritWaveRegeneration + (GuardSpiritWaveRegeneration * RegenerationSpiritWaveIncreasePercent);
                        player.CallWhenGuarding?.Invoke();
                        AudioManager.instance.Playsfx(AudioManager.Sfx.Guard);

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
                    }
                    else // �׳� �ǰ�(�����ϸ� isDirectAttack���� �ǰ��� ó�������� Ȥ�ó� ���� ��Ȳ�� ���)
                    {
                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
                        AudioManager.instance.Playsfx(AudioManager.Sfx.PlayerDamageVoice);

                        if (groggyForce > Tenacity && player.CurrentPlayerState != PlayerState.HitLongGroggy)  // ������ �� �̻��� ��� �� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // ������ ���� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                    }
                    break;
                case AttackType.Piercing:
                    if (player.CurrentPlayerState == PlayerState.Penetrate)
                    {
                        enemyStats.currentWillpower -= AttackPower * PenetrateImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += PenetrateSpiritWaveRegeneration + (PenetrateSpiritWaveRegeneration * RegenerationSpiritWaveIncreasePercent);
                        player.CallWhenGuarding?.Invoke();
                        AudioManager.instance.Playsfx(AudioManager.Sfx.Parry2);

                        if (player.PlayerESMInventory.EquipedESM != null && player.PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !player.IsRadicalESMAttackPosture) // �ش����� ������ ��ȥ���� ���� �ڼ� �� ���Ŀ� ������ �ο�
                        {
                            enemyStats.currentHealth -= AttackPower * 0.2f; // �и� �� ������ �ο�
                        }
                    }
                    else if (IsSteadfast) // ���� �ε��� ������ ��ȥ���� ���� �ÿ� ����� ���� ó��. ����� �����
                    {
                        enemyStats.currentWillpower -= AttackPower * PenetrateImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += PenetrateSpiritWaveRegeneration + (PenetrateSpiritWaveRegeneration * RegenerationSpiritWaveIncreasePercent);
                        player.CallWhenGuarding?.Invoke();
                        AudioManager.instance.Playsfx(AudioManager.Sfx.Guard);

                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);
                    }
                    else // ���� ���� �ƴѵ� �ǰݵǸ� ������ �ǰ�
                    {
                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
                        AudioManager.instance.Playsfx(AudioManager.Sfx.PlayerDamageVoice);

                        if (groggyForce > Tenacity && player.CurrentPlayerState != PlayerState.HitLongGroggy)  // ������ �� �̻��� ��� �� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // ������ ���� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                    }
                    break;
                case AttackType.Spirit:
                    if (player.IsSpiritParring)
                    {
                        enemyStats.currentWillpower -= AttackPower * SpiritParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += SpiritParrySpiritWaveRegeneration + (SpiritParrySpiritWaveRegeneration * RegenerationSpiritWaveIncreasePercent);
                        player.CallWhenGuarding?.Invoke();
                        AudioManager.instance.Playsfx(AudioManager.Sfx.SpiritParry);

                        if (player.PlayerESMInventory.EquipedESM != null && player.PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !player.IsRadicalESMAttackPosture) // �ش����� ������ ��ȥ���� ���� �ڼ� �� �и��� ������ �ο�
                        {
                            enemyStats.currentHealth -= AttackPower * 0.2f;
                        }
                    }
                    else if (IsSteadfast)  // ���� �ε��� ������ ��ȥ���� ���� �ÿ� ����� ��ȥ�и� ó��. ����� �����
                    {
                        enemyStats.currentWillpower -= AttackPower * SpiritParryImpactForcePercentage; // ������ ������ ����
                        CurrentSpiritWave += SpiritParrySpiritWaveRegeneration + (SpiritParrySpiritWaveRegeneration * RegenerationSpiritWaveIncreasePercent);
                        player.CallWhenGuarding?.Invoke();
                        AudioManager.instance.Playsfx(AudioManager.Sfx.Guard);

                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);
                    }
                    else // ��ȥ �и� ���� �ƴϸ� ������ �ǰ�
                    {
                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����
                        AudioManager.instance.Playsfx(AudioManager.Sfx.PlayerDamageVoice);

                        if (groggyForce > Tenacity && player.CurrentPlayerState != PlayerState.HitLongGroggy)  // ������ �� �̻��� ��� �� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // ������ ���� ��� ª�� �ൿ �Ҵ�
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                    }
                    break;
                case AttackType.Grab:
                    if (player.CurrentPlayerState != PlayerState.Grabbed)
                    {
                        CurrentHealth -= damage * (1 - DamageReducePercentage); // ��������ŭ ü�� ����

                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.grabbedState); // ���Ϳ��� ���� �� �� �ൿ �Ҵ� �����̻� ��ȯ�� ȣ�����ָ� ��.
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
            player.PlayerCharacterController.enabled = false;
            AudioManager.instance.Playsfx(AudioManager.Sfx.PlayerDead);

            GetComponent<PlayerInteraction>().PlayerDie();
        }
    }
}
