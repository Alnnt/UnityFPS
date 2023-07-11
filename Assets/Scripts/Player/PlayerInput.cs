using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;              // 移速
    [SerializeField]
    private float lookSensitivity = 8f;    // 鼠标灵敏度
    [SerializeField]
    private float thrusterForce = 20f;     // 向上推力
    [SerializeField]
    private PlayerController controller;

    private ConfigurableJoint joint;       // 弹性关节

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        joint = GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        // 角色移动
        float xMov = Input.GetAxisRaw("Horizontal");
        float yMov = Input.GetAxisRaw("Vertical");
        
        Vector3 velocity = (transform.right * xMov + transform.forward * yMov).normalized * speed;
        controller.Move(velocity);

        // 视角旋转
        float xMouse = Input.GetAxisRaw("Mouse X");
        float yMouse = Input.GetAxisRaw("Mouse Y");
        
        Vector3 yRotation = new Vector3(0, xMouse, 0) * lookSensitivity;
        Vector3 xRotation = new Vector3(-yMouse, 0, 0) * lookSensitivity;
        controller.Rotate(yRotation, xRotation);

        // 飞行
        Vector3 force = Vector3.zero;
        if (Input.GetButton("Jump"))
        {
            force = Vector3.up * thrusterForce;
            joint.yDrive = new JointDrive
            {
                positionSpring = 0f,
                positionDamper = 0f,
                maximumForce = 0f,
            };
        } 
        else
        {
            joint.yDrive = new JointDrive
            {
                positionSpring = 20f,
                positionDamper = 0f,
                maximumForce = 40f,
            };
        }
        controller.Thrust(force);
    }
}
