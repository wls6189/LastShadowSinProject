using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerControllerTest : MonoBehaviour
{
    public Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // 아이템 이름과 개수를 관리


    [SerializeField]
    float moveSpeed = 3.0f;

    //InputActionAsset inputActionAsset;
    // InputAction gameMenuAction;

    private void Start()
    {
    
        
        //gameMenuAction = inputActionAsset.FindAction("GameMenu");
    }
    void Update()
    {
        if(DialogSystem.Instance.isdialogueCanvas == false)
        {
            MovePlayer();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryCheck();
        }

        OnInteraction();
    }
    void OnInteraction()
    {

        if (Input.GetKeyDown(KeyCode.Escape )) //  if (gameMenuAction.WasPressedThisFrame()) 나중에 합쳐지면 이걸로 할 예정.
        {
            if (UIManager.Instance.IsGameMenuOpen) 
            {
                UIManager.Instance.GameMenuClose(); 
            }
            else
            {
                UIManager.Instance.GameMenuOpen(); 
            }
        }
    }
    void MovePlayer()
    {
        float h = Input.GetAxisRaw("Horizontal");

        Vector3 movement = new Vector3(h, 0, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
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

        Debug.Log("획득한 아이템" + itemName + "총 개수:" + collectedItems[itemName]);
    }

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
    }

   public void RemoveItem(string itemName,int count)
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
        if(other.CompareTag("FragMent"))
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


 

    private void SavePlayerData()
    {
        // 플레이어 위치 및 현재 씬 저장
        DataManager.Instance.nowPlayer.position = transform.position;
        DataManager.Instance.nowPlayer.currentScene = SceneManager.GetActiveScene().name;

        //활성화 또는 완료된 약속들 저장

        Debug.Log(QuestManager.Instance.allActiveQuests);

        DataManager.Instance.nowPlayer.allActiveQuests = QuestManager.Instance.allActiveQuests;
        DataManager.Instance.nowPlayer.allCompletedQuests = QuestManager.Instance.allCompletedQuests;


        // 데이터 저장
        DataManager.Instance.SaveData();
        Debug.Log("Player data saved." + DataManager.Instance.nowPlayer.position);
    }
}
