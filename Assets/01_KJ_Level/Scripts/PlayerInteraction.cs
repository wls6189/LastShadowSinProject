using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerInteraction : MonoBehaviour
{
    public Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // 아이템 이름과 개수를 관리



    //InputActionAsset inputActionAsset;
    //InputAction interactAction;

    private void Start()
    {
        //interactAction = inputActionAsset.FindAction("Interact");
        transform.position = DataManager.Instance.nowPlayer.position;
    }

    public void CollectItem(string itemName)
    {
       if(collectedItems.ContainsKey(itemName))
        {
            collectedItems[itemName]++;
        }
       else
        {
            collectedItems[itemName] = 1;
        }


    } //임시 메서드1

    void InventoryCheck()
    {
        foreach (var item in collectedItems)
        {
            Debug.Log($"아이템: {item.Key}, 개수: {item.Value}");
        }

        if (collectedItems.Count == 0)
        {
            Debug.Log("인벤토리가 비어 있습니다.");
        }
    } //임시 메서드2

    public void RemoveItem(string itemName,int count) ////임시 메서드3
    {
        if (collectedItems.ContainsKey(itemName)) //인벤토리에 itemName이라는 아이템이 있을 경우. 아이템 이름으로 판별.
        {
            collectedItems[itemName] -= count;
            Debug.Log($"아이템 '{itemName}'의 개수가 {count} 만큼 감소. 남은 개수: {collectedItems[itemName]}");

            if (collectedItems[itemName] <= 0)
            {
                collectedItems.Remove(itemName); // 개수가 0 이하가 되면 딕셔너리에서 삭제
                Debug.Log($"아이템 '{itemName}'이 인벤토리에서 없어짐.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FragMent") || other.CompareTag("NextPortal") || other.CompareTag("PreviousPortal")) //상호작용 확인.
        {
            isInteraction = true;
        }
        if (other.CompareTag("FragMent")) //헌신자 영혼파편
        {
            SavePlayerData();
        }
        if (other.CompareTag("NextPortal"))
        {
            other.GetComponent<Portal>().LoadScene(true);
        }
        if (other.CompareTag("PreviousPortal"))
        {
            other.GetComponent<Portal>().LoadScene(false);
        }
    }

    [SerializeField]
    private bool isInteraction;

    [SerializeField]
    private bool isInteractionStart;
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FragMent") || other.CompareTag("NextPortal") || other.CompareTag("PreviousPortal")) 
        {
            isInteraction = false;
        }
    }

    private void SavePlayerData()
    {
        // 플레이어 위치 및 현재 씬 저장
        DataManager.Instance.nowPlayer.position = transform.position;
        Debug.Log("Player data saved." + DataManager.Instance.nowPlayer.position);
        DataManager.Instance.nowPlayer.currentScene = SceneManager.GetActiveScene().name;

        //활성화 또는 완료된 약속들 저장
        DataManager.Instance.nowPlayer.allActiveQuests = QuestManager.Instance.allActiveQuests;
        DataManager.Instance.nowPlayer.allCompletedQuests = QuestManager.Instance.allCompletedQuests;

        //현재 슬롯 저장. -> Continue 버튼을 위함.
        DataManager.Instance.nowPlayer.previousSlot = DataManager.Instance.nowSlot;

        // 데이터 저장
        DataManager.Instance.SaveData();
        Debug.Log("Player data saved." + DataManager.Instance.nowPlayer.position);
        Debug.Log("Player가 다음 슬롯을 저장함." + DataManager.Instance.nowPlayer.previousSlot);
    }
}
