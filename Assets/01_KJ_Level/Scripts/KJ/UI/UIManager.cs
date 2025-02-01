using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                // ������ �̹� �����ϴ� UIManager�� �˻�
                instance = FindObjectOfType<UIManager>();

                // �������� ������ ���� ����
                if (instance == null)
                {
                    GameObject go = new GameObject("UIManager");
                    instance = go.AddComponent<UIManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject); // �ߺ� ���� ����
        }
    }

    public bool IsGameMenuOpen;

    public Button[] TabButtons;

    private bool isChoose = false;

    EventSystem system;

    public Selectable firstInput;

    private Button lastSelectedButton; // ���������� ���õ� ��ư

    public GameObject SoulFragMentInfo;


    public GameObject SoulFragMentMoveInfo;


    public GameObject SoulImages;

  

    public GameObject SoulFragMentContent;

    void Start()
    {

        system = EventSystem.current;

        if (TabButtons != null && TabButtons.Length > 0 )
        {
            TabButtons[0].Select(); // ù ��° ��ư ����
            lastSelectedButton = TabButtons[0];
            ColorChange(lastSelectedButton); // �ʱ� ���� ����
        }

    }

 

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            NavigateButtons(true); 
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            NavigateButtons(false); 
        }
    }
    private void NavigateButtons(bool isShiftPressed)
    {
        // ���� ���õ� Selectable ��������
        Selectable current = system.currentSelectedGameObject?.GetComponent<Selectable>();

        if (current == null && lastSelectedButton != null) //���콺�� �� ���� ���ý� current�� null�̹Ƿ�, null�� ���
        {
            lastSelectedButton.Select(); //������ ��ư ����
            current = lastSelectedButton; //���������� ��ư�� current�� ����.
        }

        if (current != null)
        {
            Selectable next = isShiftPressed ? current.FindSelectableOnLeft() : current.FindSelectableOnRight();  //true/false�� ���� ���� �̵��� �������̵�.

            if (next != null)
            {
                Debug.Log("2");
                next.Select();

                Button nextButton = next.GetComponent<Button>(); //��
                if (nextButton != null && nextButton != lastSelectedButton)
                {
                    Debug.Log("3");
                    nextButton.onClick.Invoke();
                    lastSelectedButton = nextButton; // ������ ���õ� ��ư ������Ʈ
                    ColorChange(lastSelectedButton);
                }
            }
        }
    }

    [SerializeField]
    GameObject GameMenu;
    public void GameMenuOpen()
    {
        GameMenu.gameObject.SetActive(true);
        if (DataManager.Instance.nowPlayer.allActiveQuests.Count > 0)
        {
            QuestManager.Instance.RefreshQuestList();
        }
        else if (DataManager.Instance.nowPlayer.allCompletedQuests.Count > 0)
        {
            QuestManager.Instance.RefreshQuestList();
        }

     

     
        IsGameMenuOpen = true;
        DialogSystem.Instance.MouseOn();
    }

    public void GameMenuClose()
    {
        DialogSystem.Instance.MouseOff();
        GameMenu.gameObject.SetActive(false);
        IsGameMenuOpen = false;
  
    }


    public void SoulImageOpen(SoulFragMent soulFragMent)
    {
        SoulImages.gameObject.SetActive(true);

        RefreshSaveScenes(soulFragMent);
    }

    [SerializeField]
    private bool isSoulFragMentUI;

    private string movedScene;

    public void RefreshSaveScenes(SoulFragMent soulFragMent)
    {
        DialogSystem.Instance.MouseOn();
        isSoulFragMentUI = true;


        // ���� UI ����
        foreach (Transform child in SoulFragMentContent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < DataManager.Instance.nowPlayer.saveActiveObjects.Count; i++)
        {
            string currentScene = DataManager.Instance.nowPlayer.saveObjectSceneName[i]; // ������ �ε����� ID ��������

            // SoulFragMentInfo�� �ν��Ͻ�ȭ
            GameObject soulFragMentObj = Instantiate(SoulFragMentInfo, Vector3.zero, Quaternion.identity);
            soulFragMentObj.transform.SetParent(SoulFragMentContent.transform, false);

            // SoulLow ������Ʈ ��������
            SoulLow sLow = soulFragMentObj.GetComponent<SoulLow>();

            if (sLow != null)
            {
                sLow.currentScene.text = currentScene; // �ش� �ε����� saveObjectId�� ����

                sLow.SelectSceneButton.onClick.RemoveAllListeners();
                sLow.SelectSceneButton.onClick.AddListener(() =>
                {
                    //isSoulFragMentUI = false;
                    //SoulCanvas.gameObject.SetActive(false);
                    //SceneManager.LoadScene(currentScene);

                    SoulFragMentMoveInfo.gameObject.SetActive(true);
                    movedScene = currentScene;
                });

                
            }
            else
            {
                Debug.LogError("SoulLow component is missing on the instantiated SoulFragMent object!");
            }
        }
    
    }

    public void SelectSceneMove()
    {
  
        isSoulFragMentUI = false;
        SoulFragMentMoveInfo.gameObject.SetActive(false);
        SoulImages.gameObject.SetActive(false);
        SceneManager.LoadScene(movedScene);
    }

    public void CloseBtn()
    {
        DialogSystem.Instance.MouseOff();
        SoulImages.gameObject.SetActive(false);
    }
    public void ColorChange(Button clickedButton)
    {
 
        foreach(Button button in TabButtons)
        {
            Image buttonImage = button.GetComponent<Image>();

            if (button == clickedButton)
            {
                // Ŭ���� ��ư�� �Ķ���
                buttonImage.color = Color.blue;
            }
            else
            {
                // ������ ��ư�� ������
                buttonImage.color = Color.white;
            }
        }
    }
}
