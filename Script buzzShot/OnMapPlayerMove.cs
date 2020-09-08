using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Playerの上空に設置されるオブジェクト用
/// </summary>
public class OnMapPlayerMove : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private float angle = 0;
    private Vector3 axis = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(player.transform.position.x, 55, player.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, 55, player.transform.position.z);

        //回転を同期
        transform.eulerAngles =new Vector3(0, player.transform.eulerAngles.y,0);
        //player.transform.rotation.ToAngleAxis(out angle, out axis);
        //transform.rotation = Quaternion.AngleAxis(angle, axis);
    }
}
