using UnityEngine;
using System.Collections.Generic;

public class SoundMgr : MonoBehaviour
{
    public static SoundMgr Instance { get; private set; }

    [SerializeField] private SoundClipData _soundData;
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    private Dictionary<string, AudioClip> _clipDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //초기 셋팅
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        _clipDictionary = new Dictionary<string, AudioClip>(_soundData.audioClips.Count);

        //클립 다 넣기
        foreach (var clip in _soundData.audioClips)
        {
            if (clip != null && !_clipDictionary.ContainsKey(clip.name))
            {
                _clipDictionary.Add(clip.name, clip);
            }
        }

        //브금은 반복
        _bgmSource.loop = true;
        PlayBGM("BGM");
    }

    public void PlaySFX(string clipName)
    {
        if (_clipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            _sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayBGM(string clipName)
    {
        if (_clipDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            _bgmSource.clip = clip;
            _bgmSource.Play();
        }
    }
}