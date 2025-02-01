﻿using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using System.Linq;

public class PlayerData
{
    public string name;
    //public int level = 1;
    //public int coin = 100;
    //  public int item = -1; 

    public Vector3 position = new Vector3(0.0f, 0.0f, -5.21f);

    public string currentScene = "StartPlayScene";
    public string currentMap = "시체 곶";

    public int previousSlot = -1;

    //퀘스트
    public List<Quest> allActiveQuests = new List<Quest>();
    public List<Quest> allCompletedQuests = new List<Quest>();
    public List<string> questGivers = new List<string>();
    

    //영혼파편
    public List<SoulFragMent> saveActiveObjects = new List<SoulFragMent>();
    public List<int> saveObjectId = new List<int>();
    public List<string> saveObjectSceneName = new List<string>();




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

        Debug.Log("데이터 로드 완료! 저장된 영혼파편 개수: " + nowPlayer.saveActiveObjects.Count);
       // Debug.Log("데이터 로드 완료! 저장된 활성화된 퀘스트 개수: " + nowPlayer.allActiveQuests.Count);
        Debug.Log("데이터 로드 완료! 저장된 완료된 퀘스트 개수: " + nowPlayer.allCompletedQuests.Count);
        Debug.Log("데이터 로드 완료! 저장된 활성화된  퀘스트 개수: " + nowPlayer.questGivers.Count);
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

    //헌신자 영혼파편 상호작용 시 현재 구역 세이브하는 부분. -> UI 띄우고 현재 구역 표시하기 위함.

    public void SaveSoulFragment(SoulFragMent soulObj,int id, string sceneName)
    {
        if(nowPlayer.saveActiveObjects.Contains(soulObj))
        {
            Debug.Log("데이터에 영혼파편이 이미 저장됨");
            return;
        }

        if (!nowPlayer.saveActiveObjects.Contains(soulObj))
        {
            nowPlayer.saveObjectId.Add(id);

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
        nowPlayer.allActiveQuests.Remove(quest);
        nowPlayer.questGivers.Add(giver);
        nowPlayer.allCompletedQuests.Add(quest);

        SaveData();
    }
}