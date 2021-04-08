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

        [Header("Stats")]

        public float moveSpeed = 5;
        public float runSpeed = 9;
        public float rotateSpeed = 5;

        [Header("States")]
        public bool running;

        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public float delta;

        public void Init()
        {
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDrag = 999;
            rigid.drag = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
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

            anim.applyRootMotion = false;
        }

        public void FixedTick(float d)
        {
            delta = d;

            rigid.drag = (moveAmount > 0) ? 0 : 4;
            float targetSpeed = moveSpeed;
            if (running)
                targetSpeed = runSpeed;

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

        void HandleMovementAnimations()
        {
            anim.SetFloat("vertical", moveAmount, 0.4f, delta);
        }
    }
}
