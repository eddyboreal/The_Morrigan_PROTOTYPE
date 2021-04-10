using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class AnimatorHook : MonoBehaviour
    {
        Animator anim;
        StateManager states;

        // Start is called before the first frame update
        public void Init(StateManager st)
        {
            states = st;
            anim = st.anim;
        }

        private void OnAnimatorMove()
        {
            if (states.canMove)
                return;
            states.rigid.drag = 0;
            float multiplier = 1;

            Vector3 delta = anim.deltaPosition;
            delta.y = 0;
            Vector3 v = (delta * multiplier) / states.delta;
            states.rigid.velocity = v;
        }

    }
}

