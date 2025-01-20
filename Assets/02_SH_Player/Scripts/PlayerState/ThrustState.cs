using UnityEngine;

public class ThrustState : IState
{
    PlayerController player;
    float frame = 35;

    public ThrustState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.Thrust;
        player.Animator.SetTrigger("DoThrust");
        player.IsAttacking = true;
    }

    public void Execute()
    {
        if (player.Animator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // 공격 시 전진 여부
        if (duration >= 1f / frame && duration <= 5f / frame)
        {
            player.AttackMoving(16f);
        }

        // 패리 판정 및 공격 콜라이더 활성화 여부
        if (duration >= 1f / frame && duration <= 5f / frame)
        {
            player.IsParring = true;
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsParring = false;
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
        player.IsParring = false;
        player.IsAttackColliderEnabled = false;
    }
}