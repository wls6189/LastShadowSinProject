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

                GameObject go = new GameObject("GameManager"); //EventBus라는 빈 객체를 만들고
                instance = go.AddComponent<GameManager>(); //EventBus 빈 객체에 EventBus 스크립트(컴포넌트)을 추가

                DontDestroyOnLoad(go);
            }
            return instance;
        }



    }
    //public TextMeshProUGUI level;
    //public TextMeshProUGUI coin;

    //public GameObject[] WeaponItem; 테스트용
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

    //헌신자 영혼파편 상호작용 시 현재 구역 세이브하는 부분. -> UI 띄우고 현재 구역 표시하기 위함.
    [Header("SaveObjectInfo")]
    public List<SaveObjectData> saveObjects; // NPC가 가지고 있는 퀘스트 목록 관리
    public SaveObjectData saveObject;

    private int ActiveSaveIndex = 0;

    public void SaveObjectFunc()
    {
        saveObject = saveObjects[ActiveSaveIndex];
        DataManager.Instance.SoulFragMentFunc(saveObject,ActiveSaveIndex);
    }
    private void Start()
    {
        // 게임 씬에 넘어오면 플레이어의 정보(이름, 레벨, 코인)가 알맞게 나타나야 함.
        //scneName.text += SceneManager.GetActiveScene().name; // name 은 현재 이름: 이므로 name += 형태로 진행.
        //level.text += DataManager.Instance.nowPlayer.level.ToString(); // text string이고 level은 int이므로 ToString()
        //coin.text += DataManager.Instance.nowPlayer.coin.ToString();

        // 아이템 설정 (주석처리된 부분, 필요시 활성화)
        // ItemSetting(DataManager.Instance.nowPlayer.item);

    }

    //public void LevelUp()
    //{
    //    DataManager.Instance.nowPlayer.level++; //값 바꾸기
    //    level.text = "레벨 : " + DataManager.Instance.nowPlayer.level.ToString(); //UI변화를 위함
    //}

    //public void CoinUp()
    //{
    //    DataManager.Instance.nowPlayer.coin += 100;
    //    coin.text = "코인 : " + DataManager.Instance.nowPlayer.coin.ToString(); //UI변화를 위함
    //}

    //public void Save()
    //{
    //    DataManager.Instance.SaveData();
    //}


    //public void ItemSetting(int number)
    //{
    //    for(int i = 0; i< WeaponItem.Length; i++)
    //    {
    //        if(number == i) //선택한 무기(0)가 첫번째(0) 라면 
    //        { 
    //            WeaponItem[i].SetActive(true); //첫번째 무기만 활성화
    //            DataManager.Instance.nowPlayer.item = number; //활성화된 무기의 번호가 PlayerData의 item 변수에 저장됨. 즉, 대응 됨.
    //        }
    //        else //아니라면
    //        {
    //            WeaponItem[i].SetActive(false); //비활성화
    //        }
    //    }
    //}
}
