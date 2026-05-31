using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class GoalShooter : MonoBehaviour
{
    [System.Serializable]
    public class Shooter
    {
        public Transform shootPoint;
        public PlayableDirector timeline;
        public Transform gun;
    }

    [Header("Shooters")]
    [SerializeField] private Shooter[] shooters;

    [Header("Ball Pool")]
    [SerializeField] private BallPool ballPool;

    [Header("Goal Area")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] private float targetZ;

    [Header("Settings")]
    [SerializeField] public float shootForce = 15f;
    [SerializeField] private float shootInterval = 3f;
    [SerializeField] private float timelineDelay = 4f;
    [SerializeField] private float gunRotateTime = 1f;

    private float timer;
    private int lastShooterIndex = -1;
    private bool isShooting;

    private void Start()
    {
        StopAllTimelines();
    }

    private void Update()
    {
        if (isShooting) return;

        timer += Time.deltaTime;
        if (timer < shootInterval) return;

        timer = 0f;
        ShootRandomShooter();
    }

    private void StopAllTimelines()
    {
        foreach (Shooter shooter in shooters)
        {
            if (shooter.timeline != null)
                shooter.timeline.Stop();
        }
    }

    private void ShootRandomShooter()
    {
        if (shooters == null || shooters.Length == 0)
        {
            Debug.LogWarning($"{nameof(GoalShooter)}: No shooters assigned.");
            return;
        }

        if (ballPool == null)
        {
            Debug.LogWarning($"{nameof(GoalShooter)}: No ball pool assigned.");
            return;
        }

        int index = GetRandomShooterIndex();
        lastShooterIndex = index;
        StartCoroutine(ShootRoutine(shooters[index]));
    }

    private int GetRandomShooterIndex()
    {
        if (shooters.Length == 1)
            return 0;

        int index;
        do
        {
            index = Random.Range(0, shooters.Length);
        }
        while (index == lastShooterIndex);

        return index;
    }

    private Vector3 GetRandomTarget()
    {
        return new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            targetZ);
    }

    private IEnumerator ShootRoutine(Shooter shooter)
    {
        isShooting = true;

        Vector3 target = GetRandomTarget();

        PlayTimeline(shooter);
        yield return new WaitForSeconds(timelineDelay);

        yield return AimGun(shooter.gun, target);

        FireBall(shooter, target);

        isShooting = false;
    }

    private void PlayTimeline(Shooter shooter)
    {
        if (shooter.timeline == null) return;

        shooter.timeline.Stop();
        shooter.timeline.Play();
    }

    private IEnumerator AimGun(Transform gun, Vector3 target)
    {
        if (gun == null) yield break;

        Quaternion startRotation = gun.rotation;
        Vector3 direction = (target - gun.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        float elapsed = 0f;
        while (elapsed < gunRotateTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / gunRotateTime;
            gun.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        gun.rotation = targetRotation;
    }

    private void FireBall(Shooter shooter, Vector3 target)
    {
        Ball ball = ballPool.GetBall(shooter.shootPoint.position);
        Vector3 direction = (target - ball.transform.position).normalized;
        ball.Launch(direction, shootForce);
    }
}