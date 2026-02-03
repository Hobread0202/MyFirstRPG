using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseDBMgr : MonoBehaviour
{
    //싱글톤
    public static FirebaseDBMgr Instance;

    DatabaseReference _dbRef;
    FirebaseUser _user;

    public SaveData LoadedData { get; private set; }

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

    //로그인 회원가입 데이터갱신
    public IEnumerator Init()
    {
        //기본셋팅
        _dbRef = FirebaseAuthMgr._dbRef;
        _user = FirebaseAuthMgr._user;
        LoadedData = null;

        //데이터불러오기
        var task = _dbRef.Child("users").Child(_user.UserId).GetValueAsync();
        //작업완료 기다리기
        yield return new WaitUntil(predicate : () => task.IsCompleted);

        //문제가있다면
        if (task.Exception != null)
        {
            Debug.LogError("로드 실패");
        }
        //문제없다면 유저데이터 불러오기
        else if (task.Result.Exists)
        {
            string json = task.Result.GetRawJsonValue();
            LoadedData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("데이터 초기화 완료");
        }
        //데이터가 없다면
        else
        {            
            LoadedData = new SaveData();
            Debug.Log("기본값 셋팅");
        }
    }

    public void Save(SaveData data, Action onComplete = null)
    {
        StartCoroutine(SaveCor(data, onComplete));
    }

    IEnumerator SaveCor(SaveData data, Action onComplete)
    {
        //json으로 data 전달
        string json = JsonUtility.ToJson(data);

        //데이터베이스에 저장
        var Task = _dbRef.Child("users").Child(_user.UserId).SetRawJsonValueAsync(json);

        //작업완료까지 기다림
        yield return new WaitUntil(predicate: () => Task.IsCompleted);

        //문제가 있다면
        if (Task.Exception != null)
        {
            Debug.LogWarning($"업데이트 실패 : {Task.Exception}");
        }
        else
        {
            //저장완료후 실행할거
            //알림전달
            onComplete?.Invoke();
        }
    }
}
