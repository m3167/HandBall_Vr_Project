using UnityEngine;
using TMPro;

public class GoalDetector : MonoBehaviour
{
    [Header("References")]
    public LevelManager levelManager;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("Sounds")]
    public AudioClip goalClip; 

    AudioSource audioSource;
    int score = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            score++;
            UpdateScoreUI();

            if (goalClip != null)
                audioSource.PlayOneShot(goalClip);

           
            Ball ball = other.GetComponent<Ball>();
            if (ball != null)
                ball.ReturnToPool();

            Debug.Log("Goal! Score: " + score);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public int GetScore() => score;
}