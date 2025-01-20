using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data",menuName = "ScriptableObjects/QuestInfo",order = 1)]
public class QuestInfo : ScriptableObject
{
    [Header("Start")]
    public string InitialFirstQuestion; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문
    public string InitialSecondQuestion; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문
    public string InitialFirstAnswer; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문
    public string InitialSecondAnswer; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문

    [Header("Comback")]
    public string AcceptCombackAnswer;
    [Header("Decline")]
    public string DeclineAnswer;
    [Header("Finisih")]
    public string FinishAnswer;
    [Header("CombackFinish")]
    public string CombackFinishAnswer; //퀘스트를 깼는데도 다시 돌아와서 말을 걸 경우
    [Header("AcceptFinisih")]
    public string AcceptThankyouAnswer;

    [Header("End")]
    public string finalFirstAnswer;
    public string finalSecondAnswer;


    [Header("Option_1")]
    [TextArea(5, 10)]
    public List<string> First_Answer1;
    [TextArea(5, 10)]
    public List<string> First_Question1;

    [TextArea(5, 10)]
    public List<string> First_Answer2;
    [TextArea(5, 10)]
    public List<string> First_Question2;
    [Header("Option_2")]
    [TextArea(5, 10)]
    public List<string> Second_Answer1;
    [TextArea(5, 10)]
    public List<string> Second_Question1;

    [TextArea(5, 10)]
    public List<string> Second_Answer2;
    [TextArea(5, 10)]
    public List<string> Second_Question2;

    [TextArea(5, 10)]
    public string hintExplain;

    [Header("Rewards")] //보상, 여기에 더 많은 것을 추가할 수 있다.
    public string rewardItem1;
    public string rewardItem2;

    [Header("Requirments")] //요구사항, 더 많은 것을 추가할 수 있다.
    public string firstRequirmentItem; //첫번째 요구사항, ex ) 돌이 필요함
    public int firstRequirmentAmount; //  ex) 돌이 5개 정도 필요함. 

    public string secondRequirmentItem; //두번째 요구사항, ex) 막대기도 필요함
    public int secondRequirmentAmount; // ex)막대기 2개 정도 필요함

    // => 즉, 돌과 막대기를 요구했고, 돌 5개와 막대기 2개가 필요하다는 말.


    

}