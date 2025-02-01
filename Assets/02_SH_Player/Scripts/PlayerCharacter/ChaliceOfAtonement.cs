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
