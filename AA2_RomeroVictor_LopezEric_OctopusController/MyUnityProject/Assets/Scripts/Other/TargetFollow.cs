using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollow : MonoBehaviour
{
    private Transform target;
    private float speed;
    private bool targetToFollow;

    void Start()
    {
        speed = 20.0f;
        targetToFollow = false;
    }

    public void InitFollowTarget(Transform newTarget)
    {
        target = newTarget;
        targetToFollow = true;
    }

    void Update()
    {
        if (targetToFollow)
        {
            transform.position += (target.position - transform.position).normalized * speed * Time.deltaTime;

            if ((target.position - transform.position).magnitude <= 0.5f)
            {
                targetToFollow = false;
            }
        }
    }
}
