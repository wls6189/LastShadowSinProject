using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    public Button[] buttons;  
    public Sprite defaultSprite; 
    public Sprite hoverSprite;    
    public Sprite clickSprite;    

    void Start()
    {
        // ��� ��ư�� ���� �̺�Ʈ �����ʸ� ����
        foreach (Button button in buttons)
        {
            // ��ư�� Image ������Ʈ ��������
            Image buttonImage = button.GetComponent<Image>();

            // ��ư�� PointerEnter, PointerExit �̺�Ʈ �߰�
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            // PointerEnter �̺�Ʈ (���콺�� �÷��� ��)
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((data) => { OnPointerEnter(buttonImage); });
            trigger.triggers.Add(entryEnter);

            // PointerExit �̺�Ʈ (���콺�� ��ư���� ���� ��)
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((data) => { OnPointerExit(buttonImage); });
            trigger.triggers.Add(entryExit);

            // PointerClick �̺�Ʈ (��ư Ŭ�� ��)
            EventTrigger.Entry entryClick = new EventTrigger.Entry();
            entryClick.eventID = EventTriggerType.PointerClick;
            entryClick.callback.AddListener((data) => { OnPointerClick(buttonImage); });
            trigger.triggers.Add(entryClick);
        }
    }

    
    private void OnPointerEnter(Image buttonImage)
    {
        buttonImage.sprite = hoverSprite;
    }

  
    private void OnPointerExit(Image buttonImage)
    {
        buttonImage.sprite = defaultSprite;
    }

    // ��ư Ŭ�� ��
    private void OnPointerClick(Image buttonImage)
    {
        buttonImage.sprite = clickSprite;

        ResetButtonImage();
    }

    private void ResetButtonImage()
    {
        // ��� ��ư�� ���� �⺻ �̹����� ����
        foreach (Button button in buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = defaultSprite;
        }
    }
}
