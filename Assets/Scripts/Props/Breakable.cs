using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour //可被破坏的的东西, 比如敌人和障碍物.
{
    [SerializeField]protected int health; //血量

    protected bool isDead;

    protected void CheckIsDead() //检查是否死亡
    {
        if (health <= 0 && !isDead)
        {
            Dead();
        }
    }
    public virtual void Hurt(int damage) //障碍物受伤或霸体敌人受伤, 不需要知道攻击位置
    {
        if (!isDead)
        {
            health -= damage;
        }
    }
    public virtual void Hurt(int damage, Transform attackPosition) //敌人受伤, 需要知道攻击位置来判断击退方向
    {
        if (!isDead)
        {
            health -= damage;
        }
    }
    protected virtual void Dead()
    {
        isDead = true;
    }
    
}
