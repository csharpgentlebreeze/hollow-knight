using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region 玩家
namespace Player
{
    public enum States
    {
        Idle,
        Walk1,
        Walk2,
        Walk3,
        Rotate,
        LookUp,
        LookDown,
        Attack,
        Jump,
        Fall,
        Land,
        Dash,
        Hurt,
        Dead,
    }
    
    public class IdleState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        public IdleState(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        public void OnEnter()
        {
            manager.anim.Play("Idle");
        }

        // Update is called once per frame
        public void OnUpdate()
        {
            if(!manager.isOnGround && manager.rb.velocity.y < -2.5f)
            {
                manager.TransitionState(States.Fall);
                return;
            }
            if (manager.x != 0)   //触发事件
            {
                if (manager.changeDirection) //监护条件
                {
                    manager.TransitionState(States.Rotate);
                } 
                else
                {
                    manager.TransitionState(States.Walk1);
                }
            }
            /*if(manager.y > 0) 
            { 
                manager.TransitionState(States.LookUp);
            } 
            else if (manager.y < 0) 
            {
                manager.TransitionState(States.LookDown);
            }*/
        }

        public void OnExit()
        {
            
        }
    }

    public class Walk1State : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        
        public Walk1State(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            manager.anim.Play("Walk1");
        }

        public void OnUpdate()
        {
            if(!manager.isOnGround && manager.rb.velocity.y < -2.5f)
            {
                manager.TransitionState(States.Fall);
                return;
            }
            if (manager.anim.IsEnd())
            {
                manager.TransitionState(States.Walk2);
            }
        }

        public void OnFixedUpdate()
        {
            manager.Move();
        }

        public void OnExit()
        {
            
        }
    }

    public class Walk2State : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        
        public Walk2State(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            manager.anim.Play("Walk2");
            AudioManager.Instance.PlayAudio(manager.footStep);
        }

        public void OnUpdate()
        {
            if(!manager.isOnGround && manager.rb.velocity.y < -2.5f)
            {
                manager.TransitionState(States.Fall);
                return;
            }
            if (manager.x == 0)
            {
                manager.TransitionState(States.Walk3);
            }
        }

        public void OnFixedUpdate()
        {
            manager.Move();
        }

        public void OnExit()
        {
            AudioManager.Instance.PauseSound(manager.footStep);
        }
    }
    
    public class Walk3State : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        
        public Walk3State(PlayerFSM manager) 
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            manager.anim.Play("Walk3");
        }

        public void OnUpdate()
        {
            if(!manager.isOnGround && manager.rb.velocity.y < -2.5f)
            {
                manager.TransitionState(States.Fall);
                return;
            }
            
            if (manager.changeDirection)
            {
                manager.TransitionState(States.Rotate);
                return;
            }
            
            if (manager.x != 0)
            {
                manager.TransitionState(States.Walk1);
                return;
            }
            
            if (manager.anim.IsEnd())
            {
                manager.TransitionState(States.Idle);
            }
        }

        public void OnFixedUpdate()
        {
            manager.Move();
        }

        public void OnExit()
        {
            
        }
    }
    
    
    public class RotateState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        
        public RotateState(PlayerFSM manager) 
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            manager.anim.Play("Rotate");
        }

        public void OnUpdate()
        {
            if (manager.anim.IsEnd())
            {
                if (manager.x != 0)
                {
                    manager.TransitionState(States.Walk2);
                }
                else
                {
                    manager.TransitionState(States.Idle);
                }
            }
        }
        
        public void OnFixedUpdate()
        {
            manager.Move();
        }

        public void OnExit()
        {
            
        }
    }
    
    public class LookUpState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        
        public LookUpState(PlayerFSM manager) 
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            manager.anim.Play("LookUp");
        }

        public void OnUpdate()
        {
            if (manager.y == 0)
            {
                manager.TransitionState(States.Idle);
            }
        }
        
        public void OnFixedUpdate()
        {
            manager.Move();
        }

        public void OnExit()
        {
            
        }
    }

    public class LookDownState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        
        public LookDownState(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            manager.anim.Play("LookDown");
        }

        public void OnUpdate()
        {
            if (manager.y == 0)
            {
                manager.TransitionState(States.Idle);
            }
        }
        
        public void OnFixedUpdate()
        {
            manager.Move();
        }

        public void OnExit()
        {
            
        }
    }

    public class AttackState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;

        public AttackState(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            manager.canAttack = false;
            if (Time.time >= manager.lastSlashTime + parameter.slashIntervalTime)
            {
                manager.lastSlashTime = Time.time;
                if (manager.y > 0)
                {
                    manager.SlashAndDetect(Attack.AttackType.Upslash);
                    manager.anim.Play("UpAttack");

                }
                else if (!manager.isOnGround && manager.y < 0)
                {
                    manager.SlashAndDetect(Attack.AttackType.DownSlash);
                    manager.anim.Play("DownAttack");
                }
                else
                {
                    manager.slashCount++;
                    switch (manager.slashCount)
                    {
                        case 1:
                            manager.SlashAndDetect(Attack.AttackType.Slash);
                            manager.anim.Play("Attack");
                            break;
                        case 2:
                            manager.SlashAndDetect(Attack.AttackType.AltSlash);
                            manager.anim.Play("AttackTwice");
                            manager.slashCount = 0;
                            break;
                    }
                }
            }
        }

        public void OnUpdate()
        {
            if (manager.anim.IsEnd())
            {
                if(!manager.isOnGround && manager.rb.velocity.y < -2.5f)
                {
                    manager.TransitionState(States.Fall);
                    return;
                }
                if (manager.x != 0)
                {
                    manager.TransitionState(States.Walk2);
                }
                else
                {
                    manager.TransitionState(States.Idle);
                }
            }
        }

        public void OnFixedUpdate()
        {
            manager.Move();
        }

        public void OnExit()
        {
            manager.canAttack = true;
        }
    }
    
    public class JumpState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        private float timer;
        
        public JumpState(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            timer = 0;
            if (manager.jumpCount == 2)
            {
                if (manager.isOnGround == false) return;
                manager.anim.Play("Jump");
                manager.rb.velocity = new Vector2(manager.rb.velocity.x, 0);
                manager.rb.AddForce(new Vector2(0, parameter.jumpForce), ForceMode2D.Impulse);
                AudioManager.Instance.PlaySound("Knight/hero_jump", false);
                manager.jumpCount--;
            }
            /*else if(manager.jumpCount == 1)
            {
                manager.anim.Play("DoubleJump");
                manager.rb.velocity = new Vector2(manager.rb.velocity.x, 0);
                manager.rb.AddForce(new Vector2(0, parameter.jumpForce), ForceMode2D.Impulse);
                AudioManager.Instance.PlaySound("Knight/hero_wings", false);
                manager.jumpCount--;
            }*/
        }

        public void OnUpdate()
        {
            timer += Time.deltaTime;
            /*if (timer > 1f || InputManager.Instance.jump.IsPressed() == false)
            {
                manager.TransitionState(States.Fall);
            }*/
        }
        
        public void OnFixedUpdate()
        {
            manager.rb.AddForce(new Vector2(0, parameter.jumpForce));
            manager.Move();
        }

        public void OnExit()
        {
            
        }
    }
    
    public class FallState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        private AudioSource sound;
        private float timer;
        private bool trigger;
        
        public FallState(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            manager.anim.Play("Fall");
            timer = 0f;
            trigger = true;
            manager.fallSpeed = 0f;
        }

        public void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= 1.5f)
            {
                if (trigger)
                {
                    AudioManager.Instance.PlayAudio(manager.fall);
                    trigger = false;
                }
            }
            
            if (manager.isOnGround)
            {
                manager.TransitionState(States.Land);
            }

            if (manager.rb.velocity.y < manager.fallSpeed)
            {
                manager.fallSpeed = manager.rb.velocity.y;
            }
        }
        
        public void OnFixedUpdate()
        {
            manager.Move();
        }

        public void OnExit()
        {
            AudioManager.Instance.PauseSound(manager.fall);
        }
    }
    
    public class LandState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        
        public LandState(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            if(manager.fallSpeed < -25f)
            {
                manager.anim.Play("HardLand");
                AudioManager.Instance.PlaySound("Knight/hero_land_hard", false);
            }
            else
            {
                manager.anim.Play("SoftLand");
                AudioManager.Instance.PlaySound("Knight/hero_land_soft", false);
            }
        }

        public void OnUpdate()
        {
            if (manager.x != 0)
            {
                manager.TransitionState(States.Walk2);
                return;
            }
            if (manager.anim.IsEnd())
            {
                manager.TransitionState(States.Idle);
            }
        }
        
        public void OnFixedUpdate()
        {
            if (manager.anim.currentClip == "SoftLand")
            {
                manager.Move();
            }
        }

        public void OnExit()
        {
            
        }
    }
    
    public class DashState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        private Vector2 destination;
        private bool trigger;
        
        public DashState(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            manager.characterEffect.DoEffect(CharacterEffect.EffectType.Dash,true);
            manager.rb.gravityScale = 0f;
            manager.rb.velocity = Vector2.zero;
            manager.lastDashTime = Time.time;
            trigger = true;
            manager.canMove = false;
            manager.canDash = false;
            manager.canAttack = false;
            manager.canJump = false;
            
            /*if(manager.fallSpeed < -25f)
            {*/
                manager.anim.Play("DashWhite");
                AudioManager.Instance.PlaySound("Knight/hero_dash", false);
            /*}
            else
            {
                manager.anim.Play("DashBlack");
                AudioManager.Instance.PlaySound("Knight/hero_land_soft", false);
            }*/
        }

        public void OnUpdate()
        {
            if (trigger)
            {
                destination = new Vector2(manager.rb.position.x - manager.transform.localScale.x * manager.anim.stateInfo.length * parameter.dashSpeed, manager.rb.position.y);
                trigger = false;
            }
            if (manager.anim.IsEnd())
            {
                if(!manager.isOnGround && manager.rb.velocity.y < -2.5f)
                {
                    manager.TransitionState(States.Fall);
                    return;
                }
                if(manager.x != 0)
                {
                    manager.TransitionState(States.Walk2);
                }
                else
                {
                    manager.TransitionState(States.Idle);
                }
            }
        }
        
        public void OnFixedUpdate()
        {
            manager.rb.position = Vector2.MoveTowards(manager.rb.position, destination, parameter.dashSpeed * Time.fixedDeltaTime);
        }

        public void OnExit()
        {
            manager.rb.gravityScale = 1.5f;
            manager.canMove = true;
            manager.canAttack = true;
            manager.canJump = true;
        }
    }
    
    public class HurtState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        private float timer;
        
        public HurtState(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            AudioManager.Instance.PlaySound("Knight/hero_damage_less_harsh", false);
            if (manager.currentHealth <= 0)
            {
                manager.TransitionState(States.Dead);
            }
            if (manager.attackDirection.x > 0)
            {
                manager.rb.velocity = new Vector2(1, 1) * parameter.hurtForce; //向右击飞
                manager.transform.localScale = new Vector3(1, 1, 1);//强制向左转向
            }
            else
            {
                manager.rb.velocity = new Vector2(-1, 1) * parameter.hurtForce;//向左击飞
                manager.transform.localScale = new Vector3(-1, 1, 1);//强制向右转向
            }
            timer = 0f;
            manager.anim.Play("TakeDamage");
            manager.cinemaShaking.CinemaShake();
            manager.characterEffect.DoEffect(CharacterEffect.EffectType.HitL, true);
            manager.characterEffect.DoEffect(CharacterEffect.EffectType.HitR, true);
            
        }

        public void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer > 0.5f)
            {
                Time.timeScale = 1;
                manager.TransitionState(States.Idle);
            }
        }
        
        public void OnFixedUpdate()
        {
            
        }

        public void OnExit()
        {
            
        }
    }
    
    public class DeadState : IState
    {
        private PlayerFSM manager;
        private Parameter parameter;
        private float timer;
        
        public DeadState(PlayerFSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }
        
        public void OnEnter()
        {
            manager.anim.Play("Dead");
            EventManager.Instance.EventTrigger("PlayerDead");
            Physics2D.IgnoreLayerCollision(7,9,false);
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
#endregion