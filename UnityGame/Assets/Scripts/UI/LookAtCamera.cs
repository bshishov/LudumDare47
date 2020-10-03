using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Camera;

    private Vector3 _dif;

    void Start()
    {
        _dif = transform.parent.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = transform.position - Camera.transform.position;
        v.x = 0;
        v.z = 0;
        transform.rotation = Quaternion.LookRotation(v - Camera.transform.position);
        transform.position = transform.parent.position - _dif;
    }
}
