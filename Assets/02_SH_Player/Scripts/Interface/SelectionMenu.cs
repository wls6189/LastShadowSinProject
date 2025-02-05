using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SelectionMenu : MonoBehaviour
{
    PlayerController player;
    EternalSpiritMark eSM;
    SpiritMark sM;
    SMAndESMSlot sMAndESMSlot;

    GameObject eSMEquipBtn;
    GameObject eSMUnequipBtn;
    GameObject sMEquipBtn;
    GameObject sMUnequipBtn;
    GameObject cancelBtn;

    Color FrameSelectColor = new Color(1f, 1f, 1f, 1f);
    Color FrameOriginColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    void Awake()
    {
        eSMEquipBtn = transform.GetChild(0).gameObject; // 자식 오브젝트의 순서 중요!!!!!!!!!!!!!!!!!!!
        eSMUnequipBtn = transform.GetChild(1).gameObject;
        sMEquipBtn = transform.GetChild(2).gameObject; 
        sMUnequipBtn = transform.GetChild(3).gameObject;
        cancelBtn = transform.GetChild(4).gameObject;
    }
    private void Update()
    {
        //if (SMAndESMUIManager.Instance.PanelClicked)
        //{
        //    DestroyUI();
        //}
    }
    public void SetESMBtnIsEquiped(bool isEquiped, PlayerController player, EternalSpiritMark eSM)
    {
        sMEquipBtn.SetActive(false);
        sMUnequipBtn.SetActive(false);
        if (isEquiped)
        {

            eSMEquipBtn.SetActive(false);
        }
        else
        {
            eSMUnequipBtn.SetActive(false);
        }

        this.player = player;
        this.eSM = eSM;
    }
    public void SetSMBtnIsEquiped(bool isEquiped, PlayerController player, SpiritMark sM, SMAndESMSlot sMAndESMSlot)
    {
        eSMEquipBtn.SetActive(false);
        eSMUnequipBtn.SetActive(false);
        if (isEquiped)
        {
            sMEquipBtn.SetActive(false);
        }
        else
        {
            sMUnequipBtn.SetActive(false);
        }

        this.player = player;
        this.sM = sM;
        this.sMAndESMSlot = sMAndESMSlot;
    }
    public void PointerEnter(GameObject go)
    {
        go.GetComponent<Image>().color = FrameSelectColor;
    }
    public void PointerExit(GameObject go)
    {
        go.GetComponent<Image>().color = FrameOriginColor;
    }
    public void PointerClick(GameObject go)
    {
        bool findSwapSlot = false;

        if (go.name == "ESMEquip")
        {
            player.PlayerESMInventory.SwapESM(eSM);
            DestroyUI();
        }
        else if (go.name == "ESMUnequip")
        {
            player.PlayerESMInventory.EquipedESM = null;
            SMAndESMUIManager.Instance.SetOwnedESMList();
            DestroyUI();
        }
        else if (go.name == "SMEquip")
        {
            for (int i = 0; i < 1 + DataManager.Instance.nowPlayer.DecayedStampCount; i++)
            {
                if (player.PlayerMarkInventory.EquipedSpiritMark[i] == null)
                {
                    player.PlayerMarkInventory.SwapSpiritMark(i, sM);
                    findSwapSlot = true;
                    break;
                }
            }

            if (findSwapSlot)
            {
                DestroyUI();
                return;
            }

            SMAndESMUIManager.Instance.IsSMSwap = true;
            SMAndESMUIManager.Instance.SMSwapGo = sMAndESMSlot.gameObject;
            SMAndESMUIManager.Instance.SMSwapSelectionMenuGo = gameObject;
        }
        else if (go.name == "SMUnequip")
        {
            player.PlayerMarkInventory.SwapSpiritMark(sMAndESMSlot.ThisSMIndex, null);
            SMAndESMUIManager.Instance.SetOwnedSMList();
            DestroyUI();
        }
        else if (go.name == "Cancel")
        {
            DestroyUI();
        }
    }
    public void DestroyUI()
    {
        Destroy(gameObject);
        SMAndESMUIManager.Instance.ActivePanel(false);
    }
}
