using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Shooters")]
    public GameObject shooterL;        // model all L
    public GameObject shooterC;        // model all C
    public GameObject shooterR;        // model all R
    public GoalShooter goalShooter;

    [Header("Moving Shooter")]
    public GameObject movingShooterObject;
    public MovingShooter movingShooter;

    [Header("Level Manager")]
    public LevelManager levelManager;

    void Start()
    {
        // خبي كل المدافع في الأول
        HideAllShooters();
    }

    // لما اللاعب يدوس Static
    public void StartStatic()
    {
        HideAllShooters();

        if (shooterL != null) shooterL.SetActive(true);
        if (shooterC != null) shooterC.SetActive(true);
        if (shooterR != null) shooterR.SetActive(true);

        if (levelManager != null) levelManager.StartGame();
        if (movingShooter != null) movingShooter.enabled = false;
    }

    // لما اللاعب يدوس Random
    public void StartRandom()
    {
        HideAllShooters();

        if (movingShooterObject != null)
        {
            movingShooterObject.SetActive(true);
            movingShooterObject.transform.localScale = Vector3.zero;
        }

        if (goalShooter != null) goalShooter.enabled = false;
        if (movingShooter != null) movingShooter.enabled = true;
    }

    void HideAllShooters()
    {
        if (shooterL != null) shooterL.SetActive(false);
        if (shooterC != null) shooterC.SetActive(false);
        if (shooterR != null) shooterR.SetActive(false);
        if (movingShooterObject != null) movingShooterObject.SetActive(false);

        if (goalShooter != null) goalShooter.enabled = false;
        if (movingShooter != null) movingShooter.enabled = false;
    }
}