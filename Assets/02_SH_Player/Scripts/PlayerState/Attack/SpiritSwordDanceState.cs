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
        player.PlayerStats.CurrentSpiritWave -= 3; // ��ȥ�� �ĵ� �Ҹ�
    }

    public void Execute()
    {
        if (player.PlayerAnimator.IsInTransition(0))
        {
            return;
        }

        float duration = player.StateInfo.normalizedTime % 1f;

        // ���� �� ���� ����
        if (duration >= 7f / frame && duration <= 12f / frame)
        {
            player.AttackMoving(12f);
        }
        if (duration >= 26f / frame && duration <= 31f / frame)
        {
            player.AttackMoving(12f);
        }

        // �и� ���� �� ���� �ݶ��̴� Ȱ��ȭ ����
        if (duration >= 7f / frame && duration <= 10f / frame)
        {
            player.IsAttackingParryColliderEnabled = true;
        }
        else if (duration >= 26f / frame && duration <= 29f / frame)
        {
            player.IsAttackingParryColliderEnabled = true;
            player.IsSpiritSwordDanceSecondAttack = true; // ��ȥ �˹��� ��� 2��° ���ݿ� Ư�� �ɷ��� �־ ������ ������ ����
        }
        else
        {
            player.IsAttackingParryColliderEnabled = false;
            player.IsSpiritSwordDanceSecondAttack = false;
        }

        // ���� �� ���� ����(���� State�� �̵� ������ ����)
        if (duration >= 39f / frame)
        {
            player.IsAttacking = false;
        }

        // �� �ٸ� �Է��� ���ٸ� ���̵� ���·� ��ȯ
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