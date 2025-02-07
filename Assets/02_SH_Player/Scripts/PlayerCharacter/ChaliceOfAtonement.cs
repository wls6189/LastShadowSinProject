using UnityEngine;

public class ChaliceOfAtonement : MonoBehaviour
{
    [HideInInspector] public int MaxChaliceOfAtonementCount;
    [HideInInspector] public int CurrentChaliceOfAtonementCount;
    [HideInInspector] public float RecoveryAmountReducePercentage;

    PlayerController player;
    float recoveryAmount;
    float spiritSpringPercent = 5;

    void Awake()
    {
        TryGetComponent(out player);
        LoadCOAData();
        CurrentChaliceOfAtonementCount = DataManager.Instance.nowPlayer.CurrentChaliceCount;
    }
    public void LoadCOAData() // ��ȥ�� ��ȣ�ۿ� �� ȣ��
    {
        MaxChaliceOfAtonementCount = 2 + DataManager.Instance.nowPlayer.SpiritSpringCount; // ���̺꿡�� ��ȥ�� ��ȣ�ۿ� Ƚ�� �ҷ�����
        recoveryAmount = 30 + (spiritSpringPercent * DataManager.Instance.nowPlayer.SpiritSpringCount); // ���̺꿡�� ��ȥ�� ��ȣ�ۿ� Ƚ�� �ҷ�����
        recoveryAmount = recoveryAmount - (recoveryAmount * RecoveryAmountReducePercentage);
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
