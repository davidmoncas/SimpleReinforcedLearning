using UnityEngine;

public class RobotAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private RobotStateController robotStateController;

    private static readonly int MovingAnimation = Animator.StringToHash("Moving");
    private static readonly int DyingAnimation = Animator.StringToHash("Dying");
    private static readonly int WinningAnimation = Animator.StringToHash("Winning");

    void Start()
    {
        robotStateController.OnStateChanged += OnStateChanged;
        UpdateAnimationState();
    }

    private void OnStateChanged()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        switch (robotStateController.State)
        {
            case RobotState.Moving:
                animator.SetBool(MovingAnimation, true);
                animator.SetBool(DyingAnimation, false);
                animator.SetBool(WinningAnimation, false);
                break;
            case RobotState.Winning:
                animator.SetBool(MovingAnimation, false);
                animator.SetBool(DyingAnimation, false);
                animator.SetBool(WinningAnimation, true);
                break;
            case RobotState.Dying:
                animator.SetBool(MovingAnimation, false);
                animator.SetBool(DyingAnimation, true);
                animator.SetBool(WinningAnimation, false);
                break;
        }
    }
}
