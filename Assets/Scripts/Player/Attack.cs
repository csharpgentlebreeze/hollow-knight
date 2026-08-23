using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject slash;
    public GameObject altSlash;
    public GameObject downSlash;
    public GameObject upSlash;

    public ContactFilter2D enemyContactFilter;

    public enum AttackType
    {
        Slash, AltSlash, DownSlash, Upslash
    }
    public void Play(AttackType attackType, ref List<Collider2D> colliders)
    {
        // 确保每次检测前清空列表，以便收集本次所有命中的敌人
        colliders.Clear();

        int hitCount = 0;
        Collider2D attackCollider = null;
        AudioSource sfx = null;

        switch (attackType)
        {
            case AttackType.Slash:
                attackCollider = slash.GetComponent<Collider2D>();
                sfx = slash.GetComponent<AudioSource>();
                break;
            case AttackType.AltSlash:
                attackCollider = altSlash.GetComponent<Collider2D>();
                sfx = altSlash.GetComponent<AudioSource>();
                break;
            case AttackType.DownSlash:
                attackCollider = downSlash.GetComponent<Collider2D>();
                sfx = downSlash.GetComponent<AudioSource>();
                break;
            case AttackType.Upslash:
                attackCollider = upSlash.GetComponent<Collider2D>();
                sfx = upSlash.GetComponent<AudioSource>();
                break;
            default:
                break;

        }

        if (attackCollider != null)
        {
            hitCount = Physics2D.OverlapCollider(attackCollider, enemyContactFilter, colliders);
        }

        // 调试日志：打印命中数量和命中对象，方便排查只命中一个的原因
        Debug.Log("Attack.Play hitCount=" + hitCount + " attackCollider=" + (attackCollider? attackCollider.name : "null"));
        for (int i = 0; i < colliders.Count; i++)
        {
            Debug.Log("  hit[" + i + "]=" + (colliders[i] ? colliders[i].name : "null"));
        }

        if (sfx != null)
            sfx.Play();

        // 可选：遍历所有命中的敌人并处理（例如造成伤害）
        for (int i = 0; i < hitCount && i < colliders.Count; i++)
        {
            Collider2D enemyCol = colliders[i];
            // TODO: 对 enemyCol 所在的敌人对象进行伤害处理
            // var enemy = enemyCol.GetComponent<Enemy>(); if (enemy) enemy.TakeDamage(damage);
        }

    }
}
