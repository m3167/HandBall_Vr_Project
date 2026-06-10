using UnityEngine;
using System.Collections;
public class SystemPause : MonoBehaviour
{

    [Header("References")]
    public GoalShooter goalShooter;

    [Header("Settings")]
    public float delayBeforeResume = 3f;

    Coroutine resumeCoroutine;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
         
            if (resumeCoroutine != null)
                StopCoroutine(resumeCoroutine);

            goalShooter.enabled = false;
            goalShooter.StopAllCoroutines();
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            resumeCoroutine = StartCoroutine(ResumeAfterDelay());
        }
    }

    IEnumerator ResumeAfterDelay()
    {
       
        yield return new WaitForSeconds(delayBeforeResume);
        goalShooter.enabled = true;
      
    }

}
