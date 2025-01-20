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
        player.Animator.SetTrigger("DoBasicHorizonSlash1");
        player.IsAttacking = true;
    }

    public void Execute()
    {
        if (!player.StateInfo.IsName("BasicHorizonSlash1"))
        {
            player.Animator.Play("BasicHorizonSlash1");
            return;
        }

        // 공격 시 전진 여부
        if (player.StateInfo.normalizedTime >= 6f / frame && player.StateInfo.normalizedTime <= 10f / frame) // 첫 발 디딤
        {
            player.AttackMoving(3f); // 발 디딜 때 플레이어가 움직이는 속도를 매개변수로 입력.
        }
        else if (player.StateInfo.normalizedTime >= 18f / frame && player.StateInfo.normalizedTime <= 32f / frame) // 돌아오는 발 디딤
        {
            player.AttackMoving(2f);
        }

        // 무기 콜라이더 활성화 여부
        if (player.StateInfo.normalizedTime >= 7f / frame && player.StateInfo.normalizedTime <= 10f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // 공격 중 상태 종료(다음 State로 이동 가능한 상태) + (콤보 공격 기능 켜짐)
        if (player.StateInfo.normalizedTime >= 20f / frame)
        {
            player.IsAttacking = false;
            player.CanBasicHorizonSlashCombo = true;
        }

        // 별 다른 입력이 없다면 아이들 상태로 전환
        if (player.StateInfo.normalizedTime >= 32f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
            player.CanBasicHorizonSlashCombo = false;
        }
    }

    public void Exit()
    {
        // 만일을 대비한 변수 초기화
        player.IsAttacking = false;
        player.CanBasicHorizonSlashCombo = false;
        player.IsAttackColliderEnabled = false;
    }
}