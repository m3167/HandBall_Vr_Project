using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Levels")]
    public int savesPerLevel = 5;
    public int maxLevel = 3;

    [Header("Shoot Force Per Level")]
    public float[] shootForcePerLevel;

    [Header("Pause Between Levels (seconds)")]
    public float pauseDuration = 3f;

    [Header("Level Up Sound")]
    public AudioSource audioSource;
    public AudioClip levelUpClip;

    [Header("Messages & Sounds")]
    public MessageEntry[] messages;

    [Header("References")]
    public GoalShooter goalShooter;
    public Transform vrCamera;

    [Header("UI Settings")]
    public float uiDistance = 2f;
    public float uiWidth = 1.5f;
    public float uiHeight = 1f;

    [System.Serializable]
    public class MessageEntry
    {
        public string text;
        public AudioClip clip;
    }

    Canvas uiCanvas;
    TextMeshProUGUI messageText;
    GameObject canvasGO;

    int currentLevel = 1;
    int saveCount = 0;
    bool isTransitioning = false;

    void Awake()
    {
        CreateUI();
        ApplyLevelSettings();
    }

    public void RegisterSave()
    {
        if (isTransitioning) return;

        saveCount++;
        Debug.Log($"Save {saveCount} / {savesPerLevel}  |  Level {currentLevel}");

        if (saveCount >= savesPerLevel)
        {
            saveCount = 0;
            if (currentLevel < maxLevel)
                StartCoroutine(LevelUpRoutine());
            else
                Debug.Log("اللاعب خلص كل المراحل!");
        }
    }

    IEnumerator LevelUpRoutine()
    {
        isTransitioning = true;
        goalShooter.enabled = false;

        if (audioSource != null && levelUpClip != null)
            audioSource.PlayOneShot(levelUpClip);

        if (messages != null && messages.Length > 0)
        {
            int index = Random.Range(0, messages.Length);
            MessageEntry entry = messages[index];

            if (audioSource != null && entry.clip != null)
                audioSource.PlayOneShot(entry.clip);

            ShowMessage(entry.text + "\nLevel " + (currentLevel + 1) + "!");
        }

        yield return new WaitForSeconds(pauseDuration);

        HideMessage();

        currentLevel++;
        ApplyLevelSettings();

        goalShooter.enabled = true;
        isTransitioning = false;
    }

    void ApplyLevelSettings()
    {
        int index = Mathf.Clamp(currentLevel - 1, 0, shootForcePerLevel.Length - 1);
        goalShooter.shootForce = shootForcePerLevel[index];
        Debug.Log($"Level {currentLevel} | Force: {goalShooter.shootForce}");
    }

    void CreateUI()
    {
        canvasGO = new GameObject("LevelUpCanvas");

        uiCanvas = canvasGO.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.WorldSpace;
        uiCanvas.sortingOrder = 10;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;

        canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(uiWidth * 100, uiHeight * 100);
        canvasRect.localScale = Vector3.one * 0.01f;

        // Text مباشرة بدون Panel أو خلفية
        GameObject textGO = new GameObject("MessageText");
        textGO.transform.SetParent(canvasGO.transform, false);
        messageText = textGO.AddComponent<TextMeshProUGUI>();
        messageText.fontSize = 18;
        messageText.fontStyle = FontStyles.Bold;
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.color = Color.yellow;

        RectTransform tr = textGO.GetComponent<RectTransform>();
        tr.anchorMin = new Vector2(0.1f, 0.2f);
        tr.anchorMax = new Vector2(0.9f, 0.8f);
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;

        canvasGO.SetActive(false);
    }

    void ShowMessage(string msg)
    {
        if (vrCamera != null)
        {
            canvasGO.transform.position = vrCamera.position + vrCamera.forward * uiDistance;
            canvasGO.transform.rotation = Quaternion.LookRotation(
                canvasGO.transform.position - vrCamera.position);
        }

        messageText.text = msg;
        canvasGO.SetActive(true);
        StartCoroutine(AnimateText());
    }

    void HideMessage()
    {
        canvasGO.SetActive(false);
    }

    IEnumerator AnimateText()
    {
        float elapsed = 0f;
        Vector3 baseScale = Vector3.one;

        while (elapsed < pauseDuration)
        {
            elapsed += Time.deltaTime;
            float scale = 1f + 0.1f * Mathf.Sin(elapsed * 6f);
            messageText.transform.localScale = baseScale * scale;

            if (vrCamera != null)
                canvasGO.transform.rotation = Quaternion.LookRotation(
                    canvasGO.transform.position - vrCamera.position);

            yield return null;
        }
    }
}