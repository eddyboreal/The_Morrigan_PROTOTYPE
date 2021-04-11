using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class StateManager : MonoBehaviour
    {
        [Header("Init")]
        public GameObject activeModel;

        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;
        public bool rb, rt, lb, lt, b, a, x, y;

        [Header("Stats")]

        [SerializeField]
        public float walkSpeed = 5;
        [SerializeField]
        public float jogSpeed = 7;
        [SerializeField]
        public float runSpeed = 9;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float actionDelay = 0.3f;

        public float dashAttackForce = 6;

        float _actionDelayET = 0;

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

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;


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
                rigid.velocity = Vector3.zero;
                a_hook.enableRootMotion = true;

                _actionDelayET += delta;
                
                if(_actionDelayET > 0.3f)
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

            
            if (!canMove)
                return;

            //anim.applyRootMotion = false;

            rigid.drag = (moveAmount > 0 || onGround) ? 0 : 4;
            
            float targetSpeed = SetSpeed();

            rigid.velocity = moveDir * (targetSpeed * moveAmount);

            Vector3 targetDir = moveDir;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;

            HandleMovementAnimations();

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

            if (rb && canMove && !isActing)
            {
                isActing = true;
                if (!running)
                    targetAnim = "DualWeapon";
                else
                    targetAnim = "dashAttack";
            }                                         

            if (string.IsNullOrEmpty(targetAnim))
                return;

            anim.CrossFade(targetAnim, 0.2f);
            /*inAction = true;
            canMove = false;*/

        }

        void HandleMovementAnimations()
        {
            anim.SetFloat("vertical", moveAmount, 0.1f, delta);
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
