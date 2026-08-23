using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Enemy
{
    public class Crawler : EnemyFSM
    {
        public Collider2D facingDetector; //用来检测前方是否有墙壁或悬崖, 如果有就转向
        public ContactFilter2D contact;

        public CrawlerParameter Crawler_parameter;
        public SpriteToSolidOrange _spriteToSolidOrange;

        public Animator _hitLight;
        public GameObject groundCheck;
        public int circleRadius; //检测墙壁时的圆形范围, 圆心为groundCheck的位置, 如果这个圆和地面有重叠就说明在地面上
        public LayerMask ground;
        bool isWall;
        void Awake()
        {
            base.Awake();
            
            Crawler_parameter = new CrawlerParameter();
            _spriteToSolidOrange = GetComponentInChildren<SpriteToSolidOrange>();

            states = new Dictionary<States, IState>
            {
                { States.Patrol, new CrawlerPatrol(this) },
                { States.Hurt, new CrawlerHurt(this) },
                { States.Dead, new CrawlerDead(this) }
            };
            currentState = states[States.Patrol];
        }

        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            
            currentHealth = Crawler_parameter.health;
        }

        // Update is called once per frame
        void Update()
        {
            base.Update();
            
            FacingDetect();
            
        }

        void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private void FacingDetect()
        {
            isWall = Physics2D.OverlapCircle(groundCheck.transform.position, circleRadius, ground);
            if (!isWall)
            {
                Flip();
            }
            else
            {
                int count = Physics2D.OverlapCollider(facingDetector, contact, new List<Collider2D>());
                if (count > 0)
                {
                    Flip();
                }
            }
        }

        private void Flip()
        {
            Vector3 vector = transform.localScale;
            vector.x *= -1;
            transform.localScale = vector;

        }

        public override void Hurt(int damage, Transform attackPosition)
        {
            _hitLight.Play("HitLight");
            _spriteToSolidOrange.TriggerHitEffect(attackPosition);
            this.damage = damage;
            attackDirection = transform.position - attackPosition.position;
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                TransitionState(States.Dead);
            }
            else
            {
                TransitionState(States.Hurt);
            }
        }

        public override void SpawnCoins()
        {
            int randomCount = Random.Range(Crawler_parameter.minSpawnCoins, Crawler_parameter.maxSpawnCoins); //2,3,4
            for (int i = 0; i < randomCount; i++)
            {
                GameObject geo = Instantiate(coin, transform.position, Quaternion.identity, transform.parent);
                Vector2 force = new Vector2(Random.Range(-Crawler_parameter.maxBumpXForce, Crawler_parameter.maxBumpXForce),
                    Random.Range(Crawler_parameter.minBumpYForce, Crawler_parameter.maxBumpYForce));
                geo.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

            }
        }

        #region 事件
        public void Dead()
        {
            SpawnCoins();
            gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
            gameObject.SetActive(false);
        }
        #endregion
    }
    
    public class CrawlerPatrol : IState
    {
        private Crawler manager;
        private CrawlerParameter parameter;
        private AudioSource patrol;

        public CrawlerPatrol(Crawler manager)
        {
            this.manager = manager;
            this.parameter = manager.Crawler_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("Walk");
            manager.audio.PlaySound("Enemy/Crawler/crawler",true, 1f,(sound) =>
            {
                patrol = sound;
            });
        }

        public void OnUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            manager.rb.position = Vector2.MoveTowards(manager.rb.position, manager.rb.position + new Vector2(manager.transform.localScale.x,0) * parameter.moveSpeed, parameter.moveSpeed * Time.deltaTime);
        }

        public void OnExit()
        {
            if (patrol != null)
            {
                manager.audio.StopSound(patrol);
            }
            else
            {
                patrol = manager.GetComponent<AudioSource>();
                patrol.enabled = false;
            }
        }
    }
    
    public class CrawlerHurt : IState
    {
        private Crawler manager;
        private CrawlerParameter parameter;
        private float timer;

        public CrawlerHurt(Crawler manager)
        {
            this.manager = manager;
            this.parameter = manager.Crawler_parameter;
        }

        public void OnEnter()
        {
            timer = 0;
            manager.audio.PlaySound("Enemy/Crawler/enemy_damage",false);
            manager.rb.velocity = Vector2.zero;
            if (manager.attackDirection.x > 0)
            {
                manager.rb.AddForce(new Vector2(parameter.hurtForce, 0), ForceMode2D.Impulse);
            }
            else
            {
                manager.rb.AddForce(new Vector2(-parameter.hurtForce, 0), ForceMode2D.Impulse);
            }
        }

        public void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                manager.TransitionState(States.Patrol);
            }
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
    
    public class CrawlerDead : IState
    {
        private Crawler manager;
        private CrawlerParameter parameter;

        public CrawlerDead(Crawler manager)
        {
            this.manager = manager;
            this.parameter = manager.Crawler_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("Dead");
            manager.audio.PlaySound("Enemy/Crawler/enemy_death_sword",false);
            Vector3 diff = (GameObject.FindWithTag("Player").transform.position - manager.transform.position).normalized;
            manager.rb.velocity = Vector2.zero;
            if (diff.x < 0)
            {
                manager.rb.AddForce(Vector2.right * parameter.deadForce, ForceMode2D.Impulse);
            }
            else
                manager.rb.AddForce(Vector2.left * parameter.deadForce, ForceMode2D.Impulse);
        }

        public void OnUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
}
