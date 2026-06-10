using System;
using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector] public BallPool pool;
    [HideInInspector] public bool isSaved = false;

    [Header("Launch Settings")]
    public float launchDelay = 1f;

    [Header("Hit Settings")]
    [Tooltip("قوة الضرب - كل ما زاد زادت سرعة الكورة")]
    public float hitForce = 20f;

    [Header("Gravity")]
    [Range(0f, 1f)]
    [Tooltip("0 = بدون جاذبية (خط مستقيم) , 1 = جاذبية كاملة")]
    public float gravityScale = 0.3f;

    [Header("Sounds")]
    public AudioClip spawnClip;
    public AudioClip shootClip;

    AudioSource audioSource;
    Rigidbody rb;

    // الكورة بتتأثر بالجاذبية بس لما تكون متحركة (بعد الضرب)
    bool isLaunched = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // نوقف الجاذبية الافتراضية ونطبق واحدة مخصصة بنتحكم فيها
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        // نطبق جاذبية مخصصة اخف على الكورة بعد ما تتضرب
        if (isLaunched && !rb.isKinematic)
        {
            rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
        }
    }

    // target = النقطة اللي الكورة هتتضرب ناحيتها
    public void LaunchToTarget(Vector3 target, float Force)
    {
        hitForce = Force;
        UseGravity (false);
        StartCoroutine(HitRoutine(target));
    }

    private IEnumerator HitRoutine(Vector3 target)
    {
        isLaunched = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        if (spawnClip != null)
            audioSource.PlayOneShot(spawnClip);

        yield return new WaitForSeconds(launchDelay);

        rb.isKinematic = false;

        // اتجاه الضرب = من الكورة للنقطة (خط مستقيم)
        Vector3 direction = (target - transform.position).normalized;

        if (shootClip != null)
            audioSource.PlayOneShot(shootClip);

        // ضربة في خط مستقيم بقوة
        rb.linearVelocity = direction * hitForce;

        // دلوقتي تبدأ الجاذبية المخصصة تشتغل
        isLaunched = true;
    }

    public void ReturnToPool()
    {
        StopAllCoroutines();
        isLaunched = false;
        isSaved = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        pool.ReturnBall(this);
    }

    internal void UseGravity(bool v)
    {
        rb.useGravity = v;
    }
}