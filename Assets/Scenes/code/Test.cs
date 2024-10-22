using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update()
    {
        Debug.DrawLine(transform.localPosition - new Vector3(5, 0, 0), new Vector3(5, 0, 0) + transform.localPosition, Color.red);
        Debug.Log(transform.position.ToString());
        Debug.Log(transform.localPosition.ToString());
    }
}
