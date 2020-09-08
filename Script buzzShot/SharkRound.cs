using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自身の回転用
/// </summary>
public class SharkRound : MonoBehaviour
{
    [SerializeField]
    private float angle;

    [SerializeField]
    private GameObject target;

    void Start()
    {
        
    }

    void Update()
    {
        transform.RotateAround(target.transform.position, Vector3.up, angle * Time.deltaTime);
    }
}
