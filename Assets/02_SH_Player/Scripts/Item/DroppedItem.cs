using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public DropItemData DropItemData;

    public void SetDroppedItemData(DropItemData dropItemData)
    {
        DropItemData = dropItemData;
    }

    public void PickUpItem(PlayerController player) // �÷��̾� �ʿ��� ���� ������ ��ȣ�ۿ� �� �̸� ȣ���ϵ��� �ϱ�
    {
        player.PlayerStats.SpiritAsh += DropItemData.SpiritAsh; // ��ȥ�� ȹ��

        if (DropItemData.EternalSpiritMark != null) // ��� �����ۿ� ������ ��ȥ������ ���� ��
        {
            if (player.PlayerESMInventory.OwnedESM.Contains(DropItemData.EternalSpiritMark)) // �������� ���� ������ ��ȥ������ ȹ���ϰ� �̹� �ִٸ� ��ȥ�� ȹ��
            {
                player.PlayerStats.SpiritAsh += 2000;
                Debug.Log("���� ���� ������ ��ȥ������ ȹ���ؼ� ��� ��ȥ�� 2000�� ȹ���մϴ�."); // ���� �������̽������� ���̰Բ� ó��
            }
            else
            {
                player.PlayerESMInventory.OwnedESM.Add(DropItemData.EternalSpiritMark);
            }
        }

        if (DropItemData.SpiritMarks.Count > 0)
        {
            foreach (SpiritMark spiritMark in DropItemData.SpiritMarks)
            {
                if (player.PlayerMarkInventory.IsInventoryFull())
                {
                    Debug.Log("��ȥ���� �κ��丮�� ���� á���ϴ�."); // ���� �������̽������� ���̰Բ� ó��
                }
                else
                {
                    player.PlayerMarkInventory.OwnedSpiritMark.Add(spiritMark);
                }
            }
        }
    }
}
