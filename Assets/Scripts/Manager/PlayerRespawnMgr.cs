using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameRespawnManager : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] DeathEffect deathEffect;

    void OnEnable()
    {
        Debug.Log("Subscribe to DeathEffect.OnFadeComplete");
        deathEffect.OnFadeComplete += FadeComplete;
    }

    void OnDisable()
    {
        deathEffect.OnFadeComplete -= FadeComplete;
    }

    void FadeComplete()
    {
        StartCoroutine(RespawnPlayer());
    }

    IEnumerator RespawnPlayer()
    {
        //데이터 로드
        yield return StartCoroutine(FirebaseDBMgr.Instance.Init());

        SaveData data = FirebaseDBMgr.Instance.LoadedData;

        string targetScene = data.lastSceneName;
        Vector3 lastPos = data.lastPos;


        //해당씬이 아니라면
        if (SceneManager.GetActiveScene().name != targetScene)
        {
            //씬 불러오기 끝날때까지 기다림
            AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
            yield return new WaitUntil(() => op.isDone);
            
        }

        //플레이어 없다면 플레이어 찾기
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        //한프레임 기다리기
        yield return null;


        //플레이어 위치 복원
        player.position = lastPos;

        PlayerCtrl playerCtrl = player.GetComponent<PlayerCtrl>();
        if (playerCtrl != null)
        {
            //최대체력으로 리셋
            playerCtrl.ResetPlayer(playerCtrl.PlayerStats.maxHp);
        }

        //페이드 초기화
        deathEffect.ResetFade();
    }
}
