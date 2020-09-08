using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTornado : MonoBehaviour
{
    [SerializeField]
    private Vector3 velocity;

    // Use this for initialization
    void Start()
    {
        transform.GetComponent<Rigidbody>().velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
