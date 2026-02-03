using UnityEngine;

public class CursorMgr : MonoBehaviour
{
    //싱글톤
    public static CursorMgr Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //마우스커서 중앙고정
        Cursor.lockState = CursorLockMode.Locked;
        //마우스커서 비활성화
        Cursor.visible = false;
    }

    //ESC UI커넥터 연결함수
    public void ClickEsc()
    {
        //고정되어 있었다 풀기
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        //아니면 반대로 걸기
        else
        {            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
