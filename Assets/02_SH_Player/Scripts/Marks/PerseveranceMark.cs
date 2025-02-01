using UnityEngine;

public class PerseveranceMark : Mark
{
    public PerseveranceMark() : base(
        "근성",
        "공격력이 5 / 15 증가합니다."
        ){}

    public override void Effect(PlayerController player, int count) // Awake에서 호출 요
    {
        if (count >= 4)
        {
            player.PlayerStats.AttackPower += 15;
        }
        else if (count >= 2)
        {
            player.PlayerStats.AttackPower += 5;
        }
    }
}
