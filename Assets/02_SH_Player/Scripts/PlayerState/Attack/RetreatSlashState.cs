using UnityEngine;

public class RetreatSlashState : IState
{
    PlayerController player;
    float frame = 35;

    public RetreatSlashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.RetreatSlash;
        player.PlayerAnimator.SetTrigger("DoRetreatSlash");
        player.IsAttacking = true;
        player.PlayerStats.CurrentSpiritWave -= 1; // 영혼의 파동 소모량
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // 공격 시 이동 여부
        if (duration >= 9f / frame && duration <= 14f / frame)
        {
            player.AttackMoving(-16f);
        }

        // 공격 콜라이더 활성화 여부
        if (duration >= 6f / frame && duration <= 11f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // 공격 중 상태 종료(다음 State로 이동 가능한 상태)
        if (duration >= 20f / frame)
        {
            player.IsAttacking = false;
        }

        // 별 다른 입력이 없다면 아이들 상태로 전환
        if (duration >= 32f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsAttacking = false;
        player.IsAttackColliderEnabled = false;
    }
}