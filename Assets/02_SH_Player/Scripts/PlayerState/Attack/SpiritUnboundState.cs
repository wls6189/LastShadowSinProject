using UnityEngine;

public class SpiritUnboundState : IState
{
    PlayerController player;
    float frame = 30;

    public SpiritUnboundState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritUnbound;
        player.PlayerAnimator.SetTrigger("DoSpiritUnbound");
        player.IsAttacking = true;
        player.PlayerStats.CurrentSpiritWave -= 2; // 영혼의 파동 소모량

        player.FireSpiritUnboundProjectile();
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // 공격 중 상태 종료(다음 State로 이동 가능한 상태)
        if (duration >= 15f / frame)
        {
            player.IsAttacking = false;
        }

        // 별 다른 입력이 없다면 아이들 상태로 전환
        if (duration >= 27f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsAttacking = false;
    }
}