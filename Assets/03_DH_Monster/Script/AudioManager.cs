using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx { Guard,Hit,Dead,EnemyAtk, Warning }//플레이어나 npc용 사운드이름 추가


    [Header("#Monster SFX")]
    private Dictionary<int, AudioClip[]> monsterSfxDict = new Dictionary<int, AudioClip[]>(); // 몬스터별 효과음 저장
    private Dictionary<int, AudioSource[]> monsterSfxPlayers = new Dictionary<int, AudioSource[]>(); // 몬스터별 오디오 소스 저장
    private Dictionary<int, int> monsterChannelIndex = new Dictionary<int, int>(); // 몬스터별 채널 인덱스


    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        //bgm 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;


        //sfx 초기화
        GameObject sfxObject = new GameObject("sfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    public void Playsfx(Sfx sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;


            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
     // 몬스터별 효과음을 등록하는 함수
    public void RegisterMonsterSfx(int monsterNumber, AudioClip[] sfxClips)
    {
        if (!monsterSfxDict.ContainsKey(monsterNumber))
        {
            monsterSfxDict.Add(monsterNumber, sfxClips);

            // 몬스터별 AudioSource 생성 (동적)
            GameObject sfxObject = new GameObject($"MonsterSfx_{monsterNumber}");
            sfxObject.transform.parent = transform;
            AudioSource[] sfxPlayers = new AudioSource[channels];

            for (int i = 0; i < channels; i++)
            {
                sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
                sfxPlayers[i].playOnAwake = false;
                sfxPlayers[i].volume = sfxVolume;
            }

            monsterSfxPlayers.Add(monsterNumber, sfxPlayers);
            monsterChannelIndex.Add(monsterNumber, 0);
        }
    }

    // 몬스터별 효과음 재생
    public void PlayMonsterSfx(int monsterNumber, int sfxIndex)
    {
        if (!monsterSfxDict.ContainsKey(monsterNumber) || !monsterSfxPlayers.ContainsKey(monsterNumber))
            return;

        AudioClip[] clips = monsterSfxDict[monsterNumber];
        if (sfxIndex < 0 || sfxIndex >= clips.Length) return;

        AudioSource[] sfxPlayers = monsterSfxPlayers[monsterNumber];
        int index = monsterChannelIndex[monsterNumber];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (index + i) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            monsterChannelIndex[monsterNumber] = loopIndex;
            sfxPlayers[loopIndex].clip = clips[sfxIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
    //  BGM 볼륨 조절
    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmPlayer.volume = bgmVolume;
    }

    //  SFX 볼륨 조절 (플레이어 & NPC)
    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (AudioSource source in sfxPlayers)
        {
            source.volume = sfxVolume;
        }
    }

    //  전체 몬스터의 SFX 볼륨 조절
    public void SetAllMonsterSfxVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);

        // 모든 몬스터에 대해 볼륨 조절
        foreach (var monsterSfx in monsterSfxPlayers.Values)
        {
            foreach (AudioSource source in monsterSfx)
            {
                source.volume = clampedVolume;
            }
        }
    }

    //  전체 볼륨 조절 (BGM + SFX + 몬스터 SFX)
    public void SetMasterVolume(float volume)
    {
        SetBgmVolume(volume);
        SetSfxVolume(volume);
        SetAllMonsterSfxVolume(volume);
    }

}

