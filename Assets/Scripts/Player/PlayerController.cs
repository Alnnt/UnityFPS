using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;      // ÿ�����ƶ��ľ���
    private Vector3 yRotation = Vector3.zero;     // ˮƽ��ת��ɫ
    private Vector3 xRotation = Vector3.zero;     // ��ֱ��ת�����
    private float recoilForce = 0f; //������

    private float cameraRotationTotal = 0f;
    [SerializeField]
    private float cameraRotationLimit = 85f;

    private Vector3 thrusterForce = Vector3.zero; // ��������

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

    public void AddRecoilForce(float newRecoilForce)
    {
        recoilForce = newRecoilForce;
    }
    
    private void PerformMovement()
    {
        if(velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }

        if(thrusterForce != Vector3.zero)
        {
            rb.AddForce(thrusterForce);  //����Time.fixedDeltaTime�루0.02�룩
        }
    }

    private void PerformRotation()
    {
        if (recoilForce < 0.1)
        {
            recoilForce = 0f;
        }
        
        if(yRotation != Vector3.zero || recoilForce > 0)
        {
            rb.transform.Rotate(yRotation + Vector3.up * Random.Range(-2f * recoilForce, 2f * recoilForce));
        }
        if(xRotation != Vector3.zero || recoilForce > 0)
        {
            cameraRotationTotal += xRotation.x - recoilForce;
            cameraRotationTotal = Mathf.Clamp(cameraRotationTotal, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(cameraRotationTotal, 0f, 0f);
        }

        recoilForce *= 0.5f;
    }

    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }
}
