using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class GruzMother : EnemyFSM
    {
        public ParticleSystem _finalHitEffect;
        public ParticleSystem _finalSmokeEffect;
        public ParticleSystem _finalBurstEffect;
        public ParticleSystem _burstChildEffect;
        
        public GruzMotherParameter Gr_parameter;
        public CinemachineImpulseSource myImpulse;
        public ContactFilter2D contactFilter;
        public SpriteToSolidOrange _spriteToSolidOrange;
        public List<Collider2D> contacts;
        private AudioSource snore;
        public Animator _hitLight;
        public bool isFlip = false;

        public GameObject player;

        void Awake()
        {
            base.Awake();

            Gr_parameter = new GruzMotherParameter();
            myImpulse = GetComponent<CinemachineImpulseSource>();
            snore = GetComponent<AudioSource>();
            _spriteToSolidOrange = GetComponentInChildren<SpriteToSolidOrange>();

            states = new Dictionary<States, IState>
            {
                { States.Sleep, new GruzSleep(this) },
                { States.Idle, new GruzIdle(this) },
                { States.Attack, new GruzAttack(this) },
                { States.Charge, new GruzCharge(this)},
                { States.Skill1, new GruzMutiCrash(this) },
                { States.Event1, new GruzBurst(this) },
                { States.Dead, new GruzDead(this) },
                { States.Hit, new GruzHit(this)},
                { States.HitUp, new GruzHitUp(this)},
                { States.HitDown, new GruzHitDown(this)}
            };
            currentState = states[States.Sleep];
        }

        // Start is called before the first frame update
        void Start()
        {
            base.Start();

            currentHealth = Gr_parameter.health;
        }

        // Update is called once per frame
        void Update()
        {
            base.Update();
        }

        void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public void OnDrawGizmos()
        {

        }

        public void Flip()
        {
            Vector2 direction = player.transform.position - transform.position;
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                isFacingRight = false;
            }
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
                isFacingRight = true;
            }
        }
        
        public override void SpawnCoins()
        {
            for (int i = 0; i < Gr_parameter.coins; i++)
            {
                GameObject geo = Instantiate(coin, transform.position, Quaternion.identity, transform.parent);
                Vector2 force = new Vector2(Random.Range(-Gr_parameter.maxBumpXForce, Gr_parameter.maxBumpXForce),
                    Random.Range(Gr_parameter.minBumpYForce, Gr_parameter.maxBumpYForce));
                geo.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

            }
        }

        public void WakeUp()
        {
            EventManager.Instance.EventTrigger("GruzWakeUp");
            audio.PlaySound("Enemy/GruzMother/big_fly_snore_awake", false);
            Destroy(snore);
            player = GameObject.FindWithTag("Player");
            TransitionState(States.Idle);
        }

        public void Dead()
        {
            EventManager.Instance.EventTrigger("GruzDead");
        }

        public void BurstAudio1()
        {
            AudioManager.Instance.PlaySound("Enemy/GruzMother/big_fly_stomache_problems_1", false);
        }
        
        public void BurstAudio2()
        {
            AudioManager.Instance.PlaySound("Enemy/GruzMother/big_fly_stomache_problems_2", false);
        }

        public IEnumerator ContinuousShake(int count)
        {
            myImpulse.m_ImpulseDefinition.m_ImpulseDuration = 0.05f;
            for (int i = 0; i < count; i++)
            {
                myImpulse.GenerateImpulseWithVelocity(new Vector3(0.5f,0.5f,0));
                yield return new WaitForSeconds(0.05f);
                myImpulse.GenerateImpulseWithVelocity(new Vector3(0.5f,-0.5f,0));
                yield return new WaitForSeconds(0.05f);
                myImpulse.GenerateImpulseWithVelocity(new Vector3(-0.5f,-0.5f,0));
                yield return new WaitForSeconds(0.05f);
                myImpulse.GenerateImpulseWithVelocity(new Vector3(-0.5f,0.5f,0));
                yield return new WaitForSeconds(0.05f);
            }
            myImpulse.m_ImpulseDefinition.m_ImpulseDuration = 0.2f;
        }
        
        public void BurstFinalAudio()
        {
            StartCoroutine(ContinuousShake(5));
            AudioManager.Instance.PlaySound("Enemy/GruzMother/big_fly_stomache_problems_final_and_explode", false);
        }

        public void Burst()
        {
            _burstChildEffect.Play();
            EventManager.Instance.EventTrigger("Burst");
        }

        public override void Hurt(int damage, Transform attackPosition)
        {
            _hitLight.Play("HitLight");
            _spriteToSolidOrange.TriggerHitEffect(attackPosition);
            AudioManager.Instance.PlaySound("Enemy/GruzMother/boss_hit", false,2f);
            this.damage = damage;
            currentHealth -= damage;
            if (currentState == states[States.Sleep])
            {
                WakeUp();
            }

            if (currentHealth <= 0)
            {
                TransitionState(States.Dead);
            }
        }
    }

    public class GruzSleep : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;

        public GruzSleep(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("Sleep");
            manager.audio.PlaySound("Enemy/GruzMother/big_fly_snore_loop", true);
        }

        public void OnUpdate()
        {

        }

        public void OnExit()
        {

        }
    }

    public class GruzIdle : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;
        private AudioSource fly;
        private float idleRadius = 10f;
        private float stayTimer;
        private float attackTimer;
        private float stuckTimer;
        private Vector2 targetPosition;
        private Vector2 lastPosition;
        private RaycastHit2D hit;

        public GruzIdle(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("Fly");
            manager.audio.PlaySound("Enemy/GruzMother/big_fly_flying",true, 0.7f,(audio) =>
            {
                fly = audio;
            });
            SetNextPosition();
            stayTimer = 0f;
            attackTimer = 0f;
            stuckTimer = 0f;
        }

        public void OnUpdate()
        {
            manager.Flip();
            attackTimer += Time.deltaTime;
            if (attackTimer > UnityEngine.Random.Range(2f, 3f))
            {
                manager.TransitionState(States.Charge);
            }
            
            Watch();
            StuckDetect();
            
        }

        public void OnFixedUpdate()
        {
            
        }

        public void OnExit()
        {
            manager.audio.StopSound(fly);
        }

        public void StuckDetect()
        {
            if ((Vector2)manager.transform.position != lastPosition)
            {
                lastPosition = manager.transform.position;
                stuckTimer = 0f;
            }
            stuckTimer += Time.deltaTime;
            if (stuckTimer >= 1f)
            {
                SetNextPosition();
            }
        }

        private void Watch()
        {
            if (Vector2.Distance(manager.transform.position, targetPosition) > 1f)
            {
                // 计算归一化飞行方向（2D）
                Vector2 moveDir = (targetPosition - (Vector2)manager.transform.position).normalized;
                // 平滑移动：Time.deltaTime保证帧率无关
                manager.transform.Translate(moveDir * parameter.moveSpeed * Time.deltaTime);
            }
            else
            {
                int flag = UnityEngine.Random.Range(0, 2);
                if (flag == 0)
                {
                    stayTimer += Time.deltaTime;
                    // 停留时间到 → 生成新的随机目标点，重置计时器
                    if (stayTimer >= 0.5f)
                    {
                        SetNextPosition();
                        stayTimer = 0;
                    }
                }
                else //跳过停留直接飞向下一个目标点
                {
                    SetNextPosition();
                }
            }
        }

        private void SetNextPosition()
        {
            hit = Physics2D.Raycast(manager.transform.position,Vector2.down, Mathf.Infinity, LayerMask.GetMask("Terrain"));
            // 在指定半径内生成随机方向
            Vector2 randomDir = UnityEngine.Random.insideUnitSphere * idleRadius;
            // 以初始中心为基准计算目标点
            targetPosition = (Vector2)manager.transform.position + randomDir;
            targetPosition.x = Mathf.Clamp(targetPosition.x, 130, 169);
            targetPosition.y = Mathf.Clamp(targetPosition.y, hit.point.y + 3f, hit.point.y + 8f);
        }
    }
    
    public class GruzCharge : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;
        private AudioSource charge;

        public GruzCharge(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("Charge");
            manager.audio.PlaySound("Enemy/GruzMother/big_fly_charge_loop 1",false, 0.7f,(audio) =>
            {
                charge = audio;
            });
        }

        public void OnUpdate()
        {
            if (manager.anim.IsEnd())
            {
                int flag = UnityEngine.Random.Range(0, 2);
                if (flag == 0)
                {
                    manager.TransitionState(States.Attack);
                }
                else
                {
                    manager.TransitionState(States.Skill1);
                }
            }
        }
        
        public void OnFixedUpdate()
        {
            
        }

        public void OnExit()
        {
            manager.audio.StopSound(charge);
        }
    }

    public class GruzAttack : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;
        private Vector2 direction;
        public RaycastHit2D hit;

        public GruzAttack(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("Crash");
            direction = (manager.player.transform.position - manager.transform.position).normalized;
            if (direction.x > 0)
            {
                manager.transform.localScale = new Vector3(1, manager.transform.localScale.y, manager.transform.localScale.z);
            }
            if(direction.x < 0)
            {
                manager.transform.localScale = new Vector3(-1, manager.transform.localScale.y, manager.transform.localScale.z);
            }
        }

        public void OnUpdate()
        {
            hit = Physics2D.BoxCast(manager.transform.position, manager.box.size, 0, direction.normalized, 0.2f,
                LayerMask.GetMask("Terrain"));
            if (hit.point != Vector2.zero)
            {
                if (hit.normal == Vector2.up)
                {
                    manager.TransitionState(States.HitDown);
                }
                else if (hit.normal == Vector2.down)
                {
                    manager.TransitionState(States.HitUp);
                }
                else
                {
                    manager.TransitionState(States.Hit);
                }
            }

        }
        
        public void OnFixedUpdate()
        {
            manager.rb.position += direction.normalized * parameter.crashSpeed * Time.fixedDeltaTime;
        }

        public void OnExit()
        {

        }
    }

    public class GruzMutiCrash : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;
        private Vector2 direction;
        private float horizontalDirection;
        private float detectTimer;
        public Vector2 hitPosition;
        public RaycastHit2D[] hits;
        public int hitCount;
        public bool isOn;
        /*private bool isFlip = false;*/

        public GruzMutiCrash(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            if(manager.isFlip)Flip();
            isOn = true;
            detectTimer = 0;
            if (hitCount == 0)
            {
                if ((manager.player.transform.position - manager.transform.position).x < 0)
                {
                    horizontalDirection = -1;
                }
                if((manager.player.transform.position - manager.transform.position).x > 0)
                {
                    horizontalDirection = 1;
                }
                manager.transform.localScale = new Vector3(horizontalDirection, manager.transform.localScale.y, manager.transform.localScale.z);
            }
            Vector2 testDirection = new Vector2(horizontalDirection, Mathf.Pow(-1, hitCount) * Mathf.Tan(Mathf.Deg2Rad * 80)).normalized;
            manager.anim.Play("Fly");
            /*RaycastHit2D hit = Physics2D.Raycast((Vector2)manager.transform.position, testDirection.normalized , Mathf.Infinity, LayerMask.GetMask("Terrain"));
            if(hit.normal == Vector2.right || hit.normal == Vector2.left)
            {
                /*hit = Physics2D.Raycast(hit.point, new Vector2(-horizontalDirection, Mathf.Pow(-1, hitCount) * Mathf.Tan(Mathf.Deg2Rad * testAngle)).normalized , Mathf.Infinity, LayerMask.GetMask("Terrain"));
                direction = (hit.point - (Vector2)manager.transform.position).normalized;#1#
                manager.isFlip = true;
            }*/
            direction = testDirection;
        }

        public void OnUpdate()
        {
            detectTimer += Time.deltaTime;
            if (detectTimer >= 0.1f)
            {
                WallDetect();
            }
        }

        private void Flip()
        {
            horizontalDirection *= -1;
            manager.transform.localScale = new Vector3(-manager.transform.localScale.x, manager.transform.localScale.y, manager.transform.localScale.z);
            manager.isFlip = false;
        }

        private void WallDetect()
        {
            hits = Physics2D.BoxCastAll(manager.box.bounds.center, manager.box.size, 0, direction, 0.2f,
                LayerMask.GetMask("Terrain"));
            foreach(RaycastHit2D hit in hits)
            {
                if (hit.normal == Vector2.up)
                {
                    manager.TransitionState(States.HitDown);
                }
                else if (hit.normal == Vector2.down)
                {
                    manager.TransitionState(States.HitUp);
                }
                else
                {
                    manager.isFlip = true;
                }
            }
        }
        
        public void OnFixedUpdate()
        {
            manager.rb.position += direction * parameter.mutiCrashSpeed * Time.fixedDeltaTime;
        }

        public void OnExit()
        {

        }
    }

    public class GruzBurst : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;

        public GruzBurst(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("Burst");
            
        }

        public void OnUpdate()
        {

        }

        public void OnExit()
        {

        }
    }

    public class GruzDead : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;
        private float burstTimer;

        public GruzDead(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            manager._finalHitEffect.Play();
            AudioManager.Instance.StopBackgroundMusic();
            manager.anim.Play("Dead_charge");
            AudioManager.Instance.PlaySoundWithComplete("Enemy/GruzMother/boss_final_hit",1f, (audio) =>
            {
                manager.StartCoroutine(manager.ContinuousShake(16));
                AudioManager.Instance.PlaySound("Enemy/GruzMother/boss_gushing",false);
                manager._finalSmokeEffect.Play();
            });
            manager.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
            burstTimer = 0;
        }

        public void OnUpdate()
        {
            if(manager.anim.currentClip == "Dead_charge" && manager.anim.stateInfo.normalizedTime >= 14.5f)
            {
                manager.anim.Play("Dead_fly");
                manager.SpawnCoins();
                manager.gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
                manager._finalBurstEffect.Play();
                AudioManager.Instance.PlaySound("Enemy/GruzMother/boss_explode",false);
                AudioManager.Instance.PlaySound("Enemy/GruzMother/Boss Defeat",false);
                manager.rb.gravityScale = 1;
                manager.rb.AddForce(new Vector2(-manager.transform.localScale.x * parameter.deadForce, 5), ForceMode2D.Impulse);
            }
            
            if(manager.anim.currentClip == "Dead_fly")
            {
                RaycastHit2D hit = Physics2D.Raycast(manager.transform.position, Vector2.down, manager.box.size.y/2 + 0.1f, LayerMask.GetMask("Terrain"));
                if (hit.point != Vector2.zero)
                {
                    manager.anim.Play("Dead");
                }
            }

            if (manager.anim.currentClip == "Dead" && manager.anim.stateInfo.normalizedTime >= 1f)
            {
                burstTimer += Time.deltaTime;
                if (burstTimer > 1f)
                {
                    manager.TransitionState(States.Event1);
                }
            }
        }

        public void OnExit()
        {

        }
    }
    
    public class GruzHitUp : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;
        private GruzMutiCrash state;

        public GruzHitUp(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("HitUp");
            manager.myImpulse.GenerateImpulse();
            manager.audio.PlaySound("Enemy/GruzMother/big_fly_wall_hit",false);
            state = manager.states[States.Skill1] as GruzMutiCrash;
        }

        public void OnUpdate()
        {
            if (manager.anim.currentClip == "HitUp" && manager.anim.stateInfo.normalizedTime >= 1f)
            {
                if (state.isOn)
                {
                    state.hitPosition = manager.transform.position;
                    if (state.hitCount < 20)
                    {
                        state.hitCount++;
                        manager.TransitionState(States.Skill1);
                    }
                    else
                    {
                        state.isOn = false;
                        state.hitCount = 0;
                        manager.TransitionState(States.Idle);
                    }
                }
                else
                {
                    manager.TransitionState(States.Idle);
                }
            }
        }

        public void OnExit()
        {

        }
    }
    
    public class GruzHitDown : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;
        private GruzMutiCrash state;

        public GruzHitDown(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("HitDown");
            manager.myImpulse.GenerateImpulse();
            manager.audio.PlaySound("Enemy/GruzMother/big_fly_wall_hit",false);
            state = manager.states[States.Skill1] as GruzMutiCrash;
        }

        public void OnUpdate()
        {
            if (manager.anim.currentClip == "HitDown" && manager.anim.stateInfo.normalizedTime >= 1f)
            {
                if (state.isOn)
                {
                    state.hitPosition = manager.transform.position;
                    if (state.hitCount < 20)
                    {
                        state.hitCount++;
                        manager.TransitionState(States.Skill1);
                    }
                    else
                    {
                        state.isOn = false;
                        state.hitCount = 0;
                        manager.TransitionState(States.Idle);
                    }
                }
                else
                {
                    manager.TransitionState(States.Idle);
                }
            }
        }

        public void OnExit()
        {

        }
    }
    
    public class GruzHit : IState
    {
        private GruzMother manager;
        private GruzMotherParameter parameter;
        private GruzMutiCrash state;

        public GruzHit(GruzMother manager)
        {
            this.manager = manager;
            this.parameter = manager.Gr_parameter;
        }

        public void OnEnter()
        {
            manager.anim.Play("Hit");
            manager.myImpulse.GenerateImpulse();
            manager.audio.PlaySound("Enemy/GruzMother/big_fly_wall_hit",false);
            state = manager.states[States.Skill1] as GruzMutiCrash;
        }

        public void OnUpdate()
        {
            if (manager.anim.currentClip == "Hit" && manager.anim.stateInfo.normalizedTime >= 1f)
            {
                if (state.isOn)
                {
                    state.hitPosition = manager.transform.position;
                    if (state.hitCount < 20)
                    {
                        state.hitCount++;
                        manager.TransitionState(States.Skill1);
                    }
                    else
                    {
                        state.isOn = false;
                        state.hitCount = 0;
                        manager.TransitionState(States.Idle);
                    }
                }
                else
                {
                    manager.TransitionState(States.Idle);
                }
            }
        }

        public void OnExit()
        {

        }
    }
}
