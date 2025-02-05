using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PlayerData
{
    //플레이어
    public string name; //닉네임
    public Vector3 Initposition = new Vector3(0.0f, 0.0f, -5.21f);
    public Vector3 position = new Vector3(0.0f, 0.0f, -5.21f);
    public string currentScene = "StartPlayScene";
    public string currentMap = "시체 곶";
    public int previousSlot = -1; //PlayePrefs 

    public float InitMaxHealth = 100.0f;

    public float MaxHealth = 100.0f;
    public float MaxSpiritWave = 10.0f;
    public float MaxSpiritMarkForce = 100.0f;
    public float CurrentHealth = 100.0f;
    public float CurrentSpiritWave = 0.0f;
    public float CurrentSpiritMarkForce = 0.0f;

    public float SpiritAshAmount = 0;
    public EternalSpiritMark EquipedESM = null;
    public HashSet<EternalSpiritMark> OwnedESM = new();
    public SpiritMark[] EquipedSpiritMark;
    public PlayerData()
    {
        EquipedSpiritMark = new SpiritMark[1 + DecayedStampCount];
    }
    public List<SpiritMark> OwnedSpiritMark = new();

    //퀘스트
    public List<Quest> allActiveQuests = new List<Quest>();
    public List<Quest> allCompletedQuests = new List<Quest>();

    public List<string> questGivers = new List<string>();
    

    //영혼파편
    public List<SpiritShardOfTheDevoted> saveActiveObjects = new List<SpiritShardOfTheDevoted>();
    public List<int> SpiritShardOfTheDevotedID = new List<int>();
    public List<string> saveObjectSceneName = new List<string>();

    //영혼샘
    public int SpiritSpringCount = 0;
    public List<string> currentSceneSpiritSpring = new List<string>();

    //문드러진 도장
    public int DecayedStampCount = 0;
    public List<string> currentScenesDecayedStamp = new List<string>();

    //부음받은 자 
    public bool isOpenChaosRift = true; //혼돈의 틈새가 아직 열려있는 상태 여부 , 처음엔 열려 있음
    public bool isReCloseChaosRift; //혼돈의 틈새가 닫혀있는 상태에서 상호작용 할 시 
    public bool isPromise;
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
        else 
        {
            Destroy(this.gameObject);
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
        File.WriteAllText(path + nowSlot.ToString(), data); // 경로의 파일이름을 지정한 후 데이터를 저장하고 파일 이름 뒤에 슬롯의 번호까지 추가. Save0,Save1...
    }

    public void LoadData()
    {

        if (!File.Exists(path + nowSlot.ToString()))
        {
            Debug.LogWarning("저장된 데이터가 없습니다. 기본 데이터 사용.");
            return;
        }

        string data = File.ReadAllText(path + nowSlot.ToString());
        nowPlayer = JsonUtility.FromJson<PlayerData>(data);  

        Debug.Log("데이터 로드 완료! 플레이어 현재 체력 : " + nowPlayer.MaxHealth);
        Debug.Log("데이터 로드 완료! 플레이어 현재 영혼 파동  : " + nowPlayer.MaxSpiritWave);


        Debug.Log("데이터 로드 완료! 저장된 영혼파편 개수: " + nowPlayer.saveActiveObjects.Count);
        Debug.Log("데이터 로드 완료! 저장된 활성화 퀘스트 개수: " + nowPlayer.allActiveQuests.Count);
        Debug.Log("데이터 로드 완료! 저장된 완료된 퀘스트 개수: " + nowPlayer.allCompletedQuests.Count);
        Debug.Log("데이터 로드 완료! 저장된 퀘스트 제공자 몇명? : " + nowPlayer.questGivers.Count);
        Debug.Log("데이터 로드 완료! 저장된 영혼샘 상호작용 횟수:" + nowPlayer.SpiritSpringCount);
        Debug.Log("데이터 로드 완료! 저장된 문드러진 도장 상호작용 횟수:" + nowPlayer.DecayedStampCount);
  
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

    public Dictionary<string, int> collectedItems = new Dictionary<string, int>();

    //헌신자 영혼파편 상호작용 시 현재 구역 세이브하는 부분. -> UI 띄우고 현재 구역 표시하기 위함.

    public void SaveSoulFragment(SpiritShardOfTheDevoted soulObj,int id, string sceneName)
    {
        if(nowPlayer.saveActiveObjects.Contains(soulObj))
        {
            Debug.Log("데이터에 영혼파편이 이미 저장됨");
            return;
        }

        if (!nowPlayer.saveActiveObjects.Contains(soulObj))
        {
            nowPlayer.SpiritShardOfTheDevotedID.Add(id);

            nowPlayer.saveObjectSceneName.Add(sceneName);

            nowPlayer.saveActiveObjects.Add(soulObj);     

            SaveData();

        }
    }

    public void SaveActiveQuest(string giver,Quest quest)
    {
        Debug.Log("퀘스트 제공자는 " + giver);


        nowPlayer.questGivers.Add(giver);
        nowPlayer.allActiveQuests.Add(quest);

        SaveData();
    }
    public void SaveCompletedQuest(string giver,Quest quest)
    {
        nowPlayer.allActiveQuests.Clear();

        nowPlayer.questGivers.Add(giver);
        nowPlayer.allCompletedQuests.Add(quest);

        SaveData();
    }
}