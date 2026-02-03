using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputFiledTab : MonoBehaviour
{
    [SerializeField] InputField[] _inputField;
    [SerializeField] Button _enterLogin;
    
    public void OnTab(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //활성화된 UI
            GameObject current = EventSystem.current.currentSelectedGameObject;

            //인풋필드만 돌면서 검사
            for (int i = 0; i < _inputField.Length; i++)
            {
                //활성화된 인풋필드 찾으면
                if (_inputField[i].gameObject == current)
                {
                    //배열수로 나누어떨어지면 처음으로 돌아감
                    int nextIndex = (i + 1) % _inputField.Length;

                    //다음필드선택
                    _inputField[nextIndex].Select();
                    break;
                }
            }
        }
    }

    public void OnEnter(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //로그인 버튼 실행
            _enterLogin.onClick.Invoke();
        }
    }
}
