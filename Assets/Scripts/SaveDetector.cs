using UnityEngine;
using TMPro;

public class SaveDetector : MonoBehaviour
{
    [Header("References")]
    public LevelManager levelManager;

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