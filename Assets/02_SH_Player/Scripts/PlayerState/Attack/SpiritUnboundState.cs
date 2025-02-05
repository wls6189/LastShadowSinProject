using UnityEngine;

public class SpiritUnboundState : IState
{
    PlayerController player;
    float frame = 30;

    public SpiritUnboundState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritUnbound;
        player.PlayerAnimator.SetTrigger("DoSpiritUnbound");
        player.IsAttacking = true;
        player.PlayerStats.CurrentSpiritWave -= 2; // ��ȥ�� �ĵ� �Ҹ�

        player.FireSpiritUnboundProjectile();
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� ���� ����(���� State�� �̵� ������ ����)
        if (duration >= 15f / frame)
        {
            player.IsAttacking = false;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
        if (duration >= 27f / frame)
        {
            player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleAndMoveState);
        }
    }

    public void Exit()
    {
        player.IsAttacking = false;
    }
}