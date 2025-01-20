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
        if (!player.StateInfo.IsName("BasicHorizonSlash2"))
        {
            player.Animator.Play("BasicHorizonSlash2");
            return;
        }

        if (player.StateInfo.normalizedTime >= 0f && player.StateInfo.normalizedTime <= 1f / frame) // 애니메이션 시작
        {
            player.IsAttacking = true;
            player.CanBasicHorizonSlashCombo = false;
        }

        // 공격 시 전진 여부
        if (player.StateInfo.normalizedTime >= 6f / frame && player.StateInfo.normalizedTime <= 11f / frame) // 첫 발 디딤
        {
            player.AttackMoving(4f); // 발 디딜 때 플레이어가 움직이는 속도를 매개변수로 입력.
        }

        // 무기 콜라이더 활성화 여부
        if (player.StateInfo.normalizedTime >= 7f / frame && player.StateInfo.normalizedTime <= 12f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // 공격 중 상태 종료(다음 State로 이동 가능한 상태) + (콤보 공격 기능 켜짐)
        if (player.StateInfo.normalizedTime >= 22f / frame)
        {
            player.IsAttacking = false;
        }

        // 별 다른 입력이 없다면 아이들 상태로 전환
        if (player.StateInfo.normalizedTime >= 34f / frame)
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