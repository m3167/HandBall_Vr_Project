using UnityEngine;
using TMPro;

public class SaveDetector : MonoBehaviour
{
    [Header("References")]
    public LevelManager levelManager;
    public MovingShooter movingShooter; // اسحب المدفع المتحرك هنا

    [Header("UI")]
    public TextMeshProUGUI saveText;

    [Header("Sounds")]
    public AudioClip saveClip;

    AudioSource audioSource;
    static int saves = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        saves = 0;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Ball ball = other.gameObject.GetComponent<Ball>();
            if (ball == null || ball.isSaved) return;

            ball.isSaved = true;
            saves++;
            UpdateSaveUI();

            if (saveClip != null)
                audioSource.PlayOneShot(saveClip);

            // لو Moving Mode شغال → ابعت للـ MovingShooter بس
            if (movingShooter != null && movingShooter.enabled)
                movingShooter.RegisterSave();
            // لو Static Mode → ابعت للـ LevelManager بس
            else if (levelManager != null)
                levelManager.RegisterSave();
        }
    }

    void UpdateSaveUI()
    {
        if (saveText != null)
            saveText.text = "Saves: " + saves;
    }

    public static int GetSaves() => saves;
}