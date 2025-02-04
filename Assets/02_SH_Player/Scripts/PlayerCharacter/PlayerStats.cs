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

    // 체력 관련 부분
    [HideInInspector] public float MaxHealth;
    [HideInInspector] public float CurrentHealth;
    [HideInInspector] public float DamageReducePercentage; // 초기값은 0. 영혼낙인에 의해 설정되는 변수
    [HideInInspector] public float GuardDamageReducePercentage; // 가드 시 플레이어가 받는 데미지 감쇠배율

    // 공격력 관련 부분
    
    public float AttackPower; // 공격력
    [HideInInspector] public float AttackPowerIncreasePercentage;

    // 타격력 관련 부분
    [HideInInspector] public float ParryImpactForcePercentage;
    [HideInInspector] public float PenetrateImpactForcePercentage;
    [HideInInspector] public float SpiritParryImpactForcePercentage;

    // 영혼의 파동 관련 부분
    [HideInInspector] public float MaxSpiritWave;
    [HideInInspector] public float CurrentSpiritWave;
    [HideInInspector] public float RegenerationSpiritWavePerSec; // 초당 회복량
    [HideInInspector] public float RegenerationSpiritWaveIncreasePercent; // 초당 회복량
    [HideInInspector] public float ParrySpiritWaveRegeneration;
    [HideInInspector] public float GuardSpiritWaveRegeneration;
    [HideInInspector] public float PenetrateSpiritWaveRegeneration;
    [HideInInspector] public float SpiritParrySpiritWaveRegeneration;

    // 영혼낙인력 관련 부분
     public float MaxSpiritMarkForce;
     public float CurrentSpiritMarkForce;
    [HideInInspector] public float SpiritMarkForceGainIncreasePercentage; // 영혼낙인력 획득 증가 배율

    // 강인함 관련 부분
    [HideInInspector] public TenacityAndGroggyForce Tenacity;
    [HideInInspector] public TenacityAndGroggyForce GuardTenacity;
    [HideInInspector] public TenacityAndGroggyForce ParryTenacity;

    // 그 외 부분
    [HideInInspector] public bool IsImmune; // 공격에 면역 상태인지 여부
    [HideInInspector] public bool IsSteadfast; // 막기도 패리인정이 되는지 여부
    [HideInInspector] public bool IsRavenous; // 굶주린 영원의 영혼낙인 착용 여부. 흡혈
    [HideInInspector] public bool IsEnthusiastic; // 열성적인 영원의 영혼낙인 착용 여부. 영혼의 파동 회복
    [HideInInspector] public bool IsRagingOn; // 몰아치는 영원의 영혼낙인 착용 여부. 낙뢰
    [HideInInspector] public int RagingStack; // 몰아치는 영원의 영혼낙인 공격 스택
    [HideInInspector] public float SpiritAsh;
    
    void Awake()
    {
        TryGetComponent(out player);
    }
    void Start()
    {
        //InitializeStats(); // 각 스탯의 초기화는 영혼낙인 인벤토리에서 구현
        LoadStatDataWhenQuit(); // 현재 체력, 영혼의 파동, 영혼낙인력 등 저장된 수치를 불러오기
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
        //SpiritAsh = 추후 세이브에서 가져오기
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
        if (IsImmune) return; // 면역 상태라면 모든 공격 무시
        if (player.OnDashEffect) return; // 대쉬 중에는 무시

        if (isDirectAttack) // 가드를 피해서 직접 플레이어를 때렸을 때(몬스터의 콜라이더에 Player 태그가 바로 닿을 경우)
        {
            if (groggyForce > Tenacity && !player.IsGrogging) // 위력이 상 이상인 경우 긴 행동 불능
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
            }
            else if (groggyForce == Tenacity && !player.IsGrogging) // 위력이 중인 경우 짧은 행동 불능
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
            }

            CurrentHealth -= damage * (1 - DamageReducePercentage); // 데미지만큼 체력 감소
        }
        else 
        {
            switch (type)
            {
                case AttackType.Normal:
                    if (player.IsParring || player.IsSpiritParring) // 패리
                    {
                        if (groggyForce >= ParryTenacity) // 위력이 최상인 경우 짧은 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                        else // 위력이 상, 중, 하인 경우 패리
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.parryState);
                        }

                        //enemyStats.CurrentWillpower -= AttackPower * ParryImpactForcePercentage; // 몬스터의 의지력 감소
                        CurrentSpiritWave += ParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();

                        if (player.PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !player.IsRadicalESMAttackPosture) // 극단적인 영원의 영혼낙인 수비 자세 시 패리에 데미지 부여
                        {
                            //enemyStats.CurrentHealth -= AttackPower * 0.2f; // 몬스터의 의지력 감소
                        }
                    }
                    else if (player.IsAttackingParryColliderEnabled) // 패리 능력이 있는 공격
                    {
                        if (player.IsSpiritSwordDanceSecondAttack) // 영혼 검무의 2번째 공격으로 패리에 성공하면 영혼 가르기 3번째 공격을 바로 할 수 있게 된다.
                        {
                            player.CanSpiritCleave3Combo = true;
                        }

                        if (groggyForce >= ParryTenacity) // 위력이 최상인 경우 짧은 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        //enemyStats.CurrentWillpower -= AttackPower * ParryImpactForcePercentage; // 몬스터의 의지력 감소
                        CurrentSpiritWave += ParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
                    }
                    else if (IsSteadfast) // 만약 부동의 영원의 영혼낙인 착용 시엔 가드라도 패리처리. 모션은 가드로
                    {
                        if (groggyForce >= ParryTenacity) // 위력이 최상인 경우 짧은 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                        else // 위력이 상, 중, 하인 경우 패리
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);
                        }

                        //enemyStats.CurrentWillpower -= AttackPower * ParryImpactForcePercentage; // 몬스터의 의지력 감소
                        CurrentSpiritWave += ParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
                    }
                    else if (player.IsGuarding) // 막기
                    {
                        if (groggyForce > GuardTenacity) // 위력이 최상인 경우 긴 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == GuardTenacity) // 위력이 상인 경우 짧은 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }
                        else if (groggyForce < GuardTenacity) // 위력이 중, 하인 경우 가드 힛
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);
                        }

                        CurrentHealth -= damage * (1 - GuardDamageReducePercentage) * (1 - DamageReducePercentage); // 감쇠된 데미지만큼 체력 감소
                        CurrentSpiritWave += GuardSpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
                    }
                    else // 그냥 피격(웬만하면 isDirectAttack에서 피격이 처리되지만 혹시나 생길 상황에 대비)
                    {
                        if (groggyForce > Tenacity && !player.IsGrogging) // 위력이 상 이상인 경우 긴 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // 위력이 중인 경우 짧은 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // 데미지만큼 체력 감소
                    }
                    
                    break;
                case AttackType.Piercing:
                    if (player.CurrentPlayerState == PlayerState.Penetrate)
                    {
                        //enemyStats.CurrentWillpower -= AttackPower * PenetrateImpactForcePercentage; // 몬스터의 의지력 감소
                        CurrentSpiritWave += PenetrateSpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();

                        if (player.PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !player.IsRadicalESMAttackPosture) // 극단적인 영원의 영혼낙인 수비 자세 시 패리에 데미지 부여
                        {
                            //enemyStats.CurrentHealth -= AttackPower * 0.2f; // 몬스터의 의지력 감소
                        }
                    }
                    else if (IsSteadfast) // 만약 부동의 영원의 영혼낙인 착용 시엔 가드라도 간파 처리. 모션은 가드로
                    {
                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);

                        //enemyStats.CurrentWillpower -= AttackPower * PenetrateImpactForcePercentage; // 몬스터의 의지력 감소
                        CurrentSpiritWave += PenetrateSpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
                    }
                    else // 간파 중이 아닌데 피격되면 무조건 피격
                    {
                        if (groggyForce > Tenacity && !player.IsGrogging) // 위력이 상 이상인 경우 긴 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // 위력이 중인 경우 짧은 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // 데미지만큼 체력 감소
                        
                    }
                    
                    break;
                case AttackType.Spirit:
                    if (player.IsSpiritParring)
                    {
                        //enemyStats.CurrentWillpower -= AttackPower * SpiritParryImpactForcePercentage; // 몬스터의 의지력 감소
                        CurrentSpiritWave += SpiritParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();

                        if (player.PlayerESMInventory.EquipedESM.Equals(new RadicalESM()) && !player.IsRadicalESMAttackPosture) // 극단적인 영원의 영혼낙인 수비 자세 시 패리에 데미지 부여
                        {
                            //enemyStats.CurrentHealth -= AttackPower * 0.2f; // 몬스터의 의지력 감소
                        }
                    }
                    else if (IsSteadfast)  // 만약 부동의 영원의 영혼낙인 착용 시엔 가드라도 영혼패리 처리. 모션은 가드로
                    {
                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.guardHitState);

                        //enemyStats.CurrentWillpower -= AttackPower * SpiritParryImpactForcePercentage; // 몬스터의 의지력 감소
                        CurrentSpiritWave += SpiritParrySpiritWaveRegeneration;
                        player.CallWhenGuarding?.Invoke();
                    }
                    else // 영혼 패리 중이 아니면 무조건 피격
                    {
                        if (groggyForce > Tenacity && !player.IsGrogging) // 위력이 상 이상인 경우 긴 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // 위력이 중인 경우 짧은 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // 데미지만큼 체력 감소
                    }
                    break;
                case AttackType.Grab:
                    if (player.CurrentPlayerState != PlayerState.Grabbed)
                    {
                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.grabbedState); // 몬스터에서 끝날 때 긴 행동 불능 상태이상 전환을 호출해주면 됨.

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // 데미지만큼 체력 감소
                    }
                    else // 그랩 중일 때도 데미지 입히기 가능
                    {
                        CurrentHealth -= damage * (1 - DamageReducePercentage); // 데미지만큼 체력 감소
                    }
                    break;
                case AttackType.Clash:
                    if (player.IsClashGuard)
                    {
                        player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.clashGuardState); // 몬스터와 어떻게 처리할지 추후 논의
                    }
                    else
                    {
                        if (groggyForce > Tenacity && !player.IsGrogging) // 위력이 상 이상인 경우 긴 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitLongGroggyState);
                        }
                        else if (groggyForce == Tenacity && !player.IsGrogging) // 위력이 중인 경우 짧은 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentHealth -= damage * (1 - DamageReducePercentage); // 데미지만큼 체력 감소
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
