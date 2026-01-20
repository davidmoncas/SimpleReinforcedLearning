using UnityEngine;

public class RobotEyePresenter : MonoBehaviour
{
    private static string BlinkingAnimation = "Blinking";
    private static string DyingAnimation = "Dying";

    [SerializeField] private Transform leftEye, rightEye;
    [SerializeField] private float radius;
    [SerializeField] private Transform goal;
    [SerializeField] private RobotStateController robotState;
    [SerializeField] private Animator anim;

    private Vector2 initialPosRight, initialPosLeft;

    private void Start()
    {
        initialPosLeft = leftEye.transform.localPosition;
        initialPosRight = rightEye.transform.localPosition;
        robotState.OnStateChanged += OnStateChanged;
    }

    public void OnStateChanged()
    {
        leftEye.gameObject.SetActive(robotState.State != RobotState.Dying);
        rightEye.gameObject.SetActive(robotState.State != RobotState.Dying);
        switch (robotState.State)
        {
            case RobotState.Moving:
                anim.SetBool(BlinkingAnimation, true);
                anim.SetBool(DyingAnimation, false);
                break;
            case RobotState.Winning:
                anim.SetBool(BlinkingAnimation, true);
                anim.SetBool(DyingAnimation, false);
                break;
            case RobotState.Dying:
                anim.SetBool(BlinkingAnimation, false);
                anim.SetBool(DyingAnimation, true);
                break;
        }
    }

    void OnDisable()
    {
        robotState.OnStateChanged -= OnStateChanged;
    }

    void Update()
    {
        rightEye.transform.localPosition = initialPosRight + ((Vector2)goal.position - (Vector2)transform.position).normalized * radius;
        leftEye.transform.localPosition = initialPosLeft + ((Vector2)goal.position - (Vector2)transform.position).normalized * radius;
    }
}
