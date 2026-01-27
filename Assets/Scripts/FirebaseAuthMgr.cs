using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirebaseAuthMgr : MonoBehaviour
{
    [SerializeField] Button _loginButton;
    [SerializeField] Button _signUpButton;
    [SerializeField] InputField _email;
    [SerializeField] InputField _password;
    [SerializeField] Text _loadingMassage;

    public FirebaseAuth _auth;
    public FirebaseUser _user;

    private void Start()
    {
        //버전체크                                           비동기작업중 확인 (WithOnMainThread 붙이면 플랫폼 바뀌어도 호환)
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(
            task =>

            //비동기 작업 결과를 기억
            {
                var dependencyStatus = task.Result;

                //가능하다는 결과를 받았다면
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    //인증정보 기억
                    _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                    //메뉴 셋팅
                    SetingMenu();
                }
                else
                {
                    //실패하면 로그
                    UnityEngine.Debug.LogError(System.String.Format("버전체크에러" + dependencyStatus));
                }

            }
        );
    }

    public void SetingMenu() //메뉴셋팅
    {
        _loadingMassage.enabled = false;
        _loginButton.gameObject.SetActive(true);
        _signUpButton.gameObject.SetActive(true);
        _email.gameObject.SetActive(true);
        _password.gameObject.SetActive(true);
    }

    public void Login()
    {
        //이메일 패스워드 로그인 기능
        _auth.SignInWithEmailAndPasswordAsync(_email.text, _password.text).ContinueWithOnMainThread(
            task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("로그인 오류");
                    return;
                }

                if (task.IsCanceled)
                {
                    Debug.Log("로그인 취고");
                    return;
                }

                //다 통과하면 로그인완료된 정보 넣어주기
                _user = task.Result.User;
                SceneManager.LoadScene("Stage1");

            }
        );
    }

    public void SignUp()
    {
        //회원가입기능
        _auth.CreateUserWithEmailAndPasswordAsync(_email.text, _password.text).ContinueWithOnMainThread(
            task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("회원가입 오류");
                    return;
                }

                if (task.IsCanceled)
                {
                    Debug.Log("회원가입 취고");
                    return;
                }

                //다 통과하면 로그인완료된 정보 넣어주기
                _user = task.Result.User;

            }
        );
    }
}
