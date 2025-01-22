using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerInteraction : MonoBehaviour
{
    public Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // ������ �̸��� ������ ����



    //InputActionAsset inputActionAsset;
    // InputAction gameMenuAction;

    private void Start()
    {
    
        
        //gameMenuAction = inputActionAsset.FindAction("GameMenu");
    }
    void Update()
    {
 

        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryCheck();
        }

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


    }

    void InventoryCheck()
    {
        foreach (var item in collectedItems)
        {
            Debug.Log($"������: {item.Key}, ����: {item.Value}");
        }

        if (collectedItems.Count == 0)
        {
            Debug.Log("�κ��丮�� ��� �ֽ��ϴ�.");
        }
    }

   public void RemoveItem(string itemName,int count)
    {
        if (collectedItems.ContainsKey(itemName)) //�κ��丮�� itemName�̶�� �������� ���� ���. ������ �̸����� �Ǻ�.
        {
            collectedItems[itemName] -= count;
            Debug.Log($"������ '{itemName}'�� ������ {count} ��ŭ ����. ���� ����: {collectedItems[itemName]}");

            if (collectedItems[itemName] <= 0)
            {
                collectedItems.Remove(itemName); // ������ 0 ���ϰ� �Ǹ� ��ųʸ����� ����
                Debug.Log($"������ '{itemName}'�� �κ��丮���� ������.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("FragMent")) //����� ��ȥ����
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
        // �÷��̾� ��ġ �� ���� �� ����
        DataManager.Instance.nowPlayer.position = transform.position;
        DataManager.Instance.nowPlayer.currentScene = SceneManager.GetActiveScene().name;

        //Ȱ��ȭ �Ǵ� �Ϸ�� ��ӵ� ����

        Debug.Log(QuestManager.Instance.allActiveQuests);

        DataManager.Instance.nowPlayer.allActiveQuests = QuestManager.Instance.allActiveQuests;
        DataManager.Instance.nowPlayer.allCompletedQuests = QuestManager.Instance.allCompletedQuests;


        // ������ ����
        DataManager.Instance.SaveData();
        Debug.Log("Player data saved." + DataManager.Instance.nowPlayer.position);
    }
}
