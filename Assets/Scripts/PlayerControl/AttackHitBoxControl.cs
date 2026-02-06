using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitBoxControl : MonoBehaviour
{
    [Tooltip("普攻碰撞箱引用")]
    [SerializeField] private Collider2D attackHitbox;
    [Tooltip("防反第一段攻击碰撞箱引用")]
    [SerializeField] private Collider2D parryHitbox;
    [Tooltip("防反第二段攻击碰撞箱引用")]
    [SerializeField] private Collider2D parryFollowupHitbox;

    private void Awake()
    {
        attackHitbox.enabled = false;
        parryHitbox.enabled = false;
        parryFollowupHitbox.enabled = false;
    }

    public void EnableAttackHitbox()
    {
        attackHitbox.enabled = true;
    }

    public void DisableAttackHitbox()
    {
        attackHitbox.enabled = false;
    }

    public void EnableParryHitbox()
    {
        parryHitbox.enabled = true;
    }

    public void DisableParryHitbox()
    {
        parryHitbox.enabled = false;
    }

    public void EnableParryFollowupHitbox()
    {
        parryFollowupHitbox.enabled = true;
    }

    public void DisableParryFollowupHitbox()
    {
        parryFollowupHitbox.enabled = false;
    }
}
