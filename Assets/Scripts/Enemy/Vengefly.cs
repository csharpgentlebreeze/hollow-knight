using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Vengefly : EnemyFSM
    {
        public VengeflyParameter Ve_parameter;
        public SpriteToSolidOrange _spriteToSolidOrange;
        public GameObject player;
        public AStarPathFinding _pathFinding;
        public List<Node> path;
        public Vector2 idlePossition;
        public Animator _hitLight;
        
        void Awake()
        {
            base.Awake();

            Ve_parameter = new VengeflyParameter();
            _spriteToSolidOrange = GetComponentInChildren<SpriteToSolidOrange>();
            _pathFinding = GetComponent<AStarPathFinding>();

            states = new Dictionary<States, IState>
            {
                { States.Idle, new Vengefly_Idle(this) },
                { States.Hurt, new Vengefly_Hurt(this)},
                { States.Chase, new Vengefly_Chase(this) },
                { States.Dead, new Vengefly_Dead(this) },
                { States.Rotate, new Vengefly_Rotate(this)},
                { States.Alert, new Vengefly_Alert(this)},
            };
            currentState = states[States.Idle];
            InvokeRepeating("DetectPlayer",0.5f,0.5f);
        }
        
        void Start()
        {
            base.Start();
            idlePossition = transform.position;

            currentHealth = Ve_parameter.health;
        }
        
        void Update()
        {
            base.Update();
            if (player != null)
            {
                float x = player.transform.position.x - transform.position.x;
                if ((transform.localScale.x > 0 && x < 0) || (transform.localScale.x < 0 && x > 0))
                {
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    TransitionState(States.Rotate);
                }
            }
        }

        void FixedUpdate()
        {
            base.FixedUpdate();
        }
        
        public void DetectPlayer()
        {
            if(Physics2D.OverlapCircle(transform.position, Ve_parameter.detectDistance, LayerMask.GetMask("HeroDetector")))
            {
                player = Physics2D.OverlapCircle(transform.position, Ve_parameter.detectDistance, LayerMask.GetMask("HeroDetector")).gameObject;
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
            AudioManager.Instance.PlaySound("Enemy/GruzMother/boss_hit", false,2f);
            this.damage = damage;
            currentHealth -= damage;
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
        
        public override void SpawnCoins()
        {
            int randomCount = Random.Range(Ve_parameter.minSpawnCoins, Ve_parameter.maxSpawnCoins); //2,3,4
            for (int i = 0; i < randomCount; i++)
            {
                GameObject geo = Instantiate(coin, transform.position, Quaternion.identity, transform.parent);
                Vector2 force = new Vector2(Random.Range(-Ve_parameter.maxBumpXForce, Ve_parameter.maxBumpXForce),
                    Random.Range(Ve_parameter.minBumpYForce, Ve_parameter.maxBumpYForce));
                geo.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

            }
        }
        public void OnDrawGizmos()
        {
            /*Gizmos.DrawWireSphere(transform.position, Ve_parameter.detectDistance);
            Gizmos.color = Color.green;
            if (path != null)
            {
                foreach (Node node in path)
                {
                    Gizmos.DrawCube(node.position, Vector2.one);
                }
            }*/
        }
    }
    
    public class Vengefly_Idle : IState
    {
        private Vengefly manager;
        private VengeflyParameter parameter;
        public Vengefly_Idle(Vengefly manager)
        {
            this.manager = manager;
            this.parameter = manager.Ve_parameter;
        }
        public void OnEnter()
        {
            manager.anim.Play("Idle");
            manager.InvokeRepeating("DetectPlayer",0.5f,0.5f);
        }

        public void OnUpdate()
        {
            if (manager.player != null)
            {
                manager.TransitionState(States.Alert);
            }
        }

        public void OnFixedUpdate()
        {
            manager.path = manager._pathFinding.FindPath(manager.transform.position, manager.idlePossition);
            if (manager.path.Count == 0)
            {
                return;
            }
            if (manager.path.Count == 1)
            {
                manager.rb.position = Vector2.MoveTowards(manager.rb.position,manager.path[0].position,parameter.moveSpeed * Time.fixedDeltaTime);
                manager.FlipTo(manager.path[0].position);
            }
            else
            {
                manager.rb.position = Vector2.MoveTowards(manager.rb.position,manager.path[1].position,parameter.moveSpeed * Time.fixedDeltaTime);
                manager.FlipTo(manager.path[1].position);
            }
        }

        public void OnExit()
        {
            
        }
    }
    
    public class Vengefly_Hurt : IState
    {
        private Vengefly manager;
        private VengeflyParameter parameter;
        private float timer;
        public Vengefly_Hurt(Vengefly manager)
        {
            this.manager = manager;
            this.parameter = manager.Ve_parameter;
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
    
    public class Vengefly_Chase : IState
    {
        private Vengefly manager;
        private VengeflyParameter parameter;
        public Vengefly_Chase(Vengefly manager)
        {
            this.manager = manager;
            this.parameter = manager.Ve_parameter;
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

    public class Vengefly_Dead : IState
    {
        private Vengefly manager;
        private VengeflyParameter parameter;
        public Vengefly_Dead(Vengefly manager)
        {
            this.manager = manager;
            this.parameter = manager.Ve_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("Dead");
            manager.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
            manager.rb.gravityScale = 1;
            manager.rb.AddForce(new Vector2(-manager.transform.localScale.x * parameter.deadForce, 5), ForceMode2D.Impulse);
        }

        public void OnUpdate()
        {
            if (Physics2D.Raycast(manager.transform.position, Vector2.down, manager.box.bounds.extents.y + 0.1f,LayerMask.GetMask("Terrain")))
            {
                manager.SpawnCoins();
                manager.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
                manager.gameObject.SetActive(false);
            }
        }

        public void OnExit()
        {

        }
    }
    
    public class Vengefly_Alert : IState
    {
        private Vengefly manager;
        private VengeflyParameter parameter;
        public Vengefly_Alert(Vengefly manager)
        {
            this.manager = manager;
            this.parameter = manager.Ve_parameter;
        }
        public void OnEnter()
        {
            int randomSound = Random.Range(1, 4);
            manager.anim.Play("Alert");
            manager.audio.PlaySound("Enemy/Vengefly/buzzer_startle_0" + randomSound.ToString(),false);
        }

        public void OnUpdate()
        {
            if (manager.anim.IsEnd())
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
    
    public class Vengefly_Rotate : IState
    {
        private Vengefly manager;
        private VengeflyParameter parameter;
        public Vengefly_Rotate(Vengefly manager)
        {
            this.manager = manager;
            this.parameter = manager.Ve_parameter;
        }
        public void OnEnter()
        {
            manager.anim.Play("Rotate");
        }

        public void OnUpdate()
        {
            if (manager.anim.IsEnd())
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
}
