using UnityEngine;

public class FaerieAngry : FaerieStateBehaviour {
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        faerieCircle.SetMood(true);
        angerTimer = faerieCircle.angryFaerie.minimumTime;
        if (faerieAnger != null)
            faerieAnger.StartEffect(angerTimer);
    }
}