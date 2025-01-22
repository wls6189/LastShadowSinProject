using UnityEngine;

public class BasicHorizonSlash2State : IState
{
    PlayerController player;
    float frame = 37;

    public BasicHorizonSlash2State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicHorizonSlash2;
        player.Animator.SetTrigger("DoBasicHorizonSlash2");
        player.IsAttacking = true;
        player.CanBasicHorizonSlashCombo = false;
    }

    public void Execute()
    {
        if (player.Animator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // 공격 시 전진 여부
        if (duration >= 6f / frame && duration <= 11f / frame)
        {
            player.AttackMoving(4f); 
        }

        // 무기 콜라이더 활성화 여부
        if (duration >= 7f / frame && duration <= 12f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // 공격 중 상태 종료(다음 State로 이동 가능한 상태)
        if (duration >= 22f / frame)
        {
            player.IsAttacking = false;
        }

        // 별 다른 입력이 없다면 아이들 상태로 전환
        if (duration >= 34f / frame)
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