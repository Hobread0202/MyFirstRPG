using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] PlayerCtrl _player;
    public RawImage deathImage;
    public float fadeDuration = 1.5f;

    public event Action OnFadeComplete;
        

    private void Start()
    {
        //컬러 셋팅
        Color c = deathImage.color;
        c.a = 0f;
        deathImage.color = c;
    }

    private void OnEnable()
    {
        _player.OnDead += Play;        
    }

    private void OnDisable()
    {
        _player.OnDead -= Play;
    }

    void Play()
    {
        //페이드인하고 끝나면 알림보내는 함수 호출
        deathImage.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad).SetDelay(3f).OnComplete(FadeComplete);
    }

    void FadeComplete()
    {
        Debug.Log("Fade Complete!");
        OnFadeComplete?.Invoke();
    }

    public void ResetFade()
    {
        Color c = deathImage.color;
        c.a = 0f;
        deathImage.color = c;
    }
}
