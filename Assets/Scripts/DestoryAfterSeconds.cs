using System;
using System.Collections;
using UnityEngine;

public class KillMePlsAfterSeconds : MonoBehaviour
{
    public float seconds;
    
    public void Awake()
    {
        StartCoroutine(nameof(_KillMePlsAfterSeconds));
    }

    public IEnumerator _KillMePlsAfterSeconds()
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
