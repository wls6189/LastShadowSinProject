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


    [Header("FirstIntracition")]
    [TextArea(5, 10)]
    public string InitExplain;
    public string InitialFirstQuestion; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문
    public string InitialSecondQuestion; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문
    public string InitialBackQuestion;  
    [TextArea(5, 10)]
    public string InitialFirstAnswer; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문
    [TextArea(5, 10)]
    public string InitialSecondAnswer; //초기 대화상자가 될 문자열 리스트 -> 선언 이유 한번에 전체 대화 상자를 표시하고 싶지 않기 때문
    [TextArea(5, 10)]
    public string InitialFinishAnswer;

    [Header("SecondIntracition")]
    public string OpenChaosRiftExplain;
    public string OpenChaosRiftFirstQuestion;
    public string OpenChaosRiftSecondQuestion;
    [TextArea(5, 10)]
    public string OpenChaosRiftFirstAnswer; 
    [TextArea(5, 10)]
    public string OpenChaosRiftSecondAnswer;
    [TextArea(5, 10)]
    public string OpenChaosRiftFinishAnswer;

    [Header("ThirdIntracition")]
    [TextArea(5, 10)]
    public string CloseChaosRiftExplain;
    public string CloseChaosRiftFirstQuestion;
    public string CloseChaosRiftSecondQuestion;
    [TextArea(5, 10)]
    public string CloseChaosRiftFirstAnswer;
    [TextArea(5, 10)]
    public string CloseChaosRiftSecondAnswer;
    [TextArea(5, 10)]
    public string CloseChaosRiftFinishAnswer;

    [Header("FourIntracition")]
    [TextArea(5, 10)]
    public string ReCloseChaosRiftExplain;
    public string ReCloseChaosRiftFirstQuestion;
    public string ReCloseChaosRiftSecondQuestion;
    [TextArea(5, 10)]
    public string ReCloseChaosRiftFirstAnswer;
    [TextArea(5, 10)]
    public string ReCloseChaosRiftSecondAnswer;
    [TextArea(5, 10)]
    public string ReCloseChaosRiftFinishAnswer;


    [Header("Option")]
    public string ClearPromiseAnswer;


    [TextArea(5, 10)]
    public string hintExplain;

    [Header("Rewards")] //보상, 여기에 더 많은 것을 추가할 수 있다.
    public GameObject rewardItem1;

    [SerializeField]
    public Sprite rewardImage;


    [Header("Requirments")] //요구사항, 더 많은 것을 추가할 수 있다.
    public string firstRequirment; //첫번째 요구사항, ex ) 돌이 필요함
    public int firstRequirmentAmount; //  ex) 돌이 5개 정도 필요함. 

    public string secondRequirment; //두번째 요구사항, ex) 막대기도 필요함
    public int secondRequirmentAmount; // ex)막대기 2개 정도 필요함

    // => 즉, 돌과 막대기를 요구했고, 돌 5개와 막대기 2개가 필요하다는 말.


    

}