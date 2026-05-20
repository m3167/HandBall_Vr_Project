using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector] public BallPool pool;
    [HideInInspector] public bool isSaved = false;

    [Header("Launch Settings")]
    public float launchDelay = 1f;

    [Header("Sounds")]
    public AudioClip spawnClip;  
    public AudioClip shootClip;  

    AudioSource audioSource;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

     
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Launch(Vector3 direction, float force)
    {
        isSaved = false;
        StartCoroutine(LaunchAfterDelay(direction, force));
    }

    IEnumerator LaunchAfterDelay(Vector3 direction, float force)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

       
        if (spawnClip != null)
            audioSource.PlayOneShot(spawnClip);

        yield return new WaitForSeconds(launchDelay);

        rb.isKinematic = false;

       
        if (shootClip != null)
            audioSource.PlayOneShot(shootClip);

        rb.linearVelocity = direction * force;
    }

    public void ReturnToPool()
    {
        StopAllCoroutines();
        isSaved = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        pool.ReturnBall(this);
    }
}