using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {

                GameObject go = new GameObject("GameManager"); //EventBus��� �� ��ü�� �����
                instance = go.AddComponent<GameManager>(); //EventBus �� ��ü�� EventBus ��ũ��Ʈ(������Ʈ)�� �߰�

                DontDestroyOnLoad(go);
            }
            return instance;
        }



    }
    //public TextMeshProUGUI level;
    //public TextMeshProUGUI coin;

    //public GameObject[] WeaponItem; �׽�Ʈ��
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //����� ��ȥ���� ��ȣ�ۿ� �� ���� ���� ���̺��ϴ� �κ�. -> UI ���� ���� ���� ǥ���ϱ� ����.
    [Header("SaveObjectInfo")]
    public List<SaveObjectData> saveObjects; // NPC�� ������ �ִ� ����Ʈ ��� ����
    public SaveObjectData saveObject;

    private int ActiveSaveIndex = 0;

    public void SaveObjectFunc()
    {
        saveObject = saveObjects[ActiveSaveIndex];
        DataManager.Instance.SoulFragMentFunc(saveObject,ActiveSaveIndex);
    }
    private void Start()
    {
        // ���� ���� �Ѿ���� �÷��̾��� ����(�̸�, ����, ����)�� �˸°� ��Ÿ���� ��.
        //scneName.text += SceneManager.GetActiveScene().name; // name �� ���� �̸�: �̹Ƿ� name += ���·� ����.
        //level.text += DataManager.Instance.nowPlayer.level.ToString(); // text string�̰� level�� int�̹Ƿ� ToString()
        //coin.text += DataManager.Instance.nowPlayer.coin.ToString();

        // ������ ���� (�ּ�ó���� �κ�, �ʿ�� Ȱ��ȭ)
        // ItemSetting(DataManager.Instance.nowPlayer.item);

    }

    //public void LevelUp()
    //{
    //    DataManager.Instance.nowPlayer.level++; //�� �ٲٱ�
    //    level.text = "���� : " + DataManager.Instance.nowPlayer.level.ToString(); //UI��ȭ�� ����
    //}

    //public void CoinUp()
    //{
    //    DataManager.Instance.nowPlayer.coin += 100;
    //    coin.text = "���� : " + DataManager.Instance.nowPlayer.coin.ToString(); //UI��ȭ�� ����
    //}

    //public void Save()
    //{
    //    DataManager.Instance.SaveData();
    //}


    //public void ItemSetting(int number)
    //{
    //    for(int i = 0; i< WeaponItem.Length; i++)
    //    {
    //        if(number == i) //������ ����(0)�� ù��°(0) ��� 
    //        { 
    //            WeaponItem[i].SetActive(true); //ù��° ���⸸ Ȱ��ȭ
    //            DataManager.Instance.nowPlayer.item = number; //Ȱ��ȭ�� ������ ��ȣ�� PlayerData�� item ������ �����. ��, ���� ��.
    //        }
    //        else //�ƴ϶��
    //        {
    //            WeaponItem[i].SetActive(false); //��Ȱ��ȭ
    //        }
    //    }
    //}
}
