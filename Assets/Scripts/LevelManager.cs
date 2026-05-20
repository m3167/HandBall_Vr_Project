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

    [System.Serializable]
    public class MessageEntry
    {
        public string text;   // مثلاً "Excellent!"
        public AudioClip clip;   // الصوت بتاعها
    }

    // ── UI ──────────────────────────────────────────────────────────────
    Canvas uiCanvas;
    TextMeshProUGUI messageText;

    // ── State ────────────────────────────────────────────────────────────
    int currentLevel = 1;
    int saveCount = 0;
    bool isTransitioning = false;

    // ────────────────────────────────────────────────────────────────────
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

        // صوت الليفل أب العام
        if (audioSource != null && levelUpClip != null)
            audioSource.PlayOneShot(levelUpClip);

        // اختار message عشوائية
        if (messages != null && messages.Length > 0)
        {
            int index = Random.Range(0, messages.Length);
            MessageEntry entry = messages[index];

            // شغل الصوت بتاع الرسالة دي
            if (audioSource != null && entry.clip != null)
                audioSource.PlayOneShot(entry.clip);

            // وريها على الشاشة
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
        GameObject canvasGO = new GameObject("LevelUpCanvas");
        uiCanvas = canvasGO.AddComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCanvas.sortingOrder = 10;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasGO.transform, false);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.5f);
        RectTransform pr = panel.GetComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        pr.offsetMin = Vector2.zero;
        pr.offsetMax = Vector2.zero;

        GameObject textGO = new GameObject("MessageText");
        textGO.transform.SetParent(panel.transform, false);
        messageText = textGO.AddComponent<TextMeshProUGUI>();
        messageText.fontSize = 72;
        messageText.fontStyle = FontStyles.Bold;
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.color = Color.yellow;
        RectTransform tr = textGO.GetComponent<RectTransform>();
        tr.anchorMin = new Vector2(0.1f, 0.3f);
        tr.anchorMax = new Vector2(0.9f, 0.7f);
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;

        canvasGO.SetActive(false);
    }

    void ShowMessage(string msg)
    {
        messageText.text = msg;
        uiCanvas.gameObject.SetActive(true);
        StartCoroutine(AnimateText());
    }

    void HideMessage()
    {
        StopCoroutine(AnimateText());
        uiCanvas.gameObject.SetActive(false);
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
            yield return null;
        }
    }
}