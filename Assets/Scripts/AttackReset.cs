using UnityEngine;

public class AttackReset : StateMachineBehaviour
{
    [SerializeField] string _triggerName;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(_triggerName);

        if (animator.TryGetComponent<PlayerCtrl>(out var player))
        {
            player.OnAttackFinished();
        }
    }

}
