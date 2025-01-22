using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerData
{
    //�̸�, ���� , ����, �������� ���� ���� ����
    public string name;
    public int level = 1;
    public int coin = 100;
    //  public int item = -1; 

    public Vector3 position = new Vector3(0.0f, 1.21f, -21f);

    public string currentScene = "StartPlayScene";
    public string currentMap = "��ü ��";

    public int previousSlot = -1;

    public List<Quest> allActiveQuests = new List<Quest>();
    public List<Quest> allCompletedQuests = new List<Quest>();

}
public class DataManager : MonoBehaviour
{
    //���� ���� �׻� �����ϸ� �����Ƿ� �̱���
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DataManager"); //EventBus��� �� ��ü�� �����
                instance = go.AddComponent<DataManager>(); //EventBus �� ��ü�� EventBus ��ũ��Ʈ(������Ʈ)�� �߰�
            }
            return instance;
        }



    }
    public PlayerData nowPlayer = new PlayerData();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else // 1. go.AddComponent<EventBus>(); -> 2. Awake �����̹Ƿ� evetbus�� ���� null�̴�. �׷��� eventbus = this�� ���ش�. 
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        path = Application.persistentDataPath + "/Save";
    }

   

    public string path;

    public int nowSlot;

   
    public void SaveData()
    {
       
        string data = JsonUtility.ToJson(nowPlayer);

        //�����ϱ� - WriteAllText() ���
        File.WriteAllText(path  + nowSlot.ToString(), data); // ����� �����̸��� ������ �� �����͸� �����ϰ� ���� �̸� �ڿ� ������ ��ȣ���� �߰�. Save0,Save1...
    }

    public void LoadData()
    {
        //�ҷ����� - ReadAllText() ���
        string data = File.ReadAllText(path  + nowSlot.ToString()); 

        nowPlayer = JsonUtility.FromJson<PlayerData>(data); //������ �����ߴ� nowPlayer�� �ҷ����� ���� ���� ����� ��

    }

    public void DataClear()
    {
        nowSlot = -1; //���� ��ȣ�� 
        nowPlayer = new PlayerData(); //�ʱⰪ���� �ٽ� �ʱ�ȭ
    }


}
