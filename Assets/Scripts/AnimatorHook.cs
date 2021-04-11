using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA
{

    public class AnimatorHook : MonoBehaviour
    {
        Animator anim;
        public StateManager states;
        public bool enableRootMotion = false;

        // Start is called before the first frame update
        public void Init(StateManager st)
        {
            states = st;
            anim = st.anim;
        }

        private void FixedUpdate()
        {
            if (states.canMove)
                return;

            anim.applyRootMotion = true;

            if (enableRootMotion)
            {
                states.rigid.drag = 2;
                Vector3 facingDirection = states.transform.forward.normalized;
                Debug.Log(facingDirection);
                states.rigid.AddForce(facingDirection * states.dashAttackForce, ForceMode.Impulse);
                /*states.rigid.drag = 0;
                float multiplier = 0.2f;

                Vector3 delta = hips.position - lastPosition;
                Debug.Log(hips.position);
                delta.y = 0;
                Vector3 v = (delta * multiplier) / (states.delta * 0.5f);
                states.rigid.velocity = v;
                lastPosition = hips.position;*/
            }

            
        }

    }
}

