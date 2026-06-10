using System.Collections;
using UnityEngine;

public class MovingShooter : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Ball Pool")]
    public BallPool ballPool;

    [Header("Target Points")]
    public Transform[] targetPoints;

    [Header("Gun")]
    public Transform gun;
    public Transform shootPoint;

    [Header("Settings")]
    public float shootForce = 15f;
    public float gunRotateTime = 1f;
    public float scaleTime = 0.5f;
    public float waitBeforeShoot = 1f;
    public float waitAfterShoot = 1f;

    [Header("Reset Settings")]
    public int savesToReset = 10; // بعد 10 صدات يعيد من الأول
    public LevelManager levelManager;

    int lastWaypointIndex = -1;
    int lastTargetIndex = -1;
    int saveCount = 0;

    void OnEnable()
    {
        saveCount = 0; // reset العداد كل مرة يتشغل
        StartCoroutine(ShooterLoop());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void RegisterSave()
    {
        saveCount++;
        Debug.Log($"Moving Mode Save: {saveCount} / {savesToReset}");

        if (saveCount >= savesToReset)
        {
            saveCount = 0; // reset العداد ويكمل
            StartCoroutine(ResetRoutine());
        }
    }

    IEnumerator ResetRoutine()
    {
        // قول كلمة تشجيعية
        if (levelManager != null)
            levelManager.ShowRandomMessage();

        // استنى الـ message تخلص
        yield return new WaitForSeconds(levelManager != null ? levelManager.pauseDuration : 3f);

        // كمل من الأول
        Debug.Log("Moving Mode Reset!");
    }

    IEnumerator ShooterLoop()
    {
        transform.localScale = Vector3.zero;

        while (true)
        {
            // اختار waypoint جديد
            int wpIndex = GetRandomIndex(waypoints.Length, lastWaypointIndex);
            lastWaypointIndex = wpIndex;
            transform.position = waypoints[wpIndex].position;
            transform.rotation = waypoints[wpIndex].rotation;

            // ظهور بـ Scale
            yield return ScaleTo(Vector3.one);

            // استنى عشان اللاعب يشوفه
            yield return new WaitForSeconds(waitBeforeShoot);

            // اختار target
            int targetIndex = GetRandomIndex(targetPoints.Length, lastTargetIndex);
            lastTargetIndex = targetIndex;
            Vector3 target = targetPoints[targetIndex].position;

            // دور الـ Gun على الـ target
            yield return AimGun(target);

            // شوط
            FireBall(target);

            // استنى بعد الشوط
            yield return new WaitForSeconds(waitAfterShoot);

            // اختفاء بـ Scale
            yield return ScaleTo(Vector3.zero);

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ScaleTo(Vector3 targetScale)
    {
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < scaleTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / scaleTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    IEnumerator AimGun(Vector3 target)
    {
        if (gun == null) yield break;

        Quaternion startRot = gun.rotation;
        Quaternion endRot = Quaternion.LookRotation((target - gun.position).normalized, Vector3.up);

        float elapsed = 0f;
        while (elapsed < gunRotateTime)
        {
            elapsed += Time.deltaTime;
            gun.rotation = Quaternion.Slerp(startRot, endRot, elapsed / gunRotateTime);
            yield return null;
        }

        gun.rotation = endRot;
    }

    void FireBall(Vector3 target)
    {
        Ball ball = ballPool.GetBall(shootPoint.position);
        ball.LaunchToTarget(target, shootForce);
    }

    int GetRandomIndex(int length, int exclude)
    {
        if (length == 1) return 0;

        int index;
        do { index = Random.Range(0, length); }
        while (index == exclude);
        return index;
    }
}