using Nightmare;
using UnityEngine;

public class FaerieStateBehaviour : StateMachineBehaviour {
    internal FaerieCircle faerieCircle;
    internal float angerTimer;
    public int nextState;
    internal FaerieAnger faerieAnger;

    public void Setup(FaerieCircle fc, FaerieAnger fa) {
        faerieCircle = fc;
        faerieAnger = fa;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        ProcessAnger(animator);
    }

    private void ProcessAnger(Animator animator) {
        if (angerTimer <= 0f) return;
        
        angerTimer -= Time.deltaTime;
        if (angerTimer <= 0f) {
            animator.SetInteger(AnimationConstants.AngerAttribute, nextState);
        }
    }
}