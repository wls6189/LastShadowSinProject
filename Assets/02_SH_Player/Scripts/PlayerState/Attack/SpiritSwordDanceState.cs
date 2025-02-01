using UnityEngine;

public class SpiritSwordDanceState : IState
{
    PlayerController player;
    float frame = 60;

    public SpiritSwordDanceState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritSwordDance;
        player.PlayerAnimator.SetTrigger("DoSpiritSwordDance");
        player.IsAttacking = true;
        player.PlayerStats.CurrentSpiritWave -= 3; // 영혼의 파동 소모량
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // 공격 시 전진 여부
        if (duration >= 7f / frame && duration <= 12f / frame)
        {
            player.AttackMoving(12f);
        }
        if (duration >= 26f / frame && duration <= 31f / frame)
        {
            player.AttackMoving(12f);
        }

        // 패리 판정 및 공격 콜라이더 활성화 여부
        if (duration >= 7f / frame && duration <= 10f / frame)
        {
            player.IsAttackingParryColliderEnabled = true;
        }
        else if (duration >= 26f / frame && duration <= 29f / frame)
        {
            player.IsAttackingParryColliderEnabled = true;
            player.IsSpiritSwordDanceSecondAttack = true; // 영혼 검무의 경우 2번째 공격에 특수 능력이 있어서 별도의 변수를 선언
        }
        else
        {
            player.IsAttackingParryColliderEnabled = false;
            player.IsSpiritSwordDanceSecondAttack = false;
        }

        // 공격 중 상태 종료(다음 State로 이동 가능한 상태)
        if (duration >= 39f / frame)
        {
            player.IsAttacking = false;
        }

        // 별 다른 입력이 없다면 아이들 상태로 전환
        if (duration >= 56f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsAttacking = false;
        player.IsAttackingParryColliderEnabled = false;
        player.IsSpiritSwordDanceSecondAttack = false;
    }
}