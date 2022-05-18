using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 3f));
        Animator anim = GetComponent<Animator>();
        anim.Play(Animator.StringToHash("Star"));
        Destroy(this);
    }
}
