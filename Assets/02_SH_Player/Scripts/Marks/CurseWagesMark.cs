using UnityEngine;

public class CurseWagesMark : Mark
{
    public CurseWagesMark() : base(
        "저주의 대가",
        "회복약의 회복량이 70% 감소하고 공격력이 10 / 30 증가합니다."
        )
    { }

    public override void Effect(PlayerController player, int count) // 딱 한 번만 호출 요
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
