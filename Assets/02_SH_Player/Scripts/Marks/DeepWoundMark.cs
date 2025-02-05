using UnityEngine;

public class DeepWoundMark : Mark
{
    public DeepWoundMark() : base(
        "���� ��ó",
        "��ȥ���η� ȸ������ 10% / 30% �����մϴ�."
        )
    { }

    public override void Effect(PlayerController player, int count) // �� �� ���� ȣ�� ��
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
