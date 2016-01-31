using UnityEngine;
using System.Collections;

public class BallMove : MonoBehaviour {

    public float Range;

	void Start ()
    {

	}
	
	void Update ()
    {
        float deltaX = Range * Mathf.Cos(Time.time * Mathf.PI / 4.0f);
        float deltaZ = Range * Mathf.Sin(Time.time * Mathf.PI / 4.0f);

        transform.position = transform.position + new Vector3(deltaX, 0.0f, deltaZ);
	}
}
