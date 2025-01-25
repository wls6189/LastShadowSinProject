using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    PlayerController player;

    // 체력 관련 부분
    public float MaxHealth;
    public float CurrentHealth; // 체력
    public float DamageReducePercent; // 플레이어가 받는 데미지 감쇠배율

    // 의지력 관련 부분
    public float MaxWillpower; 
    public float CurrentWillpower; // 의지력
    public float WillpowerRecoveryPerSec; // 초당 의지력 회복량
    public float WillpowerRecoveryParry; // 패리 시 의지력 회복량
    public float WillpowerRecoveryPenetrate; // 간파 시 의지력 회복량
    public float WillpowerRecoverySpiritParry; // 영혼 패리 시 의지력 회복량
    public float PlayerWillpowerRecoveryIncreasePercent; // 플레이어의 의지력 회복 증가배율

    // 공격력 관련 부분
    public float AttackPower; // 공격력

    // 영혼의 파동 관련 부분
    public float MaxSpiritWave;
    public float CurrentSpiritWave; // 영혼의 파동

    // 강인함 관련 부분
    public int Tenacity; // 1, 2, 3, 4로 결정.
    public int GuardTenacity;
    public int ParryTenacity;

    // 타격력 관련 부분
    public float ImpactForceReducePercent; // 플레이어가 받는 타격력 감쇠배율
    public float PlayerImpactForceIncreasePercent; // 플레이어가 가하는 타격력 증가배율
    
    void Awake()
    {
        TryGetComponent(out player);

        MaxHealth = 100; // 아이템에 의해 변경 가능
        CurrentHealth = MaxHealth;
        DamageReducePercent = 0.9f;

        MaxWillpower = 150; // 아이템에 의해 변경 가능
        CurrentWillpower = MaxWillpower;
        WillpowerRecoveryPerSec = 10f;
        WillpowerRecoveryParry = 3f;
        PlayerWillpowerRecoveryIncreasePercent = 0f;

        MaxSpiritWave = 12;
        CurrentSpiritWave = MaxSpiritWave; // 추후 시작 시 영혼의 파동은 0으로 되게끔 수정 요

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

            CurrentHealth -= damage; // 데미지만큼 체력 감소
            CurrentWillpower -= impactForce; // 타격력만큼 의지력 감소
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

                        CurrentWillpower += (WillpowerRecoveryParry + WillpowerRecoveryParry * PlayerWillpowerRecoveryIncreasePercent); // 캐릭터의 의지력 회복
                        //enemyStats.CurrentWillpower -= 15; // 몬스터의 의지력 감소
                    }
                    else if (player.IsAttackingParring) // 패리 능력이 있는 공격
                    {
                        if (groggyForce >= ParryTenacity) // 위력이 최상인 경우 짧은 행동 불능
                        {
                            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.hitShortGroggyState);
                        }

                        CurrentWillpower += (WillpowerRecoveryParry + WillpowerRecoveryParry * PlayerWillpowerRecoveryIncreasePercent); // 캐릭터의 의지력 회복
                        //enemyStats.CurrentWillpower -= 15; // 몬스터의 의지력 감소
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

                        CurrentHealth -= (damage - damage * DamageReducePercent); // 감쇠된 데미지만큼 체력 감소
                        CurrentWillpower -= (impactForce - impactForce * ImpactForceReducePercent); // 감쇠된 타격력만큼 의지력 감소
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

                        CurrentHealth -= damage; // 데미지만큼 체력 감소
                        CurrentWillpower -= impactForce; // 타격력만큼 의지력 감소
                    }
                    break;
                case AttackType.Piercing:
                    if (player.CurrentPlayerState == PlayerState.Penetrate)
                    {
                        CurrentWillpower += (WillpowerRecoveryPenetrate + WillpowerRecoveryPenetrate * PlayerWillpowerRecoveryIncreasePercent); // 캐릭터의 의지력 회복
                        //enemyStats.CurrentWillpower -= 25; // 몬스터의 의지력 감소

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

                        CurrentHealth -= damage; // 데미지만큼 체력 감소
                        CurrentWillpower -= impactForce; // 타격력만큼 의지력 감소
                    }
                    break;
                case AttackType.Spirit:
                    if (player.IsSpiritParring)
                    {
                        CurrentWillpower += (WillpowerRecoverySpiritParry + WillpowerRecoverySpiritParry * PlayerWillpowerRecoveryIncreasePercent); // 캐릭터의 의지력 회복
                        //enemyStats.CurrentWillpower -= 70; // 몬스터의 의지력 감소
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

                        CurrentHealth -= damage; // 데미지만큼 체력 감소
                        CurrentWillpower -= impactForce; // 타격력만큼 의지력 감소
                    }
                    break;
                case AttackType.Grab:
                    player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.grabbedState); // 몬스터에서 끝날 때 긴 행동 불능 상태이상 전환을 호출해주면 됨.
                    break;
                case AttackType.Clash:
                    if (player.IsClashGuard)
                    {

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

                        CurrentHealth -= damage; // 데미지만큼 체력 감소
                        CurrentWillpower -= impactForce; // 타격력만큼 의지력 감소
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
