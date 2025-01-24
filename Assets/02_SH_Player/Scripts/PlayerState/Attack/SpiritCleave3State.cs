using UnityEngine;

public class SpiritCleave3State : IState
{
    PlayerController player;
    float frame = 50;

    public SpiritCleave3State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritCleave3;
        player.PlayerAnimator.SetTrigger("DoSpiritCleave3");
        player.IsAttacking = true;
        player.CanSpiritCleave3Combo = false;
        player.PlayerStats.CurrentSpiritWave -= 2; // 영혼의 파동 소모량
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // 공격 시 전진 여부
        if (duration >= 2f / frame && duration <= 12f / frame) // 첫 발 디딤
        {
            player.AttackMoving(5f); // 발 디딜 때 플레이어가 움직이는 속도를 매개변수로 입력하면 매개변수의 속도로 움직임
        }

        // 무기 콜라이더 활성화 여부
        if (duration >= 11f / frame && duration <= 14f / frame)
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
        if (duration >= 47f / frame)
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