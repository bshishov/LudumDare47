using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAtCamera : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject Camera;   
    public Text TimerText;

    private GameObject _camera;
    private Vector3 _dif;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _dif = transform.parent.position - transform.position;
        transform.Rotate(0, -transform.parent.eulerAngles.y, 0);
    }

    // Update is called once per frame
    void Update()
    {   
        transform.position = transform.parent.position - _dif;

        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);


    }

}
