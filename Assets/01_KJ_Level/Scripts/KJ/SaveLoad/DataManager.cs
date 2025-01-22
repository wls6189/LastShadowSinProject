using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerData
{
    //이름, 레벨 , 코인, 착용중인 무기 들을 저장
    public string name;
    public int level = 1;
    public int coin = 100;
    //  public int item = -1; 

    public Vector3 position = new Vector3(0.0f, 1.21f, -21f);

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

    public string path;

    public int nowSlot;

    private void Awake()
    {
        #region 싱글톤
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion

        path = Application.persistentDataPath + "/Save"; 
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


}
