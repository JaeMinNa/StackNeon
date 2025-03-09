using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("PlayerAudioSource")]
    [SerializeField] private AudioSource BGMAudioSource;
    [SerializeField] private AudioSource SFXAuidoSource;

    //[Header("ETCAudioSource")]
    //[SerializeField] private AudioSource[] _etcSFXAudioSources;

    private Dictionary<string, AudioClip> _bgm;
    private Dictionary<string, AudioClip> _sfx;
    private int _index;

    [Header("Controller")]
    //[SerializeField] private float _maxDistance = 50f;
    [Range(0f, 1f)] public float StartVolume = 0.1f;

    public static SoundManager Instance;

    void Awake()
    {
        Instance = this;

        Init();
    }


    public void Init()
    {
        // 초기 셋팅
        _bgm = new Dictionary<string, AudioClip>();
        _sfx = new Dictionary<string, AudioClip>();

        BGMAudioSource.loop = true;
        BGMAudioSource.volume = StartVolume;
        SFXAuidoSource.playOnAwake = false;
        SFXAuidoSource.volume = StartVolume;
        //for (int i = 0; i < _etcSFXAudioSources.Length; i++)
        //{
        //    _etcSFXAudioSources[i].playOnAwake = false;
        //    _etcSFXAudioSources[i].volume = StartVolume;
        //}

        // BGM

        // SFX
        _sfx.Add("Click", Resources.Load<AudioClip>("Sound/SFX/Click"));
        _sfx.Add("GameOver", Resources.Load<AudioClip>("Sound/SFX/GameOver"));
        _sfx.Add("Neon", Resources.Load<AudioClip>("Sound/SFX/Neon"));
        _sfx.Add("Neon2", Resources.Load<AudioClip>("Sound/SFX/Neon2"));
    }

    // 메모리 해제
    public void Release()
    {

    }

    // 다른 오브젝트에서 출력되는 사운드
    // 2D에서는 Vector2.Distance 사용
    //public void StartSFX(string name, Vector3 position)
    //{
    //    _index = _index % _etcSFXAudioSources.Length;

    //    float distance = Vector3.Distance(position, GameManager.I.PlayerManager.Player.transform.position);
    //    float volume = 1f - (distance / _maxDistance);
    //    _etcSFXAudioSources[_index].volume = Mathf.Clamp01(volume) * StartVolume;
    //    _etcSFXAudioSources[_index].PlayOneShot(_sfx[name]);

    //    _index++;
    //}

    // Player에서 출력되는 사운드
    public void StartSFX(string name)
    {
        SFXAuidoSource.PlayOneShot(_sfx[name]);
    }

    public void StartBGM(string name)
    {
        BGMAudioSource.Stop();
        BGMAudioSource.clip = _bgm[name];
        BGMAudioSource.Play();
    }

    public void StopBGM()
    {
        if (BGMAudioSource != null) BGMAudioSource.Stop();
    }
}