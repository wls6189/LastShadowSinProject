using UnityEngine;

public class BasicVerticalSlashState : IState
{
    PlayerController player;
    float frame = 40;

    public BasicVerticalSlashState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicVerticalSlash;
        player.Animator.SetTrigger("DoBasicVerticalSlash");
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
        if (duration >= 8f / frame && duration <= 13f / frame) // 첫 발 디딤
        {
            player.AttackMoving(3f); // 발 디딜 때 플레이어가 움직이는 속도를 매개변수로 입력.
        }

        // 무기 콜라이더 활성화 여부
        if (duration >= 9f / frame && duration <= 13f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // 공격 중 상태 종료
        if (duration >= 26f / frame)
        {
            player.IsAttacking = false;
        }

        // 별 다른 입력이 없다면 아이들 상태로 전환
        if (duration >= 37f / frame)
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