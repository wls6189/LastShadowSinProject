using UnityEngine;

public class SpiritCleave1State : IState
{
    PlayerController player;
    float frame = 45;

    public SpiritCleave1State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritCleave1;
        player.PlayerAnimator.SetTrigger("DoSpiritCleave1");
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

        // 공격 시 전진 여부
        if (duration >= 8f / frame && duration <= 13f / frame) // 첫 발 디딤
        {
            player.AttackMoving(5f); // 발 디딜 때 플레이어가 움직이는 속도를 매개변수로 입력하면 매개변수의 속도로 움직임
        }
        else if (duration >= 24f / frame && duration <= 32f / frame) // 돌아오는 발 디딤
        {
            player.AttackMoving(2f);
        }

        // 무기 콜라이더 활성화 여부
        if (duration >= 10f / frame && duration <= 13f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // 공격 중 상태 종료
        if (duration >= 22f / frame)
        {
            player.IsAttacking = false;
            player.CanSpiritCleave2Combo = true;
        }

        // 별 다른 입력이 없다면 아이들 상태로 전환
        if (duration >= 41f / frame)
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