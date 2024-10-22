using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class karryn : MonoBehaviour
{
    private Animator ain; // 声明动画组件变量
    public Rigidbody2D rBody;  // 声明刚体组件变量

    public float moveSpeed = 2f;  // 移动速度
    public float runSpeedMultiplier = 2f; // 加速时的速度倍率
    private bool isRunning = false; // 是否正在加速
    private Vector2 inputDir; // 存储输入方向
    private float horizontal; // 存储水平输入值
    private float vertical; // 存储垂直输入值
    private float x, y; // 存储动画方向

    private void Start()
    {
        Global.instance.playerData.name = gameObject.name;
        transform.localPosition = new Vector3(Global.instance.playerData.x, Global.instance.playerData.y, Global.instance.playerData.z);
        ain = GetComponent<Animator>();   // 获取动画组件
        rBody = GetComponent<Rigidbody2D>();   // 获取刚体组件
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");  // 获取水平轴输入值
        vertical = Input.GetAxisRaw("Vertical");      // 获取垂直轴输入值

        if (Input.GetKey(KeyCode.LeftShift)) // 如果按下shift键，则加速
        {
            isRunning = true;
        }
        else // 否则正常行走
        {
            isRunning = false;
        }
        inputDir = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        var Speed = isRunning ? runSpeedMultiplier : moveSpeed;
        rBody.velocity = new Vector2(inputDir.x * Speed * Time.deltaTime, inputDir.y * Speed * Time.deltaTime);

        if (inputDir != Vector2.zero)
        {
            ain.SetBool("isMoving", true);
            x = inputDir.x;
            y = inputDir.y;
        }
        else
        {
            ain.SetBool("isMoving", false);
            isRunning = false;
        }

        ain.SetFloat("Horizontal", x);
        ain.SetFloat("Vertical", y);
    }

    public void stopPlayerAction()
    {
        //防止玩家移动角色
        GameObject obj = gameObject;
        if (obj != null)
        {
            obj.GetComponent<Animator>().enabled = false;
            obj.GetComponent<karryn>().enabled = false;
            obj.GetComponent<Rigidbody2D>().Sleep();
        }
    }

    public void enablePlayerAction()
    {
        GameObject obj = gameObject;
        obj.GetComponent<Animator>().enabled = true;
        obj.GetComponent<karryn>().enabled = true;
        obj.GetComponent<Rigidbody2D>().WakeUp();
    }
}