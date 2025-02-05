using UnityEngine;

public class FocusMark : Mark
{
    public FocusMark() : base(
        "����",
        "��ȥ�� �ĵ� ȸ������ 10% / 30% �����մϴ�."
        )
    { }

    public override void Effect(PlayerController player, int count) // �� �� ���� ȣ�� ��
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
