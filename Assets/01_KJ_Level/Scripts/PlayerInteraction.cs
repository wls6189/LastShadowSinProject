using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInteraction : MonoBehaviour
{
    public readonly Dictionary<string, int> collectedItems = new Dictionary<string, int>(); // ������ �̸��� ������ ����

    private NPC currentNPC;

    private SpiritShardOfTheDevoted soulfragMent;
    private SpiritSpring soulWell;
    private DecayedStamp crackedSeal;
    private RadiantTorch radiantTorch;
    private ChaosRift chaosRift;
    private DroppedItem droppedItem;
    //private MonsterSpawner monsterSpawner; �ӽÿ�


    InputActionAsset inputActionAsset;
    InputAction interactAction;

    private void Awake()
    {
        inputActionAsset = GetComponent<PlayerInput>().actions;
        
    }
    private void Start()
    {
        interactAction = inputActionAsset.FindAction("Interact");

        //if(DataManager.Instance.nowPlayer.saveActiveObjects.Contains(soulfragMent))
        //{

        //}
        transform.position = DataManager.Instance.nowPlayer.Initposition;

        GetComponent<PlayerStats>().CurrentHealth = DataManager.Instance.nowPlayer.CurrentHealth;
        GetComponent<PlayerStats>().CurrentSpiritWave = DataManager.Instance.nowPlayer.CurrentSpiritWave;
        GetComponent<PlayerStats>().CurrentSpiritMarkForce = DataManager.Instance.nowPlayer.CurrentSpiritMarkForce;

        //monsterSpawner = GameObject.Find("MonsterSpawner").GetComponent<MonsterSpawner>();
    }

    private void Update()
    {

        if (interactAction.WasPressedThisFrame() && isInteractionReady)
        {
            isInteractionStart = true;

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
            DataManager.Instance.collectedItems[itemName] += 1; // ���� ������ ���� ����
        }
        else
        {
            DataManager.Instance.collectedItems.Add(itemName, 1); // �� ������ �߰�

        }

    } //�ӽ� �޼���1


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
        if (UIManager.Instance.isPlayerDieUI == false)
        {
            StartCoroutine(UIManager.Instance.PlayerDieUI(this));
        }      
    }

    public void RespawnPlayer()
    {
        UIManager.Instance.PlayerDieUISet();
        SceneManager.LoadScene(DataManager.Instance.nowPlayer.currentScene);
        transform.position = DataManager.Instance.nowPlayer.position;

    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("FragMent") )
        {
            isInteractionReady = true;
        }
        if (other.CompareTag("SoulWell"))
        {
            isInteractionReady = true;
            if (!DataManager.Instance.nowPlayer.currentSceneSpiritSpring.Contains
            (other.GetComponent< SpiritSpring>().currentSceneSpiritSpring))
            {
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]";
            }
                
        }
        if(other.CompareTag("CrackedSeal"))
        {
            isInteractionReady = true;
            if (!DataManager.Instance.nowPlayer.currentScenesDecayedStamp.Contains
                 (other.GetComponent<DecayedStamp>().currentScenesDecayedStamp))
            {
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]";
            }
        }

        if ( other.CompareTag("NextPortal") || other.CompareTag("PreviousPortal")
            || other.CompareTag("ChaosRift") || other.CompareTag("DroppedItem") || other.CompareTag("RadiantTorch")) //��ȣ�ۿ� Ȯ��.
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
                
                other.GetComponentInChildren<TextMeshProUGUI>().text = "Interaction [F]"; // UI �ؽ�Ʈ ������Ʈ
                Debug.Log("��ȥ ������ ����.");
            }
            else // �̹� ����� ���¿��� �ٽ� ��ȣ�ۿ�
            {
                UIManager.Instance.SoulImageOpen(soulfragMent);
            }
            isInteractionStart = false;
        } //����� ��ȥ����

        if (other.CompareTag("NextPortal") && isInteractionStart)
        {
            isInteractionStart = false;
            other.GetComponent<Portal>().LoadScene(true, GetComponent<PlayerStats>());
        } //���� ��Ż
        if (other.CompareTag("PreviousPortal") && isInteractionStart)
        {
            isInteractionStart = false;
            other.GetComponent<Portal>().LoadScene(false, GetComponent<PlayerStats>());
        } //���� ��Ż
        if(other.CompareTag("SoulWell") && isInteractionStart)
        {
            isInteractionStart = false;

           soulWell = other.GetComponent<SpiritSpring>();
            other.GetComponentInChildren<TextMeshProUGUI>().text = " ";
            soulWell.InteractionPlayer();

            GetComponent<PlayerController>().PlayerChaliceOfAtonement.LoadCOAData();

        } //��ȥ��
        if(other.CompareTag("CrackedSeal") && isInteractionStart)
        {
            isInteractionStart = false;

            crackedSeal = other.GetComponent<DecayedStamp>();
            other.GetComponentInChildren<TextMeshProUGUI>().text = " ";
            crackedSeal.InteractionPlayer();

            GetComponent<PlayerController>().PlayerMarkInventory.LoadSMData();
        } //���巯�� ����

        if(other.CompareTag("ChaosRift") && isInteractionStart)
        {
            isInteractionStart = false;

            Debug.Log("ChaosRift ��ȣ�ۿ�");
            CollectItem(GetCleanName(other.gameObject.name));

            chaosRift = other.GetComponent<ChaosRift>();
            chaosRift.InteractionPlayer();
        } //ȥ����ƴ��

        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = true;
            currentNPC = other.GetComponent<NPC>();
        }
         
        if (other.CompareTag("DroppedItem") && isInteractionStart)
        {
            isInteractionStart = false;
            droppedItem = other.GetComponent<DroppedItem>();
            droppedItem.PickUpItem(this.GetComponent<PlayerController>());
        } //��� ������
        if (other.CompareTag("RadiantTorch") && isInteractionStart) //������ Ƚ��
        {
            isInteractionStart = false;
            radiantTorch = other.GetComponent<RadiantTorch>();
            radiantTorch.InteractionPlayer();
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
            soulfragMent = null;
        }
        if (other.CompareTag("DroppedItem"))
        {
            droppedItem = null;
        }
        if (other.CompareTag("RadiantTorch"))
        {
            radiantTorch = null;
        }
        if (other.CompareTag("NextPortal")|| other.CompareTag("PreviousPortal") 
            || other.CompareTag("SoulWell") || other.CompareTag("CrackedSeal") 
            || other.CompareTag("ChaosRift") || other.CompareTag("DroppedItem")
            || other.CompareTag("RadiantTorch")) //��ȣ�ۿ� Ȯ��.
        {
            isInteractionReady = false;
            other.GetComponentInChildren<TextMeshProUGUI>().text = ""; // UI �ؽ�Ʈ ������Ʈ

        }

        if (other.CompareTag("NPC"))
        {
            isNpcInteraction = false;
            currentNPC.NpcTalkImage.gameObject.SetActive(false);
        }

       
    }


    //LoadStatDataWhenQuit() �۾� ���� ������ 5�� �ش����� ��Ȱ ��ġ. 
    public void LoadStatDataWhenQuit()
    {
        //������� ��ȥ����� ��ȣ�ۿ��Ͽ� ���̺��ϰ� ���� �ٽ� �ε� ���� �� ���̺� ���� ����  CurrentHealth, MaxSpiritWave ����
        //GetComponent<PlayerStats>().CurrentHealth = DataManager.Instance.nowPlayer.MaxHealth;

        //GetComponent<PlayerStats>().CurrentSpiritWave = DataManager.Instance.nowPlayer.MaxSpiritWave;

        //GetComponent<PlayerStats>().CurrentSpiritMarkForce = DataManager.Instance.nowPlayer.MaxSpiritMarkForce;
    }
    private void SpriritShardOfTheDevotedSave()
    {
        if (DataManager.Instance.nowPlayer.currentScene == "StartPlayScene" ||
         DataManager.Instance.nowPlayer.currentScene == "LastVillage" ||
         DataManager.Instance.nowPlayer.currentScene == "Village")
        {
            DataManager.Instance.nowPlayer.currentMap = "��ü ��";
        }

        if (DataManager.Instance.nowPlayer.currentScene == "DeapSouthernVillage" ||
            DataManager.Instance.nowPlayer.currentScene == "SouthernVillage")
        {
            DataManager.Instance.nowPlayer.currentMap = "���� �Ƚ�ó";
        }

     

        //��ȥ���� ���̺� ����
        DataManager.Instance.SaveSoulFragment(soulfragMent, soulfragMent.fragmentID, soulfragMent.sceneflow.currentSceneName);

        // �÷��̾� ��ġ �� ���� �� ����
        DataManager.Instance.nowPlayer.position = transform.position; //��ġ. 
        DataManager.Instance.nowPlayer.currentScene = SceneManager.GetActiveScene().name; //����� 
        //�÷��̾� ü�� , ��ȥ�ĵ� ���� , MaxSpiritMarkForce  ����
        DataManager.Instance.nowPlayer.MaxHealth = GetComponent<PlayerStats>().CurrentHealth;
        DataManager.Instance.nowPlayer.MaxSpiritWave = GetComponent<PlayerStats>().CurrentSpiritWave;
        DataManager.Instance.nowPlayer.MaxSpiritMarkForce = GetComponent<PlayerStats>().CurrentSpiritMarkForce;

        DataManager.Instance.nowPlayer.EquipedESM = GetComponent<PlayerController>().PlayerESMInventory.EquipedESM; // �������� ������ ��ȥ����

        DataManager.Instance.nowPlayer.OwnedESM = GetComponent<PlayerController>().PlayerESMInventory.OwnedESM; // ���� ���� ������ ��ȥ����
        DataManager.Instance.nowPlayer.CurrentHealth = GetComponent<PlayerController>().PlayerStats.CurrentHealth; // ���� ü��
        DataManager.Instance.nowPlayer.CurrentSpiritWave = GetComponent<PlayerController>().PlayerStats.CurrentSpiritWave; // ���� ��ȥ�� �ĵ�
        DataManager.Instance.nowPlayer.CurrentSpiritMarkForce = GetComponent<PlayerController>().PlayerStats.CurrentSpiritMarkForce; // ���� ��ȥ���η�
        DataManager.Instance.nowPlayer.SpiritAshAmount = GetComponent<PlayerController>().PlayerStats.SpiritAsh; // ��ȥ��
                                                                                                                 //���������� ����� �͵��� json���� ����
        DataManager.Instance.nowPlayer.EquipedSpiritMark = GetComponent<PlayerController>().PlayerMarkInventory.EquipedSpiritMark; // �������� ������ ��ȥ����
        DataManager.Instance.nowPlayer.OwnedSpiritMark = GetComponent<PlayerController>().PlayerMarkInventory.OwnedSpiritMark; // ���� ���� ������ ��ȥ����

        GetComponent<PlayerController>().PlayerStats.CurrentHealth = GetComponent<PlayerController>().PlayerStats.MaxHealth;
        DataManager.Instance.SaveData();
    }


    private string GetCleanName(string originalName)
    {
        // \s* -> ������ ������ 0�� �̻��� ���� ���� ����
        //\(.*\) -> ��ȣ �ȿ� �ִ� ��� ���� ����.

        return Regex.Replace(originalName, @"\s*\(.*\)", "");
    }
}
