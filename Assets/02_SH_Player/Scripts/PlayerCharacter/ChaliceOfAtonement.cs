using UnityEngine;

public class ChaliceOfAtonement : MonoBehaviour
{
    [HideInInspector] public int MaxChaliceOfAtonementCount;
    [HideInInspector] public int CurrentChaliceOfAtonementCount;

    PlayerController player;
    float recoveryAmount;

    void Awake()
    {
        TryGetComponent(out player);
        MaxChaliceOfAtonementCount = 2; // 추후 세이브에서 불러오는 걸로
        CurrentChaliceOfAtonementCount = MaxChaliceOfAtonementCount;
        recoveryAmount = 30; // 추후 세이브에서 불러오기
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
