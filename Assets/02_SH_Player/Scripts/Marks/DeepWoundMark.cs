using UnityEngine;

public class DeepWoundMark : Mark
{
    public DeepWoundMark() : base(
        "깊은 상처",
        "영혼낙인력 회복량이 10% / 30% 증가합니다."
        )
    { }

    public override void Effect(PlayerController player, int count) // 딱 한 번만 호출 요
    {
        if (count >= 4)
        {
            player.PlayerStats.SpiritMarkForceGainIncreasePercentage += 0.3f;
        }
        else if (count >= 2)
        {
            player.PlayerStats.SpiritMarkForceGainIncreasePercentage += 0.1f;
        }
    }
}
