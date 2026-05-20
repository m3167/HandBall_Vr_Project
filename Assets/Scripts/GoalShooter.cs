using UnityEngine;

public class GoalShooter : MonoBehaviour
{
    public enum ShooterType { Left, Middle, Right }

    [System.Serializable]
    public class Shooter
    {
        public ShooterType type;
        public Transform shootPoint;
    }

    [Header("Shooters")]
    public Shooter[] shooters;

    [Header("Ball Pool")]
    public BallPool ballPool;

    [Header("Goal Area")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float targetZ;

    [Header("Settings")]
    public float shootForce = 15f;
    public float shootEvery = 3f;

    float timer;
    int lastShooterIndex = -1;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= shootEvery)
        {
            timer = 0;
            ShootRandom();
        }
    }

    void ShootRandom()
    {
        
        if (shooters == null || shooters.Length == 0)
        {
            Debug.LogWarning("GoalShooter: مفيش Shooters!");
            return;
        }
        if (ballPool == null)
        {
            Debug.LogWarning("GoalShooter: مفيش BallPool!");
            return;
        }

      
        int randomIndex;
        if (shooters.Length == 1)
            randomIndex = 0;
        else
            do { randomIndex = Random.Range(0, shooters.Length); }
            while (randomIndex == lastShooterIndex);

        lastShooterIndex = randomIndex;
        Shooter shooter = shooters[randomIndex];

       
        Ball ball = ballPool.GetBall(shooter.shootPoint.position);

        Vector3 target = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            targetZ);

        Vector3 dir = (target - ball.transform.position).normalized;
        ball.Launch(dir, shootForce);

        Debug.Log("Shooter: " + shooter.type);
    }
}