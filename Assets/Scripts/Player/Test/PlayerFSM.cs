using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Enemy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerFSM : MonoBehaviour
    {
        public Parameter parameter;
        public AnimationController anim;
        public Rigidbody2D rb;
        public Attack attackArea;
        public CharacterEffect characterEffect;
        public CinemaShaking cinemaShaking;
        public Invincibility invincibility;
        public AudioSource footStep;
        public AudioSource fall;
        public Vector2 attackDirection;
        public Vector3 lastGroundedPosition;
        public Vector3 lastGroundedScale;
        
        public bool canMove = true;
        public bool canAttack = true;
        public bool canJump = true;
        public bool canDash = true;
        public bool changeDirection;
        private bool isFacingRight;
        public bool isOnGround;
        
        public float x;
        public float y;
        public float fallSpeed;
        
        public float lastSlashTime;
        public float lastDashTime;

        public int currentHealth = 5;
        public int slashCount;
        public int jumpCount;
        
        private Dictionary<States,IState> states;
        private IState currentState;
        
        public void Awake()
        {
            anim = GetComponent<AnimationController>();
            rb = GetComponent<Rigidbody2D>();
            attackArea = GetComponentInChildren<Attack>();
            characterEffect = GetComponentInChildren<CharacterEffect>();
            cinemaShaking = GetComponent<CinemaShaking>();
            invincibility = GetComponent<Invincibility>();
            parameter = new Parameter();
            
            states = new Dictionary<States, IState>
            {
                { States.Idle, new IdleState(this) },
                { States.Walk1, new Walk1State(this) },
                { States.Walk2, new Walk2State(this) },
                { States.Walk3, new Walk3State(this) },
                { States.Rotate , new RotateState(this) },
                /*{ States.LookUp , new LookUpState(this) },
                { States.LookDown , new LookDownState(this) },*/
                { States.Attack , new AttackState(this) },
                { States.Jump , new JumpState(this) },
                { States.Fall , new FallState(this) },
                { States.Land , new LandState(this) },
                { States.Dash , new DashState(this) },
                { States.Hurt , new HurtState(this) },
                { States.Dead , new DeadState(this) },
            };
            currentState = states[States.Idle];
        }

        // Start is called before the first frame update
        void Start()
        {
            StopInput();
            /*InputManager.Instance.attack.performed += (context) =>
            {
                if (canAttack)
                {
                    TransitionState(States.Attack);
                }
            };
            InputManager.Instance.jump.performed += (context) =>
            {
                if (canJump)
                {
                    TransitionState(States.Jump);
                }
            };
            InputManager.Instance.dash.performed += (context) =>
            {
                if (canDash)
                {
                    TransitionState(States.Dash);
                }
            };*/
        }

        // Update is called once per frame
        void Update()
        {
            if (canMove)
            {
                /*x = InputManager.Instance.Horizontal.ReadValue<float>();
                y = InputManager.Instance.Vertical.ReadValue<float>();*/
            }
            
            if (x < 0)
            {
                isFacingRight = false;
                if (transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    changeDirection = true;
                }
                else
                {
                    changeDirection = false;
                }
            }

            if (x > 0)
            {
                isFacingRight = true;
                if (transform.localScale.x > 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    changeDirection = true;
                }
                else
                {
                    changeDirection = false;
                }
            }
            
            if (Time.time >= lastSlashTime + parameter.maxComboTime && slashCount != 0)
            {
                slashCount = 0;
            }

            if (Time.time >= lastDashTime + parameter.dashIntervalTime)
            {
                canDash = true;
            }
            currentState.OnUpdate();
        }

        public void FixedUpdate()
        {
            currentState.OnFixedUpdate();
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Grouding(collision, false);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            Grouding(collision, false);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            Grouding(collision, true);
        }

        public void TransitionState(States newState)
        {
            if(currentState != null)
            {
                currentState.OnExit();
                currentState = states[newState];
                currentState.OnEnter();
            }
        }
        
        public void Move()
        {
            if (x < 0)
            {
                rb.position -= new Vector2(parameter.moveSpeed * Time.fixedDeltaTime, 0f);
            }
            
            if (x > 0)
            {
                rb.position += new Vector2(parameter.moveSpeed * Time.fixedDeltaTime, 0f);
            }
        }
        
        public void SlashAndDetect(Attack.AttackType attackType)
        {
            List<Collider2D> colliders = new List<Collider2D>();
            attackArea.Play(attackType, ref colliders);
            foreach (Collider2D col in colliders) 
            {
                print(col.name);
                //击中敌人
                if (col.gameObject.layer == LayerMask.NameToLayer("EnemyDetector"))
                {
                    Debug.Log(1);
                    EnemyFSM enemy = col.GetComponentInParent<EnemyFSM>();
                    if (enemy != null && enemy.enabled)
                    {
                        enemy.Hurt(parameter.slashDamage, transform);
                        //Recoil
                        if (attackType == Attack.AttackType.DownSlash)
                        {
                            jumpCount = 1;
                            AddDownRecoilForce();
                        }
                        /*else
                        {
                            StartCoroutine(AddRecoilForce());
                        }*/
                    }
                    // 处理完当前命中后继续检测其它命中目标（原来是 break，会导致只命中一个）
                    continue;
                }
                
                //击中陷阱
                if (col.gameObject.layer == LayerMask.NameToLayer("DamagePlayer"))  
                {
                    if (attackType == Attack.AttackType.DownSlash)
                    {
                        AddDownRecoilForce();
                    }
                    // 处理完当前命中后继续检测其它命中目标
                    continue;
                }

                //击中可破坏物
                Breakable breakable = col.GetComponent<Breakable>();
                if (col.gameObject.layer == LayerMask.NameToLayer("Interactive Object") && breakable != null)
                {
                    breakable.Hurt(parameter.slashDamage, transform);
                }

            }
        }
        private void AddDownRecoilForce()  //下劈击退
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); //重置y轴速度
            rb.AddForce(Vector2.up * parameter.downRecoilForce, ForceMode2D.Impulse);
        }
        
        IEnumerator AddRecoilForce()  //普通击退
        {
            /*InputManager.Instance.SetActionInput("Horizontal", false);
            if (isFacingRight)
            {
                rb.AddForce(Vector2.left * parameter.recoilForce, ForceMode2D.Impulse);

            }*/
            yield return new WaitForSeconds(0.2f);
            /*InputManager.Instance.SetActionInput("Horizontal", true);*/
        }
        
        private void Grouding(Collision2D col, bool exitGround)
        {
            if (exitGround) //离地
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                {
                    isOnGround = false;
                    lastGroundedPosition = transform.position;
                    lastGroundedScale = transform.localScale;
                }
            }
            else
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("Terrain") && !isOnGround && Vector2.Angle(col.contacts[0].normal,Vector2.up) < 80)  //落地
                {
                    characterEffect.DoEffect(CharacterEffect.EffectType.FallTrail, true);

                    isOnGround = true;

                    jumpCount = 2;

                    /*JumpCancel();*/
                }
                else if (col.gameObject.layer == LayerMask.NameToLayer("Terrain") && !isOnGround && Vector2.Angle(col.contacts[0].normal,Vector2.down) < 80) //顶头
                {
                    /*JumpCancel();*/
                }

            }
            /*anim.SetBool("isOnGround", isOnGround);*/

        }
        
        #region 输入

        public void StopInput()
        {
            /*InputManager.Instance.SetMapInput("Player", false);*/
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    
        public void ResumeInput()
        {
            /*InputManager.Instance.SetMapInput("Player", true);*/
        }

        #endregion
    

        #region 事件

        public void HardLand()
        {
            StopInput();
            characterEffect.DoEffect(CharacterEffect.EffectType.BurstRocks, true);
        }
        
        public void TakeDamage(int damage, Transform attackPosition)
        {
            if (invincibility.isInvincible) return;
            currentHealth -= damage;
            attackDirection = transform.position - attackPosition.position;
            StartCoroutine(invincibility.SetInvincibility());//图像闪烁
            FindObjectOfType<Health>().Hurt();//UI扣血
            TransitionState(States.Hurt);
        }

        public void Dead()
        {
            /*FindObjectOfType<StartGame>().operation.allowSceneActivation = true;*/
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            UIManager.Instance.ClosePanel("KnightPanel");
        }
        #endregion
    }
}