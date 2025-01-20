using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // 드롭된 아이템 데이터
    private bool isPlayerNearby = false; // 플레이어가 범위 안에 있는지 확인

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true; // 플레이어가 범위 안에 있음
        }
    
    if (isPlayerNearby && Input.GetKeyDown(KeyCode.F)) // 범위 내에서 F 키를 눌렀을 때
        {
            // 플레이어의 인벤토리를 가져오기
            GameObject player = GameObject.FindGameObjectWithTag("Player");
    Inventory playerInventory = player.GetComponent<Inventory>();

            if (playerInventory != null)
            {
                playerInventory.AddItem(item); // 아이템 추가
                Destroy(gameObject); // 아이템 오브젝트 제거
}
        }
    }
}

