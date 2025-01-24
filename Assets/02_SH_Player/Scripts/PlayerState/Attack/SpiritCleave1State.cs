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
        player.PlayerStats.CurrentSpiritWave -= 1; // ��ȥ�� �ĵ� �Ҹ�
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� ���� ����
        if (duration >= 8f / frame && duration <= 13f / frame) // ù �� ���
        {
            player.AttackMoving(5f); // �� ��� �� �÷��̾ �����̴� �ӵ��� �Ű������� �Է��ϸ� �Ű������� �ӵ��� ������
        }
        else if (duration >= 24f / frame && duration <= 32f / frame) // ���ƿ��� �� ���
        {
            player.AttackMoving(2f);
        }

        // ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 10f / frame && duration <= 13f / frame)
        {
            player.IsAttackColliderEnabled = true;
        }
        else
        {
            player.IsAttackColliderEnabled = false;
        }

        // ���� �� ���� ����
        if (duration >= 22f / frame)
        {
            player.IsAttacking = false;
            player.CanSpiritCleave2Combo = true;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
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