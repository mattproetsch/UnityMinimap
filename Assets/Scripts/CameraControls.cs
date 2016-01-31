using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
    public float Speed = 10.0f;

    private bool m_rightClickDown;
    private Vector2 m_prevMousePosn;

    void Update()
    {
        // Update position
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * Speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Time.deltaTime * Speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * Speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * Speed;
        }
        if (Input.GetKey(KeyCode.C))
        {
            transform.position -= Vector3.up * Time.deltaTime * Speed;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += Vector3.up * Time.deltaTime * Speed;
        }

        // Update orientation
        if (Input.GetMouseButtonDown(1))
        {
            m_rightClickDown = true;
            m_prevMousePosn = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            m_rightClickDown = false;
        }
        else if (Input.GetMouseButton(1))
        {
            m_rightClickDown = true;
        }

        if (m_rightClickDown)
        {
            Vector2 mousePosn = Input.mousePosition;
            Vector2 delta = mousePosn - m_prevMousePosn;
            m_prevMousePosn = mousePosn;

            transform.Rotate(-delta.y * Speed / 14.0f, delta.x * Speed / 14.0f, 0.0f);
            Vector3 rot = transform.rotation.eulerAngles;
            float newX = 0.0f;
            if (rot.x >= 270.0f)
            {
                newX = Mathf.Clamp(rot.x, 270.1f, 360.0f);
            }
            else
            {
                newX = Mathf.Clamp(rot.x, 0.0f, 89.9f);
            }
            transform.rotation = Quaternion.Euler(newX, rot.y, 0.0f);
        }
    }
}
