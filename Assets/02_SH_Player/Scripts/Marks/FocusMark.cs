using UnityEngine;

public class FocusMark : Mark
{
    public FocusMark() : base(
        "집중",
        "영혼의 파동 회복량이 10% / 30% 증가합니다."
        )
    { }

    public override void Effect(PlayerController player, int count) // 딱 한 번만 호출 요
    {
        if (count >= 4)
        {
            player.PlayerStats.RegenerationSpiritWaveIncreasePercent += 0.3f;
        }
        else if (count >= 2)
        {
            player.PlayerStats.RegenerationSpiritWaveIncreasePercent += 0.1f;
        }
    }
}
