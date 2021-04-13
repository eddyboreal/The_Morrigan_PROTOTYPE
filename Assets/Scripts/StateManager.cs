using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SA
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        [Space(10)]
        public GameObject activeModel;
        public Slider LifeSlider;
        public Slider StaminaSlider;
        public Slider BloodSlider;

        [Header("Inputs")]
        [Space(10)]
        [Range(-1,1)]
        public float vertical;
        [Range(-1, 1)]
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rb, rt, lb, lt, b, a, x, y;

        [Header("Stats")]
        [Space(10)]

        [SerializeField]
        public float walkSpeed = 5;
        [SerializeField]
        public float jogSpeed = 7;
        [SerializeField]
        public float runSpeed = 9;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float actionDelay = 0.3f;
        public float rememberInputTime = 0.3f;


        [Header("Attacks")]
        [Space(10)]

        public float dashAttackForce = 9;
        public float dashAttackStaminaCost = 30;

        [Space(10)]
        public float light_attack_1_force = 3;
        public float light_attack_1_stamina_cost = 20;

        [Space(10)]
        public float light_attack_2_force = 3;
        public float light_attack_2_stamina_cost = 20;

        [Space(10)]
        public float light_attack_3_force = 3;
        public float light_attack_3_stamina_cost = 20;

        [Space(10)]
        public float staminaRunSpeedMultiplier = 1f;
        public float staminaRegenSpeedMultiplier = 1f;

        [Header("Info")]
        [Space(10)]
        public int nextAction = 0;
        public float staminaCost;

        float _actionDelayET = 0;
        float _rememberInputTimeET = 0;

        /*[Header("EffectiveStats")]
        public float effectiveWalkSpeed = 5 * 0.3f;
        public float effectiveJogSpeed = 7 * 0.65f;
        public float effectiveRunSpeed = 9 * 1f;*/

        [Header("States")]
        public bool jogging;
        public bool running;
        public bool onGround;
        public bool inAction;
        public bool canMove;
        public bool isActing;
        public bool lockOn;

        [Header("Other")]
        public EnnemyTarget lockOntarget;


        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public CharacterStats stats;


        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoredLayers;

        public void Init()
        {
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);

            stats = GetComponent<CharacterStats>();

            gameObject.layer = 8;
            ignoredLayers = ~(1 << 9);
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found");
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }
            if (anim == null)
            {
                anim = activeModel.GetComponent<Animator>();
            }

            //anim.applyRootMotion = false;

        }

        public void FixedTick(float d)
        {
            delta = d;

            DetectActions();

            inAction = !anim.GetBool("canMove");
            canMove = anim.GetBool("canMove");
           
            if (inAction)
            {
                a_hook.enableRootMotion = true;

                _actionDelayET += delta;
                
                if(_actionDelayET > 0.5f)
                {
                    isActing = false;
                    vertical = 0.01f;
                    anim.SetFloat("vertical", vertical);
                    _actionDelayET = 0;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (running)
                {
                    float stam = stats.Stamina - delta * staminaRunSpeedMultiplier;
                    stats.Stamina = Mathf.Clamp(stam, 0, 100);
                }
                else
                {
                    float stam = stats.Stamina + delta * staminaRegenSpeedMultiplier;
                    stats.Stamina = Mathf.Clamp(stam, 0, 100);
                }

                StaminaSlider.value = stats.Stamina;
            }

            
            if (!canMove)
                return;

            rigid.drag = (moveAmount > 0 || onGround) ? 0 : 4;
            
            float targetSpeed = SetSpeed();

            if (onGround)
            {
                rigid.velocity = moveDir * (targetSpeed * moveAmount);
            }
                
            if (running)
                lockOn = false;

            Vector3 targetDir = (lockOn == false) ? moveDir : lockOntarget.transform.position - transform.position;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;

            anim.SetBool("lockon", lockOn);

            if (!lockOn)
                HandleMovementAnimations();
            else
                HandleLockOnAnimations(moveDir);

        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
        }

        public void DetectActions()
        {
            if (rb == false && rt == false && lb == false && lt == false)
                return;

            string targetAnim = null;

            if (canMove)
                nextAction = 0;

            if (rb && canMove && !isActing)
            {
                isActing = true;
                _rememberInputTimeET = 0;
                if (!running)
                {
                    if (nextAction == 0)
                    {
                        staminaCost = light_attack_1_stamina_cost;
                        targetAnim = "light_attack_1";
                        a_hook.Multiplier = light_attack_1_force;
                        anim.CrossFade(targetAnim, 0.2f);
                        nextAction = 1;
                    }
                }
                else
                {
                    Debug.Log('a');
                    targetAnim = "dashAttack";
                    staminaCost = dashAttackStaminaCost;
                    a_hook.Multiplier = dashAttackForce;
                    anim.CrossFade(targetAnim, 0.2f);
                }
                    
            }
            else if(!canMove && !running)
            {
                _rememberInputTimeET += delta;
                if(!canMove && _rememberInputTimeET >= rememberInputTime && rb)
                {
                    _rememberInputTimeET = 0;
                    if (nextAction == 0)
                    {
                        nextAction = 1;
                        a_hook.Multiplier = light_attack_2_force;
                        staminaCost = light_attack_2_stamina_cost;
                    }
                    else if (nextAction == 1) 
                    {
                        nextAction = 2;
                        a_hook.Multiplier = light_attack_3_force;
                        staminaCost = light_attack_3_stamina_cost;
                    }
                        
                    else if (nextAction == 2) 
                    {
                        nextAction = 0;
                        a_hook.Multiplier = light_attack_1_force;
                        staminaCost = light_attack_1_stamina_cost;
                    }
                        

                    anim.SetBool("AttackRecorded", true);
                }
            }

            if (string.IsNullOrEmpty(targetAnim))
                return;

            
            /*inAction = true;
            canMove = false;*/

        }

        void HandleMovementAnimations()
        {
            anim.SetFloat("vertical", moveAmount, 0.1f, delta);
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativDir = transform.InverseTransformDirection(moveDir);
            float hor = horizontal;
            float ver = vertical;

            if (hor < 0.3f && hor > -0.3f)
                hor = 0f;
            
            if (hor >= 0.3 && hor <= 0.51f)
                hor = 0.5f;
            if (hor > 0.51f)
                hor = 1f;

            if (hor <= -0.3f && hor >= -0.5f)
                hor = -0.5f;
            if (hor < -0.5f)
                hor = -1;

            if (ver <= 0.3f && ver >= 0.3f)
                ver = 0f;
            if (ver > 0.3f)
                ver = 1f;
            if (ver < -0.3f)
                ver = -1f;

            float h = relativDir.x * hor * Mathf.Sign(relativDir.x);
            float v = relativDir.z * ver * Mathf.Sign(relativDir.z);

            anim.SetFloat("vertical", v, 0.1f, delta);
            anim.SetFloat("horizontal", h, 0.1f, delta);
        }

        float SetSpeed()
        {
            float targetSpeed;

            if (!jogging)
                targetSpeed = walkSpeed;
            else
                targetSpeed = jogSpeed;

            if (running)
                targetSpeed = runSpeed;

            return targetSpeed;
        }

        public void getHit(float damages)
        {
            
            if ((stats.Life - damages) <= 0)
            {
                stats.Life = 0;
                anim.CrossFade("Death", 0.1f);
            }
            else
            {
                anim.CrossFade("Hit", 0.1f);
                stats.Life -= damages;
            }
            VisualStatUpdate();
        }

        public void ChangeStamina()
        {
            float stam = stats.Stamina - staminaCost;
            stats.Stamina = Mathf.Clamp(stam, 0, 100);
            StaminaSlider.value = stats.Stamina;
        }

        public void VisualStatUpdate()
        {
            LifeSlider.value = stats.Life;
            StaminaSlider.value = stats.Stamina;
            BloodSlider.value = stats.Blood;
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.3f;

            RaycastHit hit;
            if(Physics.Raycast(origin,dir,out hit, dis))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }

            return r;
        }
    }
}
