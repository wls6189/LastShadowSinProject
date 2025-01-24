using UnityEngine;

public class ChaliceOfAtonement : MonoBehaviour
{
    PlayerController player;
    public int MaxChaliceOfAtonementCount;
    public int CurrentChaliceOfAtonementCount;
    float recoveryAmount;
    bool lastRecoveryTime;

    void Awake()
    {
        TryGetComponent(out player);
        MaxChaliceOfAtonementCount = 2; // ���� ���̺꿡�� �ҷ����� �ɷ�
        CurrentChaliceOfAtonementCount = MaxChaliceOfAtonementCount;
        recoveryAmount = 30; // ���� ���̺꿡�� �ҷ�����
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
