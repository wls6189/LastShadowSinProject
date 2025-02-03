using UnityEngine;
using UnityEngine.UI;
using static MarkImageSO;

public class ESMSlot : MonoBehaviour
{
    PlayerController player;

    Image ESMFrame;
    [HideInInspector] public Image ESMImage;
    [HideInInspector] public EternalSpiritMark ThisESM;
    [HideInInspector] public bool IsEquiped;
    [SerializeField] GameObject selectionMenuPrefab;
    Transform selectionMenuPos;

    Color eSMFrameSelectColor = new Color(1f, 1f, 1f, 1f);
    Color eSMFrameOriginColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    void Awake()
    {
        ESMFrame = GetComponentsInChildren<Image>()[2];
        this.ESMImage = GetComponentsInChildren<Image>()[1];
        selectionMenuPos = transform.GetChild(3);
    }

    public void SetESMSlotData(Sprite eSMImage, EternalSpiritMark thisESM, PlayerController player)
    {
        ESMImage.sprite = eSMImage;
        ThisESM = thisESM;
        this.player = player;
    }
    public void PointerEnter()
    {
        ESMFrame.color = eSMFrameSelectColor;
        SMAndESMUIManager.Instance.ESMSlotPointerEnter(gameObject);
    }
    public void PointerExit()
    {
        ESMFrame.color = eSMFrameOriginColor;
        SMAndESMUIManager.Instance.ESMSlotPointerExit();
    }
    public void PointerClick()
    {
        if (IsEquiped)
        {
            GameObject go = Instantiate(selectionMenuPrefab, SMAndESMUIManager.Instance.SelectionMenuPos);
            go.transform.position = selectionMenuPos.position;
            go.GetComponent<SelectionMenu>().SetBtnIsEquiped(true, player, ThisESM);
        }
        else
        {
            GameObject go = Instantiate(selectionMenuPrefab, SMAndESMUIManager.Instance.SelectionMenuPos);
            go.transform.position = selectionMenuPos.position;
            go.GetComponent<SelectionMenu>().SetBtnIsEquiped(false, player, ThisESM);
        }

        SMAndESMUIManager.Instance.ActivePanel(true);
    }
    public void EquipedState(bool isEquiped)
    {
        if (isEquiped)
        {
            transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            IsEquiped = true;

        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            IsEquiped = false;
        }
    }
    
}
