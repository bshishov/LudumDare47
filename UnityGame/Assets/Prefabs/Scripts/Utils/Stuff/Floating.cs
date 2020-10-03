using UnityEngine;

public class Floating : MonoBehaviour
{
    public Vector3 Amplitude = new Vector3(0, 1, 0);
    public Vector3 Offset = new Vector3(0, 1, 0);
    public Vector3 Speed = new Vector3(0, 1, 0);
    public Vector3 Rotation = new Vector3(0, 5, 0);

    private Vector3 _initialPosition;

    void Start()
    {
        _initialPosition = transform.position;
    }

    void Update()
    {
        var t = Time.time;
        transform.position = _initialPosition + new Vector3(
                                 Amplitude.x * Mathf.Sin(Offset.x + Speed.x * t),
                                 Amplitude.y * Mathf.Sin(Offset.y + Speed.y * t),
                                 Amplitude.z * Mathf.Sin(Offset.z + Speed.z * t));

        transform.Rotate(Rotation); 
    }
}
