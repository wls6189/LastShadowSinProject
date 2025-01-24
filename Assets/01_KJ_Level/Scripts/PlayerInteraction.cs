using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerInteraction : MonoBehaviour
{
    public Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // ������ �̸��� ������ ����

    private NPC currentNPC;

    InputActionAsset inputActionAsset;
    InputAction interactAction;

    private void Awake()
    {
        inputActionAsset = GetComponent<PlayerInput>().actions;
    }
    private void Start()
    {
        interactAction = inputActionAsset.FindAction("Interact");
        transform.position = DataManager.Instance.nowPlayer.position;
    }

    private void Update()
    {
        if(interactAction.WasPressedThisFrame() && isInteraction)
        {
            isInteractionStart = true;

        }

        if(interactAction.WasPressedThisFrame() && isFragMentInfo)
        {
            isFragMentInfo = false;
            GameManager.Instance.SaveObjectFunc();
           
        }    


        if (currentNPC != null && currentNPC.playerInRange)
        {
            currentNPC.NpcTalkImage.gameObject.SetActive(true);

            if (interactAction.WasPressedThisFrame() && isNpcInteraction)
            {
                isNpcInteraction = false;
                currentNPC.PlayerWithTalk();
                currentNPC.PlayerLookNpc(transform.position);
            }
               
           
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


    } //�ӽ� �޼���1

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
    } //�ӽ� �޼���2

    public void RemoveItem(string itemName,int count) ////�ӽ� �޼���3
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


    [SerializeField]
    private bool isFragMentInfo;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("FragMent") || other.CompareTag("NextPortal") || other.CompareTag("PreviousPortal")) //��ȣ�ۿ� Ȯ��.
        {
            isInteraction = true;

           
        }
        if (other.CompareTag("FragMent") && isInteractionStart) //����� ��ȥ����
        {
            other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]";
            isInteractionStart = false;

            isFragMentInfo = true;

            SavePlayerData();

           



        }
        if (other.CompareTag("NextPortal") && isInteractionStart)
        {
            isInteractionStart = false;
            other.GetComponent<Portal>().LoadScene(true);
        }
        if (other.CompareTag("PreviousPortal") && isInteractionStart)
        {
            isInteractionStart = false;
            other.GetComponent<Portal>().LoadScene(false);
        }
        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = true;
            currentNPC = other.GetComponent<NPC>();
        }
    }

    private bool isInteraction;

    private bool isNpcInteraction;


    private bool isInteractionStart;
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FragMent") || other.CompareTag("NextPortal") || other.CompareTag("PreviousPortal")) 
        {
            isInteraction = false;
        }


        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = false;
            currentNPC.NpcTalkImage.gameObject.SetActive(false);
            currentNPC = null;
        }
    }

    private void SavePlayerData()
    {
        Debug.Log("SAVE");
        // �÷��̾� ��ġ �� ���� �� ����
        DataManager.Instance.nowPlayer.position = transform.position;

        DataManager.Instance.nowPlayer.currentScene = SceneManager.GetActiveScene().name;

        //Ȱ��ȭ �Ǵ� �Ϸ�� ��ӵ� ����
        DataManager.Instance.nowPlayer.allActiveQuests = QuestManager.Instance.allActiveQuests;
        DataManager.Instance.nowPlayer.allCompletedQuests = QuestManager.Instance.allCompletedQuests;

        //���� ���� ����. -> Continue ��ư�� ����.


        // ������ ����
        DataManager.Instance.SaveData();


    }
}
