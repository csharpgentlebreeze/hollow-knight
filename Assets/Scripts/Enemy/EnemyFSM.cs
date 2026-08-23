using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace Enemy
{
    public enum States
    {
        Alert,
        Idle,
        Rotate,
        Patrol, 
        Chase, 
        Attack, 
        Hurt, 
        Dead,
        Sleep,
        Skill1,
        Skill2,
        Event1,
        HitUp,
        HitDown,
        Hit,
        Charge,
    }
    public class EnemyFSM : MonoBehaviour //敌人类, 继承自可被破坏的类, 具有血量和受伤方法. 还具有生成金币的方法, 和碰撞检测方法. 其他敌人类继承自这个类, 可以重写碰撞检测方法来实现不同的碰撞效果.
    {
        public GameObject coin; //敌人死后生成的金币预制体.
        public Rigidbody2D rb;
        public BoxCollider2D box;
        public AnimationController anim;
        public AudioController audio;
        
        public Dictionary<States, IState> states = new Dictionary<States, IState>();
        protected IState currentState;
        public Stack<IState> lastStates = new Stack<IState>();
        private Parameter parameter;
        
        public int currentHealth;
        public bool isFacingRight;
        public int damage;
        public Vector2 attackDirection;

        protected void Awake()
        {
            anim = GetComponent<AnimationController>();
            rb = GetComponent<Rigidbody2D>();
            box = GetComponent<BoxCollider2D>();
            audio = GetComponent<AudioController>();
            parameter = new Parameter();
        }
        protected void Start()
        {

        }

        // Update is called once per frame
        protected void Update()
        {
            if (transform.localScale.x == 1)
            {
                isFacingRight = true;
            }
            else if (transform.localScale.x == -1)
            {
                isFacingRight = false;
            }
            
            currentState.OnUpdate();
        }

        protected void FixedUpdate()
        {
            currentState.OnFixedUpdate();
        }
        
        public void TransitionState(States newState)
        {
            if(currentState != null)
            {
                lastStates.Push(currentState);
                currentState.OnExit();
                currentState = states[newState];
                currentState.OnEnter();
            }
        }
        
        /*public void TransitionLastState(int index)
        {
            IState state = null;
            for (int i = 0; i < index; i++)
            {
                if (lastStates.Count > 0)
                {
                    state = lastStates.Pop();
                }
            }
            currentState.OnExit();
            currentState = state;
            currentState.OnEnter();
            
        }*/

        public virtual void Hurt(int damage, Transform attackPosition)
        {
            
        }

        public virtual void SpawnCoins() //爆金币
        {
            
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("HeroDetector"))
            {
                PlayerFSM player = FindAnyObjectByType<PlayerFSM>();
                player.TakeDamage(1,transform);
            }
        }
    }
}
