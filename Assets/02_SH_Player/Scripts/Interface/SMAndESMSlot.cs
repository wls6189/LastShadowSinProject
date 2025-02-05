using UnityEngine;
using UnityEngine.UI;
using static MarkImageSO;

public class SMAndESMSlot : MonoBehaviour
{
    PlayerController player;

    Image ESMFrame;
    [HideInInspector] public Image SMAndESMImage;
    [HideInInspector] public EternalSpiritMark ThisESM;
    [HideInInspector] public SpiritMark ThisSM;
    [HideInInspector] public int ThisSMIndex;
    [HideInInspector] public bool IsEquiped;
    [HideInInspector] public bool IsESM;
    [SerializeField] GameObject selectionMenuPrefab;
    Transform selectionMenuPos;

    Color FrameSelectColor = new Color(1f, 1f, 1f, 1f);
    Color FrameOriginColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    void Awake()
    {
        ESMFrame = GetComponentsInChildren<Image>()[2];
        this.SMAndESMImage = GetComponentsInChildren<Image>()[1];
        selectionMenuPos = transform.GetChild(3);
    }

    public void SetESMSlotData(Sprite eSMImage, EternalSpiritMark thisESM, PlayerController player)
    {
        SMAndESMImage.sprite = eSMImage;
        ThisESM = thisESM;
        this.player = player;
        IsESM = true;
    }
    public void SetSMSlotData(Sprite sMImage, SpiritMark thisSM, PlayerController player)
    {
        SMAndESMImage.sprite = sMImage;
        ThisSM = thisSM;
        this.player = player;
        IsESM = false;
    }
    public void PointerEnter()
    {
        ESMFrame.color = FrameSelectColor;

        if (IsESM)
        {
            SMAndESMUIManager.Instance.ESMSlotPointerEnter(gameObject, true);
        }
        else
        {
            SMAndESMUIManager.Instance.ESMSlotPointerEnter(gameObject, false);
        }
    }
    public void PointerExit()
    {
        ESMFrame.color = FrameOriginColor;
        SMAndESMUIManager.Instance.ESMSlotPointerExit();
    }
    public void PointerClick()
    {
        if (SMAndESMUIManager.Instance.IsSMSwap)
        {
            SMAndESMUIManager.Instance.IsSMSwap = false;
            player.PlayerMarkInventory.SwapSpiritMark(ThisSMIndex, SMAndESMUIManager.Instance.SMSwapGo.GetComponent<SMAndESMSlot>().ThisSM);
            SMAndESMUIManager.Instance.SMSwapGo.GetComponent<SMAndESMSlot>().ThisSMIndex = ThisSMIndex;
            ThisSMIndex = -1;

            if (SMAndESMUIManager.Instance.SMSwapSelectionMenuGo != null)
            {
                Destroy(SMAndESMUIManager.Instance.SMSwapSelectionMenuGo);
                SMAndESMUIManager.Instance.ActivePanel(false);
            }
        }
        else
        {
            if (IsESM)
            {
                if (IsEquiped)
                {
                    GameObject go = Instantiate(selectionMenuPrefab, SMAndESMUIManager.Instance.ESMSelectionMenuPos);
                    go.transform.position = selectionMenuPos.position;
                    go.GetComponent<SelectionMenu>().SetESMBtnIsEquiped(true, player, ThisESM);
                }
                else
                {
                    GameObject go = Instantiate(selectionMenuPrefab, SMAndESMUIManager.Instance.ESMSelectionMenuPos);
                    go.transform.position = selectionMenuPos.position;
                    go.GetComponent<SelectionMenu>().SetESMBtnIsEquiped(false, player, ThisESM);
                }
            }
            else
            {
                if (IsEquiped)
                {
                    GameObject go = Instantiate(selectionMenuPrefab, SMAndESMUIManager.Instance.SMSelectionMenuPos);
                    go.transform.position = selectionMenuPos.position;
                    go.GetComponent<SelectionMenu>().SetSMBtnIsEquiped(true, player, ThisSM, this);
                }
                else
                {
                    GameObject go = Instantiate(selectionMenuPrefab, SMAndESMUIManager.Instance.SMSelectionMenuPos);
                    go.transform.position = selectionMenuPos.position;
                    go.GetComponent<SelectionMenu>().SetSMBtnIsEquiped(false, player, ThisSM, this);
                    SMAndESMUIManager.Instance.SMSwapGo = go;
                }
            }

            SMAndESMUIManager.Instance.ActivePanel(true);
        }
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
