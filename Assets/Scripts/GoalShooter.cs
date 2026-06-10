using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GoalShooter : MonoBehaviour
{
    [System.Serializable]
    public class Shooter
    {
        public Transform shootPoint;
        //public PlayableDirector timeline;
        public Transform gun;
    }

    [System.Serializable]
    public class TargetPoint
    {
        public Transform point;
        [Min(1)]
        public int weight = 1;
    }

    [Header("Shooters")]
    [SerializeField] private Shooter[] shooters;

    [Header("Ball Pool")]
    [SerializeField] private BallPool ballPool;

    [Header("Target Points")]
    [SerializeField] private TargetPoint[] targetPoints;

    [Header("Settings")]
    [SerializeField] public float shootForce = 15f;
    [SerializeField] private float shootInterval = 3f;
    //[SerializeField] private float timelineDelay = 4f;
    [SerializeField] private float gunRotateTime = 1f;

    private float timer;
    private int lastShooterIndex = -1;
    private int lastTargetIndex = -1;
    private bool isShooting;
    private int currentTargetIndex = -1;

    //private void Start()
    //{
    //    foreach (Shooter s in shooters)
    //        s.timeline?.Stop();
    //}

    private void Update()
    {
        if (isShooting)
            return;

        timer += Time.deltaTime;

        if (timer < shootInterval)
            return;

        timer = 0f;
        ShootRandomShooter();
    }

    private void ShootRandomShooter()
    {
        if (shooters == null || shooters.Length == 0)
            return;

        if (ballPool == null)
            return;

        if (targetPoints == null || targetPoints.Length == 0)
            return;

        int shooterIndex = GetRandomShooterIndex();
        lastShooterIndex = shooterIndex;

        StartCoroutine(ShootRoutine(shooters[shooterIndex]));
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

    private int GetWeightedTargetIndex()
    {
        if (targetPoints.Length == 1)
            return 0;

        int totalWeight = 0;

        for (int i = 0; i < targetPoints.Length; i++)
        {
            if (i == lastTargetIndex)
                continue;

            totalWeight += Mathf.Max(1, targetPoints[i].weight);
        }

        int randomValue = Random.Range(0, totalWeight);

        int currentWeight = 0;

        for (int i = 0; i < targetPoints.Length; i++)
        {
            if (i == lastTargetIndex)
                continue;

            currentWeight += Mathf.Max(1, targetPoints[i].weight);

            if (randomValue < currentWeight)
            {
                lastTargetIndex = i;
                return i;
            }
        }

        return 0;
    }

    private IEnumerator ShootRoutine(Shooter shooter)
    {
        isShooting = true;

        currentTargetIndex = GetWeightedTargetIndex();
        Vector3 targetPosition = targetPoints[currentTargetIndex].point.position;

        //shooter.timeline?.Stop();
        //shooter.timeline?.Play();

        //yield return new WaitForSeconds(timelineDelay);

        yield return AimGun(shooter.gun, targetPosition);

        FireBall(shooter, targetPosition);

        isShooting = false;
    }

    private IEnumerator AimGun(Transform gun, Vector3 target)
    {
        if (gun == null)
            yield break;

        Quaternion startRot = gun.rotation;

        Quaternion endRot = Quaternion.LookRotation(
            (target - gun.position).normalized,
            Vector3.up
        );

        float elapsed = 0f;

        while (elapsed < gunRotateTime)
        {
            elapsed += Time.deltaTime;

            gun.rotation = Quaternion.Slerp(
                startRot,
                endRot,
                elapsed / gunRotateTime
            );

            yield return null;
        }

        gun.rotation = endRot;
    }

    private void FireBall(Shooter shooter, Vector3 target)
    {
        Ball ball = ballPool.GetBall(shooter.shootPoint.position);

        Vector3 direction = (target - ball.transform.position).normalized;

        ball.LaunchToTarget(target,shootForce);
    }
    //private void OnDrawGizmos()
    //{
    //    if (targetPoints == null ||
    //        currentTargetIndex < 0 ||
    //        currentTargetIndex >= targetPoints.Length)
    //        return;

    //    if (targetPoints[currentTargetIndex] == null ||
    //        targetPoints[currentTargetIndex].point == null)
    //        return;

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(
    //        targetPoints[currentTargetIndex].point.position,
    //        0.3f
    //    );
    //}
}