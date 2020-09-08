using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//周回オブジェクト用テストスクリプト

public class RoundAI : MonoBehaviour
{
       
    private int objectCount = 1;
    private GameObject target;

    [SerializeField]
    private GameObject point1;

    [SerializeField]
    private GameObject point2;

    [SerializeField]
    private GameObject point3;

    // Use this for initialization
    void Start()
    {
        target = point1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = target.transform.position - transform.position;
        GetComponent<Rigidbody>().AddForce(direction.normalized * 10);
    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("point"))
        {
            if (objectCount == 1)
            {
                target = point2;
            }

            if (objectCount == 2)
            {
                target = point3;
            }

            if (objectCount == 3)
            {
                target = point1;
            }
        }

    }

}
