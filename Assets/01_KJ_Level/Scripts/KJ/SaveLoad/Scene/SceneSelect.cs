using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelect : MonoBehaviour
{

    public GameObject PlayerNickNameImage; //����ִ� ������ ������ �� �ߴ� â 
    public TextMeshProUGUI[] slotText; //������ �ؽ�Ʈ
    public TextMeshProUGUI[] slotSceneText; //������ �ؽ�Ʈ
    public TextMeshProUGUI newPlayerName;

    bool[] savefile = new bool[3]; //�迭�̱� ������ �ʱ�ȭ ����� �۵���.
    private void Start()
    {
        //���Ժ��� ����� �����Ͱ� �����ϴ��� �Ǵ�.
        for(int i =0; i <3; i++) //������ ���� 3���� ���� �ϹǷ�, 0,1,2 ���� ���
        {
            if (File.Exists(DataManager.Instance.path + $"{i}")) //��, Save0~Save2 �߿� ������ �����Ѵٸ�.ex) Save0 ���� ����
            {
                savefile[i] = true; //�ش� Save0~Save2 �� ������ ������ true��. ex) Save0 ������ true�� 
                DataManager.Instance.nowSlot = i; //���Գѹ� �Ҵ�.  ex) Save0 ������ �ѹ��� 0�̹Ƿ� 0�� ����0������ �Ҵ�.
                DataManager.Instance.LoadData(); //�ҷ�����
                slotText[i].text = DataManager.Instance.nowPlayer.name; //�ش� ����0���� �ؽ�Ʈ�� PlayerData�� name���� ����
                slotSceneText[i].text = "���� ��ġ : " + DataManager.Instance.nowPlayer.currentMap +
                    DataManager.Instance.nowPlayer.currentScene; //�ش� ����0���� �ؽ�Ʈ�� PlayerData�� name���� ����
               
            }
            else
            {
                slotText[i].text = "�������";
                DataManager.Instance.DataClear();
            }
        }

        previousSlot = DataManager.Instance.nowPlayer.previousSlot;
    
        //DataManager.Instance.DataClear();
    }

    public int previousSlot;

    public void Slot(int number) //��ư Ŭ�� �̺�Ʈ ȣ��
    {
        DataManager.Instance.nowPlayer.previousSlot = number;

        DataManager.Instance.nowSlot = number; //ȣ������ �� �Ű������� ���� ���ڰ� ������ ��ȣ�� �Ǵ� ����.

        
        if (savefile[number]) //1. ����� �����Ͱ� �ִٸ�. ��, ������ �����ߴٸ� -> Start�� ����.
        {
            DataManager.Instance.LoadData(); //�ҷ�����
            GoGame(); //
        }
        else //2.����� �����Ͱ� ���� ��
        {         
            PlayerNickNameImageOpen();
        }
   
    }
    [SerializeField]
    GameObject slotsImage;
    [SerializeField]
    GameObject buttonsImage;
    [SerializeField]
    GameObject slotsFullImage;
    public void NewGameButton()
    {
        SlotsFullCheack();

        if(isSlotsFull == false) //������ �� �� ���� ���� ���� 
        {
            SlotImageOpen();
        }
        else
        {
            slotsFullImage.gameObject.SetActive(true);
        }
       
    }
    public void ContinueButton()
    {

        if (slotsFullImage.activeSelf == false)
        {
            if (savefile[DataManager.Instance.nowPlayer.previousSlot])
            {
                DataManager.Instance.LoadData();
                GoGame();
            }
           
        }

    }
    public void LoadButton()
    {
        SlotImageOpen();
    }
    public void SlotImageOpen()
    {
        if(slotsFullImage.activeSelf == false)
        {
            buttonsImage.gameObject.SetActive(false);
            slotsImage.gameObject.SetActive(true);
        }
     
    }
    public void SlotImageClose()
    {
        buttonsImage.gameObject.SetActive(true);
        slotsImage.gameObject.SetActive(false);
    }
    public void PlayerNickNameImageOpen() 
    {
        PlayerNickNameImage.gameObject.SetActive(true);
    }

    public void GoGame() //�� �������� ���� ��ư�� ���� �Լ� ȣ��.
    {
        //����� �����Ͱ� ���� �� -> �� �����̿��ٸ�
        if (!savefile[DataManager.Instance.nowSlot]) //Slot �޼��忡�� DataManager.Instance.nowSlot�� ������Ʈ ����� ������ ��� ����.
        {
            DataManager.Instance.nowPlayer.name = newPlayerName.text;

            DataManager.Instance.SaveData(); // ������ ����
            SceneManager.LoadScene(1);
        }

        else
        {
         

            string sceneToLoad = DataManager.Instance.nowPlayer.currentScene;

            SceneManager.LoadScene(sceneToLoad); // �� �ε�

          
           
        }

    }

    [SerializeField]
    private bool isSlotsFull;
    private void SlotsFullCheack()
    {
        isSlotsFull = false;
        for (int i = 0; i < savefile.Length; i++)
        {
            if (savefile[i] == false)
            {
                return;
            }
          
        }
        isSlotsFull = true;
    }

    public void SlotDelete(int number)
    {
        DataManager.Instance.nowSlot = number;

        if (savefile[number] == true)
        {
            savefile[number] = false;
        }
        slotText[number].text = "�������";
        slotSceneText[number].text = "���� " + number;

        // ������ ������Ƿ� ������ Ŭ����
        DataManager.Instance.DataClear();
    }
}
