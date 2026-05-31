using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject ballPrefab;
    public int poolSize = 10;

    readonly Queue<Ball> available = new Queue<Ball>();
    readonly List<Ball>  all       = new List<Ball>();  

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
            CreateBall();
    }

    Ball CreateBall()
    {
        GameObject go = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(transform); 
        Ball ball = go.GetComponentInChildren<Ball>();
        ball.pool = this;
        all.Add(ball);
        go.SetActive(false);
        available.Enqueue(ball);
        return ball;
    }

 
    public Ball GetBall(Vector3 position)
    {
        Ball ball = available.Count > 0 ? available.Dequeue() : CreateBall();
        ball.transform.position = position;
        ball.transform.rotation = Quaternion.identity;
        ball.gameObject.SetActive(true);
        return ball;
    }

   
    public void ReturnBall(Ball ball)
    {
        ball.gameObject.SetActive(false);
        available.Enqueue(ball);
    }

   
    public List<Ball> GetAllBalls() => all;
}
