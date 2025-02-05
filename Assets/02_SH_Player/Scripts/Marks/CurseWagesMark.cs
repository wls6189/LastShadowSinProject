using UnityEngine;

public class CurseWagesMark : Mark
{
    public CurseWagesMark() : base(
        "������ �밡",
        "ȸ������ ȸ������ 70% �����ϰ� ���ݷ��� 10 / 30 �����մϴ�."
        )
    { }

    public override void Effect(PlayerController player, int count) // �� �� ���� ȣ�� ��
    {
        if (count >= 4)
        {
            player.PlayerStats.AttackPowerIncreaseAmount += 30;
            player.PlayerChaliceOfAtonement.RecoveryAmountReducePercentage += 0.7f;
        }
        else if (count >= 2)
        {
            player.PlayerStats.AttackPowerIncreaseAmount += 10;
            player.PlayerChaliceOfAtonement.RecoveryAmountReducePercentage += 0.7f;
        }
    }
}
