using UnityEngine;

public class SpiritCleave2State : IState
{
    PlayerController player;
    float frame = 45;

    public SpiritCleave2State(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritCleave2;
        player.PlayerAnimator.SetTrigger("DoSpiritCleave2");
        player.IsAttacking = true;
        player.CanSpiritCleave2Combo = false;
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
        if (duration >= 9f / frame && duration <= 13f / frame) // 첫 발 디딤
        {
            player.AttackMoving(5f); // 발 디딜 때 플레이어가 움직이는 속도를 매개변수로 입력하면 매개변수의 속도로 움직임
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
            player.CanSpiritCleave3Combo = true;
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