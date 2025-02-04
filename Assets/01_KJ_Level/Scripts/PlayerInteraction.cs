using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerInteraction : MonoBehaviour
{
    public readonly Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // ������ �̸��� ������ ����

    private NPC currentNPC;

    private SpiritShardOfTheDevoted soulfragMent;
    private SpiritSpring soulWell;
    private DecayedStamp crackedSeal;
    private ChaosRift chaosRift;
    private DroppedItem droppedItem;
    private MonsterSpawner monsterSpawner;


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

        monsterSpawner = GameObject.Find("MonsterSpawner").GetComponent<MonsterSpawner>();

        LoadStatDataWhenQuit();
    }

    private void Update()
    {

        if (interactAction.WasPressedThisFrame() && isInteractionReady)
        {
            isInteractionStart = true;

        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            monsterSpawner.OnMonsterDeath(); //�ӽ� ���
            //InventoryCheck();
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

        if (collectedItems.ContainsKey(itemName))
        {
            collectedItems[itemName] += 1; // ���� ������ ���� ����
        }
        else
        {
            collectedItems.Add(itemName, 1); // �� ������ �߰�

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

  
    public void PlayerDie()
    {
        SceneManager.LoadScene(DataManager.Instance.nowPlayer.currentScene);
        transform.position = DataManager.Instance.nowPlayer.position;
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("FragMent"))
        {
            isInteractionReady = true;
        }

        if ( other.CompareTag("NextPortal") 
            || other.CompareTag("PreviousPortal") || other.CompareTag("SoulWell")
            || other.CompareTag("CrackedSeal") || other.CompareTag("ChaosRift")
            || other.CompareTag("DroppedItem")) //��ȣ�ۿ� Ȯ��.
        {
            isInteractionReady = true;
            other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]"; // UI �ؽ�Ʈ ������Ʈ                
        }
        if (other.CompareTag("FragMent") && isInteractionStart) //����� ��ȥ����
        {
            soulfragMent = other.GetComponent<SpiritShardOfTheDevoted>();

            if (!soulfragMent.isSave) // ó�� ��ȣ�ۿ� �� ����
            {
                GetComponent<PlayerStats>().CurrentHealth = DataManager.Instance.nowPlayer.InitMaxHealth;

                SpriritShardOfTheDevotedSave();


                soulfragMent.isSave = true; // �̹� ����� ���·� �÷��� ����
                DataManager.Instance.SaveSoulFragment(soulfragMent, soulfragMent.fragmentID,soulfragMent.sceneflow.currentSceneName);
                
                //DataManager.Instance.SaveObjectFunc(soulfragMent);
                
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Saved! [F]"; // UI �ؽ�Ʈ ������Ʈ
                Debug.Log("��ȥ ������ ����.");
            }
            else // �̹� ����� ���¿��� �ٽ� ��ȣ�ۿ�
            {
                UIManager.Instance.SoulImageOpen(soulfragMent);
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]"; // UI �ؽ�Ʈ ������Ʈ
                Debug.Log("��ȥ ������ �̹� ����.");
            }
            isInteractionStart = false;
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
        if(other.CompareTag("SoulWell") && isInteractionStart)
        {
            isInteractionStart = false;
           

            soulWell = other.GetComponent<SpiritSpring>();
            soulWell.InteractionPlayer();

        }
        if(other.CompareTag("CrackedSeal") && isInteractionStart)
        {
            isInteractionStart = false;

            crackedSeal = other.GetComponent<DecayedStamp>();
            crackedSeal.InteractionPlayer();
        }

        if(other.CompareTag("ChaosRift") && isInteractionStart)
        {
            isInteractionStart = false;

            Debug.Log("ChaosRift ��ȣ�ۿ�");
            CollectItem(GetCleanName(other.gameObject.name));

            chaosRift = other.GetComponent<ChaosRift>();
            chaosRift.InteractionPlayer();
        }

        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = true;
            currentNPC = other.GetComponent<NPC>();
        }

        if (other.CompareTag("DroppedItem") & isInteractionStart)
        {
            isInteractionStart = false;
            droppedItem = other.GetComponent<DroppedItem>();
            droppedItem.PickUpItem(this.GetComponent<PlayerController>());
        }
    }
    [SerializeField]
    private bool isInteractionReady;

    private bool isNpcInteraction;

    [SerializeField]
    private bool isInteractionStart;
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FragMent"))
        {
            isInteractionReady = false;
            soulfragMent = null;
        }
        if (other.CompareTag("DroppedItem"))
        {
            isInteractionStart = false;
            droppedItem = null;
        }

        if (other.CompareTag("NextPortal")|| other.CompareTag("PreviousPortal") 
            || other.CompareTag("SoulWell") || other.CompareTag("CrackedSeal") 
            || other.CompareTag("ChaosRift") || other.CompareTag("DroppedItem")) //��ȣ�ۿ� Ȯ��.
        {
            isInteractionReady = false;
            other.GetComponentInChildren<TextMeshProUGUI>().text = ""; // UI �ؽ�Ʈ ������Ʈ

        }

        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = false;
            currentNPC.NpcTalkImage.gameObject.SetActive(false);
            currentNPC = null;
        }

       
    }



    public void LoadStatDataWhenQuit()
    {
        //������� ��ȥ����� ��ȣ�ۿ��Ͽ� ���̺��ϰ� ���� �ٽ� �ε� ���� �� ���̺� ���� ����  CurrentHealth, MaxSpiritWave ����
        GetComponent<PlayerStats>().CurrentHealth = DataManager.Instance.nowPlayer.MaxHealth;

        GetComponent<PlayerStats>().CurrentSpiritWave = DataManager.Instance.nowPlayer.MaxSpiritWave;

        GetComponent<PlayerStats>().CurrentSpiritMarkForce = DataManager.Instance.nowPlayer.MaxSpiritMarkForce;
    }
    private void SpriritShardOfTheDevotedSave()
    {
        // �÷��̾� ��ġ �� ���� �� ����
        DataManager.Instance.nowPlayer.position = transform.position; //��ġ. 
        DataManager.Instance.nowPlayer.currentScene = SceneManager.GetActiveScene().name; //����� 
        //�÷��̾� ü�� , ��ȥ�ĵ� ���� , MaxSpiritMarkForce  ����
        DataManager.Instance.nowPlayer.MaxHealth = GetComponent<PlayerStats>().CurrentHealth;
        DataManager.Instance.nowPlayer.MaxSpiritWave = GetComponent<PlayerStats>().CurrentSpiritWave;
        DataManager.Instance.nowPlayer.MaxSpiritMarkForce = GetComponent<PlayerStats>().CurrentSpiritMarkForce;
        //���������� ����� �͵��� json���� ����
        DataManager.Instance.SaveData();

    }


    private string GetCleanName(string originalName)
    {
        // \s* -> ������ ������ 0�� �̻��� ���� ���� ����
        //\(.*\) -> ��ȣ �ȿ� �ִ� ��� ���� ����.

        return Regex.Replace(originalName, @"\s*\(.*\)", "");
    }
}
