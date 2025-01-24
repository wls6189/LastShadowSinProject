using UnityEngine;

public class BasicHorizonSlash1State : IState
{
    PlayerController player;
    float frame = 35;

    public BasicHorizonSlash1State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.BasicHorizonSlash1;
        player.PlayerAnimator.SetTrigger("DoBasicHorizonSlash1");
        player.IsAttacking = true;
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // 공격 시 전진 여부
        if (duration >= 6f / frame && duration <= 10f / frame) // 첫 발 디딤
        {
            player.AttackMoving(5f); // 발 디딜 때 플레이어가 움직이는 속도를 매개변수로 입력하면 매개변수의 속도로 움직임
        }
        else if (duration >= 18f / frame && duration <= 32f / frame) // 돌아오는 발 디딤
        {
            player.AttackMoving(2f);
        }

        // 무기 콜라이더 활성화 여부
        if (duration >= 7f / frame && duration <= 10f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // 공격 중 상태 종료(다음 State로 이동 가능한 상태) + (콤보 공격 기능 켜짐)
        if (duration >= 20f / frame)
        {
            player.IsAttacking = false;
            player.CanBasicHorizonSlash2Combo = true;
        }

        // 별 다른 입력이 없다면 아이들 상태로 전환
        if (duration >= 32f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        // 만일을 대비한 변수 초기화
        player.IsAttacking = false;
        player.IsAttackColliderEnabled = false;
    }
}