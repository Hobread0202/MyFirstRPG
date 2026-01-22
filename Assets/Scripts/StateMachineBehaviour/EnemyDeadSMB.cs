using UnityEngine;

public class EnemyDeadSMB : StateMachineBehaviour
{
    [SerializeField] string _triggerName;
    bool _isFinished; //모션진행중체크

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //처음!_isFinished는 만족시켜야하니까 상태진입시 false
        _isFinished = false;    
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_isFinished == false && stateInfo.normalizedTime >= 0.95f) //95퍼 완료 시
        {
            _isFinished = true;

            if (animator.TryGetComponent<EnemyCtrl>(out var enemy))
            {
                enemy.OnDeadFinished();
                animator.ResetTrigger(_triggerName);
            }
        }
    }
}
