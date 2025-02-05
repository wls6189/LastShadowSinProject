using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSelect : MonoBehaviour 
{

    public GameObject PlayerNickNameImage; //����ִ� ������ ������ �� �ߴ� â 
    public TextMeshProUGUI[] slotText; //������ �ؽ�Ʈ
    public TextMeshProUGUI[] slotSceneText; //������ �ؽ�Ʈ
    public TextMeshProUGUI newPlayerName;

    bool[] savefile = new bool[3]; //�迭�̱� ������ �ʱ�ȭ ����� �۵���.

    [SerializeField]
    GameObject slotsImage; //���Ե��� UI
    [SerializeField]
    GameObject buttonsImage; //���� �޴� ó���� ���̴� ��ư�鿡 ���� �̹���

    [SerializeField]
    GameObject slotDeletedImage; //���� ���� �����Ұ����� ���� �̹���
    [SerializeField]
    GameObject continueCheackImage;


    [SerializeField]
    Button SlotDeleteButtons;
    [SerializeField]
    Button ContinueCheackButton;

    [SerializeField]
    TextMeshProUGUI ContinueCheackText;

    [SerializeField]
    GameObject tempSlider;

    private void Start()
    {
        //���Ժ��� ����� �����Ͱ� �����ϴ��� �Ǵ�.
        for(int i =0; i <3; i++) //������ ���� 3���� ���� �ϹǷ�, 0,1,2 ���� ���
        {
            if (File.Exists(DataManager.Instance.path + $"{i}")) //��, Save0~Save2 �߿� ������ �����Ѵٸ�.ex) Save0 ���� ����
            {
                savefile[i] = true; //�ش� Save0~Save2 �� ������ ������ true��. ex) Save0 ������ true�� 

                DataManager.Instance.nowSlot = i; //���Գѹ� �Ҵ�.  ex) Save0 ������ �ѹ��� 0�̹Ƿ� 0�� ����0������ �Ҵ�.

                DataManager.Instance.LoadData();

                slotText[i].text = DataManager.Instance.nowPlayer.name; //�ش� ����0���� �ؽ�Ʈ�� PlayerData�� name���� ����
                slotSceneText[i].text = "���� �� : " + DataManager.Instance.nowPlayer.currentMap + "\n" +
                   "���� ���� : " +  DataManager.Instance.nowPlayer.currentScene; //�ش� ����0���� �ؽ�Ʈ�� PlayerData�� name���� ����
               
            }
            else
            {
                slotText[i].text = "�������";
                DataManager.Instance.DataClear();
        
            }
        }
       
    }



    public void Slot(int number) //��ư Ŭ�� �̺�Ʈ ȣ��
    {
        DataManager.Instance.SlotSave(number);

        if (slotDeletedImage.gameObject.activeSelf == false)
        {

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
            Debug.Log("�� ���� �ƴ�");
            string sceneToLoad = DataManager.Instance.nowPlayer.currentScene;

            SceneManager.LoadScene(sceneToLoad); // �� �ε�



        }


    }
    public void NewGameButton()
    {
        SlotsFullCheack();

        if (isSlotsFull == false) //������ �� �� ���� ���� ���� 
        {
            SlotImageOpen();
        }
        else
        {
            continueCheackImage.gameObject.SetActive(true);

            ContinueCheackText.text = "��� ������ �� á���ϴ�. ������ ���� ���ο� ������ ������ �մϴ�.";

            ContinueCheackButton.onClick.RemoveAllListeners();
            ContinueCheackButton.onClick.AddListener(() =>
            {
                continueCheackImage.gameObject.SetActive(false);
                SlotImageOpen();
                return;
            });
        }


    }


    public void ContinueButton()
    {

        SlotsEmptyCheak();

        continueCheackImage.gameObject.SetActive(true);

        if (isSlotsEmpty)
        {
           
            ContinueCheackText.text = "��� ���Ե��� ��� ����ֽ��ϴ�. ���ο� ������ ����ðڽ��ϱ�?";

            ContinueCheackButton.onClick.RemoveAllListeners();
            ContinueCheackButton.onClick.AddListener(() =>
            {
                continueCheackImage.gameObject.SetActive(false);
                SlotImageOpen();
                return;
            });
        }
        else
        {
            if (PlayerPrefs.HasKey("PreviousSlot"))
            {
                
                int currentSlot = PlayerPrefs.GetInt("PreviousSlot");

                Debug.Log("������ ���� Slot " + currentSlot);

                ContinueCheackText.text = "�ֱ� ������ ������ " + currentSlot + "�� �Դϴ�. �ش� �������� ��� �����Ͻðڽ��ϱ�?";

                ContinueCheackButton.onClick.RemoveAllListeners();
                ContinueCheackButton.onClick.AddListener(() =>
                {
                    continueCheackImage.gameObject.SetActive(false);

                    if (continueCheackImage.activeSelf == false)
                    {
                        int previousSlot = PlayerPrefs.GetInt("PreviousSlot");  // ���� ���� ��ȣ ��������
                        Debug.Log("������ ���� Slot " + previousSlot);
                        // ���� ���� ��ȣ�� ��ȿ�� ���
                        if (previousSlot >= 0 && previousSlot < savefile.Length && savefile[previousSlot])
                        {
                            // �ش� ���Կ��� ������ �ε�
                            DataManager.Instance.nowSlot = previousSlot;
                            DataManager.Instance.LoadData();
                            Debug.Log("Slot " + previousSlot + " data loaded.");

                            // ���� ����
                            GoGame();
                        }
                        else
                        {
                            Debug.Log("���� ���Կ� ����� �����Ͱ� �����ϴ�.");
                          
                        }
                    }
                });
            }
            else
            {
                Debug.Log("Ű�� �����ϴ�.");

                ContinueCheackText.text = "�ֱ� ������ ������ �����ϴ�. ���ο� ������ ����ðڽ��ϱ�?";
                ContinueCheackButton.onClick.RemoveAllListeners();
                ContinueCheackButton.onClick.AddListener(() =>
                {
                    continueCheackImage.gameObject.SetActive(false);
                    SlotImageOpen();
                    return;
                });
            }
          
        }

       

      
    }

    public void LoadButton()
    {
        SlotImageOpen();
    }

  

    public void SlotImageOpen()
    {
        if(continueCheackImage.activeSelf == false)
        {
            buttonsImage.gameObject.SetActive(false);
            slotsImage.gameObject.SetActive(true);
        }
     
    }
    public void SlotImageClose() //���� UI - �ݱ� ��ư
    {
        if(slotDeletedImage.gameObject.activeSelf == false) //���� ���� ���� �̹����� ��Ȱ��ȭ �� ���� �Ʒ� ui Ȱ��ȭ
        {
            buttonsImage.gameObject.SetActive(true);
            slotsImage.gameObject.SetActive(false);
        }
    
    }
    public void PlayerNickNameImageOpen()
    {
        if (slotDeletedImage.gameObject.activeSelf == false)
        {
            PlayerNickNameImage.gameObject.SetActive(true);
        }
       
    }


    public void SlotDeleteButton(int number)
    {
        string filePath = DataManager.Instance.path + $"{number}";

        if (slotDeletedImage.gameObject.activeSelf == false && File.Exists(filePath))
        {
            slotDeletedImage.gameObject.SetActive(true);
            SlotDeleteButtons.onClick.RemoveAllListeners();
            SlotDeleteButtons.onClick.AddListener(() =>
            {
                slotDeletedImage.gameObject.SetActive(false);
                SlotDeleted(number);
            });
        }



    }
    private void SlotDeleted(int number)
    {
        DataManager.Instance.nowSlot = number;

        if (savefile[number] == true)
        {
            savefile[number] = false;

            string filePath = DataManager.Instance.path + $"{number}";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Slot {number} data file deleted.");
            }
            else
            {
                Debug.LogWarning($"Slot {number} data file not found.");
            }

        }
        slotText[number].text = "�������";
        slotSceneText[number].text = "���� " + number;

        // ������ ������Ƿ� ������ Ŭ����
        DataManager.Instance.DataClear();
        DataManager.Instance.DataCelarSlot();
    }

    [SerializeField]
    private bool isSlotsFull;
    private void SlotsFullCheack()
    {
        isSlotsFull = false;
        for (int i = 0; i < savefile.Length; i++)
        {
            if (savefile[i] == false) //���̺����Ͽ� �ϳ��� ������� ���
            {
                return;
            }
          
        }
        isSlotsFull = true;
    }

    [SerializeField]
    public bool isSlotsEmpty;
    private void SlotsEmptyCheak()
    {
        isSlotsEmpty = true;

        for (int i = 0; i < savefile.Length; i++)
        {
            if (savefile[i] == true) // �ϳ��� ä���� �ִ� ���
            {
                isSlotsEmpty = false;  // ���̺����Ͽ� �ϳ��� ä���� ������ ������� ����
                return;  // �� �̻� üũ�� �ʿ� �����Ƿ� ����
            }

        }

    }

  
    public void QuitButton()
    {
        if(continueCheackImage.activeSelf == false)
        {
            Application.Quit();
        }
    }

    public void OptionButton()
    {
        tempSlider.SetActive(true);
    }
    public void OffOption()
    {
        tempSlider.SetActive(false);
    }

}
