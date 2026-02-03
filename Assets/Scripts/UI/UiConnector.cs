using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UiConnector : MonoBehaviour
{
    [SerializeField] PlayerCtrl _player;
    [SerializeField] GameObject _panel;
    [SerializeField] CanvasGroup _panelCanvasGroup;
    [SerializeField] Image _hpBar;
    [SerializeField] Image _expBar;
    [SerializeField] Text _level;
    [SerializeField] Button _resumeButton;
    [SerializeField] Button _settingButton;
    [SerializeField] Button _saveButton;

    private void Start()
    {
        //초기 셋팅
        UpdateHpBar(_player.GetCurrentSaveData().currentHp, _player.PlayerStats.maxHp);
        UpdateExpBar(_player.GetCurrentSaveData().currentExp, _player.PlayerStats.maxExp);
        UpdateLevelText(_player.GetCurrentSaveData().currentLevel);
    }

    private void OnEnable()
    {
        _player.OnHpChanged += UpdateHpBar;
        _player.OnExpChanged += UpdateExpBar;
        _player.OnLevelChanged += UpdateLevelText;
    }

    private void OnDisable()
    {
        _player.OnHpChanged -= UpdateHpBar;
        _player.OnExpChanged -= UpdateExpBar;
        _player.OnLevelChanged -= UpdateLevelText;
    }

    public void OnClickSaveButton()
    {
        if (_player != null)
        {
            //다시 못누르게 꺼두기
            _saveButton.interactable = false;

            //현재 플레이어 데이터를 받아옴
            SaveData data = _player.GetCurrentSaveData();

            //DB 매니저에게 데이터와 작업종료후 할 행동 전달
            FirebaseDBMgr.Instance.Save(data, () => 
            {
                Application.Quit();
            });
        }
    }

    public void OnClickResume()
    {
        //패널끄고
        _panel.SetActive(false);
        //레이케스트끄기
        _panelCanvasGroup.blocksRaycasts = false;
        //커서복구
        CursorMgr.Instance.ClickEsc();
    }

    public void OnClickEsc(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //반대 상태 저장
            bool newState = !_panel.activeSelf;
            //현재상태의 반대로 두기
            _panel.SetActive(!_panel.activeSelf);
            //패널레이케스트 차단
            _panelCanvasGroup.blocksRaycasts = newState;
            //커서조정
            CursorMgr.Instance.ClickEsc();
        }
    }

    public void UpdateHpBar(float currentHp, float maxHp)
    {
        //0으로 나누기 방지
        if (maxHp <= 0) return;
        _hpBar.fillAmount = currentHp / maxHp;
    }

    public void UpdateExpBar(float currentExp, float maxExp)
    {
        //0으로 나누기 방지
        if (maxExp <= 0) return;
        _expBar.fillAmount = currentExp / maxExp;
    }

    public void UpdateLevelText(int newLevel)
    {
        _level.text = $"Lv. {newLevel}";
    }
}
