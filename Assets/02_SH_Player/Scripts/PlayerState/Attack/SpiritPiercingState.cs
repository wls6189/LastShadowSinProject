using UnityEngine;

public class SpiritPiercingState : IState
{
    PlayerController player;
    float frame = 40;

    public SpiritPiercingState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.CurrentPlayerState = PlayerState.SpiritPiercing;
        player.PlayerAnimator.SetTrigger("DoSpiritPiercing");
        player.IsAttacking = true;
        player.PlayerStats.CurrentSpiritWave -= 2; // ��ȥ�� �ĵ� �Ҹ�
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� �̵� ����
        if (duration >= 9f / frame && duration <= 15f / frame)
        {
            player.AttackMoving(8f);
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 10f / frame && duration <= 14f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����(���� State�� �̵� ������ ����)
        if (duration >= 24f / frame)
        {
            player.IsAttacking = false;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
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