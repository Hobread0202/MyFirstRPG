using UnityEngine;
using UnityEngine.UI;

public class EnemyHpUI : MonoBehaviour
{
    Transform _target; //타겟
    [SerializeField] Image _hpBar;  //hp바

    
    void Update()
    {
        //ui가활성화안됐거나 타겟없으면 리턴
        if (!gameObject.activeSelf || _target == null) return;
        {
            //show에서 타겟값 받으면 실행
            transform.LookAt(_target);
        }
    }

    public void Show(Transform lookTarget)
    {
        //플레이어 위치값받기
        _target = lookTarget;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void TakeDamage(int current, int max)
    {
                            //0~1 사이로 소수점형변환
        _hpBar.fillAmount = Mathf.Clamp01((float)current / max);
    }

    public void SetHp()
    {
        _hpBar.fillAmount = 1;
    }
}
