using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;      // 每秒钟移动的距离
    private Vector3 yRotation = Vector3.zero;     // 水平旋转角色
    private Vector3 xRotation = Vector3.zero;     // 竖直旋转摄像机

    private float cameraRotationTotal = 0f;
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Vector3 thrusterForce = Vector3.zero; // 向上推力

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _yRotation, Vector3 _xRotation)
    {
        yRotation = _yRotation;
        xRotation = _xRotation;
    }

    public void Thrust(Vector3 _thrusterForce)
    {
        thrusterForce = _thrusterForce;
    }

    private void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce);  //作用Time.fixedDeltaTime秒（0.02秒）
        }
    }

    private void PerfromRotation()
    {
        if(yRotation != Vector3.zero)
        {
            rb.transform.Rotate(yRotation);
        }
        if(xRotation != Vector3.zero)
        {
            cameraRotationTotal += xRotation.x;
            cameraRotationTotal = Mathf.Clamp(cameraRotationTotal, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(cameraRotationTotal, 0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        PerformMovement();
        PerfromRotation();
    }
}
