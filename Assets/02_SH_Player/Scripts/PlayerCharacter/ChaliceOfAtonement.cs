using UnityEngine;

public class ChaliceOfAtonement : MonoBehaviour
{
    [HideInInspector] public int MaxChaliceOfAtonementCount;
    [HideInInspector] public int CurrentChaliceOfAtonementCount;

    PlayerController player;
    float recoveryAmount;
    float spiritSpringPercent = 5;

    void Awake()
    {
        TryGetComponent(out player);
        MaxChaliceOfAtonementCount = 2 + DataManager.Instance.nowPlayer.SpiritSpringCount; // ���̺꿡�� ��ȥ�� ��ȣ�ۿ� Ƚ�� �ҷ�����
        CurrentChaliceOfAtonementCount = MaxChaliceOfAtonementCount;
        recoveryAmount = 30 + (spiritSpringPercent * DataManager.Instance.nowPlayer.SpiritSpringCount); // ���̺꿡�� ��ȥ�� ��ȣ�ۿ� Ƚ�� �ҷ�����
    }


    public void UseChaliceOfAtonement()
    {
        if (CurrentChaliceOfAtonementCount > 0)
        {
            CurrentChaliceOfAtonementCount--;
            player.PlayerStats.CurrentHealth += recoveryAmount;
        }

        if (CurrentChaliceOfAtonementCount < 0)
        {
            CurrentChaliceOfAtonementCount = 0;
        }
    }
}
