using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{

    private static DialogSystem instance;
    public static DialogSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DialogSystem"); //EventBus��� �� ��ü�� �����
                instance = go.AddComponent<DialogSystem>(); //EventBus �� ��ü�� EventBus ��ũ��Ʈ(������Ʈ)�� �߰�
            }
            return instance;
        }



    }

    public bool isdialogueCanvas; //���̾�α� UI�� �����ִ��� ����

    [Header("# NPC UI")]
    public TextMeshProUGUI StoryDialogText;

    public TextMeshProUGUI NpcGiverText;

    public Button optBtn1;
    public Button optBtn2;
    public Button BackBtn;

    public Canvas DialougeCanvas; //���̾�α� UI


    public Button FirstBtn;
    public Button SecondBtn;
    public Button ReceiveBtn;


    public GameObject NpcDialouge;
    public GameObject PlayerTalkDialouge;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
           
        }
        else // 1. go.AddComponent<EventBus>(); -> 2. Awake �����̹Ƿ� evetbus�� ���� null�̴�. �׷��� eventbus = this�� ���ش�. 
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void OpenDialogUI()
    {
        DialougeCanvas.gameObject.SetActive(true);
        isdialogueCanvas = true;

        MouseOn();
    }

    public void CloseDialogUI()
    {
        DialougeCanvas.gameObject.SetActive(false);
        isdialogueCanvas = false;

        MouseOff();
    }

    public void MouseOn()
    {
        Cursor.lockState = CursorLockMode.None; //���콺 �����ֱ�
        Cursor.visible = true;
    }
    public void MouseOff()
    {
        Cursor.lockState = CursorLockMode.Locked; //���콺 �������� �ʱ�
        Cursor.visible = false;
    }
}
