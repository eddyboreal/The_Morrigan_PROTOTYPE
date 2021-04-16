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
        public float Multiplier;

        // Start is called before the first frame update
        public void Init(StateManager st)
        {
            states = st;
            anim = st.anim;
        }       
        private void OnAnimatorMove()
        {
            if (enableRootMotion && !states.canMove)
            {
                Debug.Log("a");
                states.rigid.velocity = Vector3.zero;
                states.rigid.drag = 0;
                Vector3 delta = anim.deltaPosition;
                delta.y = 0;
                Vector3 v = (anim.deltaPosition * Multiplier) / states.delta;
                states.rigid.velocity = v;

            }
        }
    }
}

