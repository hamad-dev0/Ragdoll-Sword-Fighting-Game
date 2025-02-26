using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public Ragdoll ragdoll;
    public Collider playerCollider;

    void Start()
    {
        
    }

    void Update()
    {
        if (ragdoll.isRagdoll == true)
        {
            StartCoroutine(RestartLevel());
        }
    }

    IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
