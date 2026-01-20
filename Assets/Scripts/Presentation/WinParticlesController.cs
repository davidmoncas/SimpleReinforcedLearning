using UnityEngine;

public class WinParticlesController : MonoBehaviour
{
    [SerializeField] private ParticleSystem winParticles;
    [SerializeField] private RobotStateController robotStateController;

    void OnEnable()
    {
        robotStateController.OnStateChanged += OnStateChanged;
        UpdateParticleState();
    }

    void OnDisable()
    {
        robotStateController.OnStateChanged -= OnStateChanged;
    }
    private void OnStateChanged()
    {
        UpdateParticleState();
    }
    
    private void UpdateParticleState()
    {
        if (robotStateController.State == RobotState.Winning)
        {
            winParticles.Play();
        }
        else
        {
            winParticles.Stop();
        }
    }
}
