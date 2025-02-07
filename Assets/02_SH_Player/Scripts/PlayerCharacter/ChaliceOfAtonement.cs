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
    public void LoadCOAData() // 영혼샘 상호작용 시 호출
    {
        MaxChaliceOfAtonementCount = 2 + DataManager.Instance.nowPlayer.SpiritSpringCount; // 세이브에서 영혼샘 상호작용 횟수 불러오기
        recoveryAmount = 30 + (spiritSpringPercent * DataManager.Instance.nowPlayer.SpiritSpringCount); // 세이브에서 영혼샘 상호작용 횟수 불러오기
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
