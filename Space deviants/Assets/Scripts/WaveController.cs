using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class WaveController : MonoBehaviour
{
    float time = 0;
    Vector3 pos;
    Vector3 axis;
    public float moveSpeed = 1.5f;
    public float frequency = 5;
    public float magnitude = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        axis = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        pos += transform.up * Time.deltaTime * moveSpeed;
        transform.position = pos + axis * Mathf.Sin(Time.time * frequency) * magnitude;
    }
}
