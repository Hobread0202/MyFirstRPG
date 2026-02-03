using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirebaseAuthMgr : MonoBehaviour
{
    [SerializeField] GameObject _panel;

    [SerializeField] Text _loadingMassage;
    [SerializeField] Text _waringMassage;
    [SerializeField] Text _confirmMassage;

    
    [SerializeField] InputField _email;
    [SerializeField] InputField _password;



    public FirebaseAuth _auth;
    static public FirebaseUser _user;
    static public DatabaseReference _dbRef;
    int autoUserIndex = 1;

    private void Awake()
    {
        //버전체크                                           비동기작업중 확인 (WithOnMainThread 붙이면 플랫폼 바뀌어도 호환)
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(
            task =>

            //task에 CheckAndFixDependenciesAsync에서 체크된 결과 넘어옴
            {
                //가능하다는 결과를 받았다면
                if (task.Result == Firebase.DependencyStatus.Available)
                {
                    //인증정보 기억
                    _auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                    _dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                                        
                }
                else
                {
                    //실패하면 로그
                    UnityEngine.Debug.LogError(System.String.Format("버전체크에러" + task.Result));
                }

            }
        );
    }

    private void Start()
    {
        //메뉴 켜기
        _panel.gameObject.SetActive(true);

    }


    public void Login()
    {
        StartCoroutine(LoginCor(_email.text, _password.text));
    }

    IEnumerator LoginCor(string email, string password)
    {
        //로그인 작업을 LoginTask에 대입
        Task<AuthResult> LoginTask = _auth.SignInWithEmailAndPasswordAsync(email, password);
        //로그인 받아올때까지 수행안함       WaitUntil (값이 아닌 작업이 끝났는지)
        yield return new WaitUntil(predicate : () => LoginTask.IsCompleted);

        //문제가 생겼다면
        if (LoginTask.Exception != null)
        {
            Debug.Log($"로그인 실패 : {LoginTask.Exception}");

            //파이어베이스형식으로 에러 해석
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;

            //사용자 방식으로 형변환
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string messege = "";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    messege = "이메일을 입력해주세요";
                    break;
                case AuthError.InvalidEmail:
                    messege = "이메일을 입력해주세요";
                    break;
                case AuthError.WrongPassword:
                    messege = "비밀번호가 틀렸습니다";
                    break;
                case AuthError.MissingPassword:
                    messege = "비밀번호를 입력해주세요";
                    break;
                case AuthError.UserNotFound:
                    messege = "아이디가 존재하지 않습니다";
                    break;

                default:
                    messege = "오류발생";
                    break;
            }
            _waringMassage.text = messege;
        }

        //로그인 성공
        else
        {
            //유저 정보 기억
            _user = LoginTask.Result.User;

            //데이터 로드
            yield return StartCoroutine(FirebaseDBMgr.Instance.Init());

            _waringMassage.text = "";
            _confirmMassage.text = "데이터 로딩중...";

            //마지막씬 데이터 받아서
            string sceneName = FirebaseDBMgr.Instance.LoadedData.lastSceneName;

            //해당씬 불러오기
            SceneManager.LoadScene(sceneName);
        }
    }

    public void SignUp()
    {
        StartCoroutine(SignUpCor(_email.text, _password.text));
    }

    IEnumerator SignUpCor(string email, string password)
    {        
        Task<AuthResult> SignUpTask = _auth.CreateUserWithEmailAndPasswordAsync(email, password);        
        yield return new WaitUntil(predicate: () => SignUpTask.IsCompleted);

        if (SignUpTask.Exception != null)
        {
            Debug.Log($"로그인 실패 : {SignUpTask.Exception}");
            FirebaseException firebaseEx = SignUpTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string messege = "";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    messege = "이메일을 입력해주세요";
                    break;
                case AuthError.InvalidEmail:
                    messege = "이메일을 입력해주세요";
                    break;
                case AuthError.MissingPassword:
                    messege = "비밀번호를 입력해주세요";
                    break;
                case AuthError.EmailAlreadyInUse:
                    messege = "이미 존재합니다";
                    break;

                default:
                    messege = "오류발생";
                    break;
            }
            _waringMassage.text = messege;
        }

        else
        {
            _user = SignUpTask.Result.User;

            //한번더 가입됐는지 체크
            if (_user != null)
            {
                // 자동 닉네임 생성
                string autoNick = ($"테스터{autoUserIndex}");
                autoUserIndex++;

                UserProfile profile = new UserProfile { DisplayName = autoNick };


                Task profileTask = _user.UpdateUserProfileAsync(profile);
                // profileTask 작업완료까지 대기
                yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

                //문제가 있다면
                if (profileTask.Exception != null)
                {
                    Debug.Log($"닉네임설정실패 {profileTask.Exception}");
                    FirebaseException firebaseEx = profileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                    _waringMassage.text = "닉네임 설정 실패";
                }
                else
                {
                    _waringMassage.text = "";
                    _confirmMassage.text = $"회원가입 완료: {autoNick}";
                    //DB매니저 Init함수 호출
                    FirebaseDBMgr.Instance.Init();
                }
            }
        }
        
    }
}
