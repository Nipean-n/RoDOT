using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("按键")]
    [Tooltip("左")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
    [Tooltip("右")]
    [SerializeField] private KeyCode moveRightKey = KeyCode.D;
    [Tooltip("跳跃")]
    [SerializeField] private KeyCode jumpKey = KeyCode.K;

    [Header("数值")]
    [Tooltip("横向移速")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("起跳速度")]
    [SerializeField] private float jumpSpeed = 10f;
    [Tooltip("短跳滞空减速系数")]
    [SerializeField] private float shortJumpFactor = 0.25f;
    [Tooltip("跳跃预输入时间（秒）")]
    [SerializeField] private float jumpPreInputFrames = 0.12f;
    [Tooltip("跳跃强制上升时间（秒）")]
    [SerializeField] private float jumpForceFrames = 0.08f;

    private bool grounded = false;
    private float jumpPreInputTimer = 0f;
    private float jumpForceTimer = 0f;

    void Update()
    {
        // 获取当前速度
        Vector2 vel = GetComponent<Rigidbody2D>().velocity;

        // 处理横向移动
        // 采用无惯性的即时响应式移动
        int keyInput = 0;

        if (Input.GetKey(moveRightKey))
        {
            keyInput += 1;
        }
        if (Input.GetKey(moveLeftKey))
        {
            keyInput -= 1;
        }

        vel.x = keyInput * moveSpeed;

        // 处理跳跃
        // 通过松开按键时折损上升速度实现滞空短跳
        // grounded变量由碰撞检测函数负责维护
        if (Input.GetKeyDown(jumpKey))
        {
            // 记录跳跃预输入
            jumpPreInputTimer = jumpPreInputFrames;
        }

        if(jumpPreInputTimer > 0)
        {
            // 递减跳跃预输入计时器
            jumpPreInputTimer -= Time.deltaTime;
        }

        if (jumpPreInputTimer > 0 && grounded)
        {
            vel.y = jumpSpeed;
            grounded = false;
            jumpForceTimer = jumpForceFrames; // 启动跳跃强制上升计时器
            jumpPreInputTimer = 0; // 重置跳跃预输入计时器
        }

        if(jumpForceTimer > 0)
        {
            // 递减跳跃强制上升计时器
            jumpForceTimer -= Time.deltaTime;
        }
        else if (!Input.GetKey(jumpKey) && vel.y > 0)
        {
            // 实现短跳：在跳跃强制上升时间结束后松开跳跃键会折损上升速度
            vel.y *= shortJumpFactor;
        }

        // 应用速度的修改
        GetComponent<Rigidbody2D>().velocity = vel;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检测与地面的碰撞以更新grounded状态

        if (collision.gameObject.CompareTag("Block"))
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y > 0.75f)
                {
                    grounded = true;
                    break;
                }
            }
        }
    }
}
