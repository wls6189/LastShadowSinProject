using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;

public class PlayerData
{
    public string name;
    public int level = 1;
    public int coin = 100;
    //  public int item = -1; 

    public Vector3 position = new Vector3(0.0f, 0.0f, -5.21f);

    public string currentScene = "StartPlayScene";
    public string currentMap = "시체 곶";

    public int previousSlot = -1;

    public List<Quest> allActiveQuests = new List<Quest>();
    public List<Quest> allCompletedQuests = new List<Quest>();

}
public class DataManager : MonoBehaviour
{
    //게임 내에 항상 존재하면 좋으므로 싱글톤
    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DataManager"); //EventBus라는 빈 객체를 만들고
                instance = go.AddComponent<DataManager>(); //EventBus 빈 객체에 EventBus 스크립트(컴포넌트)을 추가
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
        else // 1. go.AddComponent<EventBus>(); -> 2. Awake 실행이므로 evetbus가 아직 null이다. 그래서 eventbus = this를 해준다. 
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        path = Application.persistentDataPath + "/Save";

        LoadPreviousSlot();
    }

   

    public string path;

    public int nowSlot;
    private void LoadPreviousSlot()
    {
        if (PlayerPrefs.HasKey("PreviousSlot"))
        {
            nowPlayer.previousSlot = PlayerPrefs.GetInt("PreviousSlot");
            nowSlot = nowPlayer.previousSlot;
            Debug.Log("Previous Slot Loaded: " + nowSlot);
        }
        else
        {
            nowSlot = 0; // 기본 슬롯 번호 설정
            Debug.Log("No previous slot, using default slot: " + nowSlot);
        }
    }
    public void SlotSave(int number)
    {

        nowPlayer.previousSlot = number;
        nowSlot = number;

        // PlayerPrefs에 이전 슬롯 번호 저장
        PlayerPrefs.SetInt("PreviousSlot", nowSlot);
        PlayerPrefs.Save(); // 변경 사항을 즉시 저장

        Debug.Log("Slot Saved: " + nowSlot);
    }

    public void SaveData()
    {
        string data = JsonUtility.ToJson(nowPlayer);

        //저장하기 - WriteAllText() 사용
        File.WriteAllText(path  + nowSlot.ToString(), data); // 경로의 파일이름을 지정한 후 데이터를 저장하고 파일 이름 뒤에 슬롯의 번호까지 추가. Save0,Save1...
    }

    public void LoadData()
    {
        //불러오기 - ReadAllText() 사용
        string data = File.ReadAllText(path  + nowSlot.ToString()); 

        nowPlayer = JsonUtility.FromJson<PlayerData>(data); //기존에 선언했던 nowPlayer을 불러오고 나서 덮어 씌우게 됨

    }

    public void DataClear()
    {
        nowSlot = -1; //슬롯 번호가 
      
        nowPlayer = new PlayerData(); //초기값으로 다시 초기화
       
    }
    public void DataCelarSlot()
    {
        PlayerPrefs.DeleteKey("PreviousSlot");
        PlayerPrefs.Save();

    }

    public List<SaveObjectData> saveActiveObjects = new List<SaveObjectData>();

    public void SoulFragMentFunc(SaveObjectData saveObject,int ActiveSaveIndex)
    {
        

        saveActiveObjects.Add(saveObject);
        UIManager.Instance.RefreshSaveScenes(saveActiveObjects);
    }

}
