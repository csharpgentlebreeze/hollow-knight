using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

namespace Enemy
{
    public class Gruz : EnemyFSM
    {
        public GruzParameter Gr_parameter;
        public SpriteToSolidOrange _spriteToSolidOrange;
        public GameObject player;
        public AStarPathFinding _pathFinding;
        public List<Node> path;
        public Vector2[] patrolPath;
        public Animator _hitLight;
        
        void Awake()
        {
            base.Awake();

            Gr_parameter = new GruzParameter();
            patrolPath = new Vector2[2]
            {
                new Vector2(transform.position.x - 3,transform.position.y),
                new Vector2(transform.position.x + 3,transform.position.y)
            };
            _spriteToSolidOrange = GetComponentInChildren<SpriteToSolidOrange>();
            _pathFinding = GetComponent<AStarPathFinding>();

            states = new Dictionary<States, IState>
            {
                { States.Idle, new Gruz_Idle(this) },
                { States.Hurt, new Gruz_Hurt(this)},
                { States.Chase, new Gruz_Chase(this) },
                { States.Dead, new Gruz_Dead(this) },
            };
            currentState = states[States.Idle];
        }
        
        void Start()
        {
            base.Start();

            currentHealth = Gr_parameter.health;
        }
        
        void Update()
        {
            base.Update();
        }

        void FixedUpdate()
        {
            base.FixedUpdate();
        }
        
        public void DetectPlayer()
        {
            if(Physics2D.OverlapCircle(transform.position, Gr_parameter.detectDistance, LayerMask.GetMask("HeroDetector")))
            {
                player = Physics2D.OverlapCircle(transform.position, Gr_parameter.detectDistance, LayerMask.GetMask("HeroDetector")).gameObject;
            }
        }
        
        public void FlipTo(Vector2 target)
        {
            Vector2 direction = target - (Vector2)transform.position;
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        
        public override void Hurt(int damage, Transform attackPosition)
        {
            _hitLight.Play("HitLight");
            _spriteToSolidOrange.TriggerHitEffect(attackPosition);
            AudioManager.Instance.PlaySound("Enemy/GruzMother/boss_hit", false,1f);
            this.damage = damage;
            currentHealth -= 1;
            attackDirection = transform.position - attackPosition.position;

            if (currentHealth <= 0)
            {
                TransitionState(States.Dead);
            }
            else
            {
                TransitionState(States.Hurt);
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Gr_parameter.detectDistance);
            Gizmos.color = Color.green;
            if (path != null)
            {
                foreach (Node node in path)
                {
                    Gizmos.DrawCube(node.position, Vector2.one);
                }
            }
        }
    }
    
    public class Gruz_Idle : IState
    {
        private Gruz manager;
        private GruzParameter parameter;
        private int patrolIndex = 0;
        public Gruz_Idle(Gruz manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }
        public void OnEnter()
        {
            manager.anim.Play("Fly");
            manager.InvokeRepeating("DetectPlayer",0.5f,0.5f);
        }

        public void OnUpdate()
        {
            if (manager.player != null)
            {
                manager.TransitionState(States.Chase);
            }
        }

        public void OnFixedUpdate()
        {
            manager.path = manager._pathFinding.FindPath(manager.transform.position,manager.patrolPath[patrolIndex]);
            if (manager.path.Count == 1)
            {
                manager.rb.position = Vector2.MoveTowards(manager.rb.position,manager.path[0].position,parameter.moveSpeed * Time.fixedDeltaTime);
                manager.FlipTo(manager.path[0].position);
            }
            else if (manager.path.Count == 0)
            {
                manager.TransitionState(States.Idle);
            }
            else
            {
                manager.rb.position = Vector2.MoveTowards(manager.rb.position,manager.path[1].position,parameter.moveSpeed * Time.fixedDeltaTime);
                manager.FlipTo(manager.path[1].position);
            }
        }

        public void OnExit()
        {
            patrolIndex = (patrolIndex + 1) % manager.patrolPath.Length;
        }
    }
    
    public class Gruz_Hurt : IState
    {
        private Gruz manager;
        private GruzParameter parameter;
        private float timer;
        public Gruz_Hurt(Gruz manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }
        public void OnEnter()
        {
            timer = 0;
            manager.rb.velocity = Vector2.zero;
            manager.rb.AddForce(manager.attackDirection * parameter.hurtForcce, ForceMode2D.Impulse);
        }

        public void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                manager.TransitionState(States.Chase);
            }
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
    
    public class Gruz_Chase : IState
    {
        private Gruz manager;
        private GruzParameter parameter;
        public Gruz_Chase(Gruz manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }
        public void OnEnter()
        {
            manager.anim.Play("Fly");
        }

        public void OnUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            if (manager.player)
            {
                manager.path = manager._pathFinding.FindPath(manager.rb.position, manager.player.transform.position);
                if (manager.path == null)
                {
                    manager.CancelInvoke("DetectPlayer");
                    manager.player = null;
                    manager.TransitionState(States.Idle);
                    /*parameter.recovery = 1f;*/
                }
                else
                {
                    if (manager.path.Count == 0)
                    {
                        return;
                    }
                    if (manager.path.Count == 1)
                    {
                        manager.rb.position = Vector2.MoveTowards(manager.rb.position,manager.path[0].position,parameter.moveSpeed * Time.fixedDeltaTime);
                        manager.FlipTo(manager.player.transform.position);
                    }
                    else
                    {
                        manager.rb.position = Vector2.MoveTowards(manager.rb.position,manager.path[1].position,parameter.moveSpeed * Time.fixedDeltaTime);
                        manager.FlipTo(manager.player.transform.position);
                    }
                }
            }
            else
            {
                if (manager.path.Count == 0 && manager.player == null)
                {
                    manager.CancelInvoke("DetectPlayer");
                    manager.TransitionState(States.Idle);
                    /*parameter.recovery = 1f;*/
                }
                manager.rb.position = Vector2.MoveTowards(manager.rb.position,manager.path[1].position,parameter.moveSpeed * Time.fixedDeltaTime);
                manager.FlipTo(manager.path[1].position);
            }
        }

        public void OnExit()
        {
            
        }
    }

    public class Gruz_Dead : IState
    {
        private Gruz manager;
        private GruzParameter parameter;
        public Gruz_Dead(Gruz manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            EventManager.Instance.EventTrigger("GruzDead");
            manager.anim.Play("Dead_fly");
            manager.rb.constraints = RigidbodyConstraints2D.None;
            manager.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
            manager.rb.gravityScale = 1;
            manager.rb.AddForce(new Vector2(-manager.transform.localScale.x * parameter.deadForce, 5), ForceMode2D.Impulse);
        }

        public void OnUpdate()
        {
            if (Physics2D.Raycast(manager.transform.position, Vector2.down, manager.box.bounds.extents.y + 0.1f,LayerMask.GetMask("Terrain")))
            {
                manager.anim.Play("Dead");
            }
            if(manager.anim.stateInfo.IsName("Dead") && manager.anim.stateInfo.normalizedTime >= 1f)
            {
                manager.gameObject.SetActive(false);
            }
        }

        public void OnExit()
        {

        }
    }
}
