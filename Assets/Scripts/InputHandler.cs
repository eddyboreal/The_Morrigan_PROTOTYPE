using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {

        float vertical;
        float horizontal;
        
        bool b_input;
        bool a_input;
        bool x_input;
        bool y_input;

        bool rb_input;
        bool rt_input;
        float rt_axis;
        bool lb_input;
        bool lt_input;
        float lt_axis;

        bool leftAxis_down;
        bool rightAxis_down;

        StateManager states;
        CameraManager camManager;

        float delta;

        // Start is called before the first frame update
        void Start()
        {
            states = GetComponent<StateManager>();
            states.Init();
            
            camManager = CameraManager.singleton;
            camManager.Init(this.transform);
        }

        private void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            //UpdateStates();
            states.FixedTick(Time.fixedDeltaTime);
            camManager.Tick(delta);
        }

        private void Update()
        {
            states.Tick(delta);
            UpdateStates();
        }

        void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            b_input = Input.GetButton("b_input");
            rb_input = Input.GetButton("RB");
            rt_input = Input.GetButton("RT");
            rt_axis = Input.GetAxis("RT");

            rightAxis_down = Input.GetButton("Lock");

        }

        void UpdateStates()
        {
            states.horizontal = horizontal;
            states.vertical = vertical;

            Vector3 v = vertical * camManager.transform.forward;
            Vector3 h = horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized;
            SetMoveAmount();

            if (b_input)
            {
                states.running = (states.moveAmount > 0);
            }
            else
            {
                states.running = false;
            }

            states.rb = rb_input;

            if (Input.GetButtonDown("Lock"))
            {
                states.lockOn = !states.lockOn;
                if (states.lockOntarget == null)
                {
                    states.lockOn = false;
                }
                camManager.lockonTarget = states.lockOntarget.transform;
                camManager.lockon = states.lockOn;
            }
        }

        void SetMoveAmount()
        {
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            m = Mathf.Clamp01(m);

            if (!states.lockOn)
            {
                
                if (m >= 0 && m <= 0.3f)
                {
                    m = 0.001f;
                    states.jogging = false;
                    states.anim.SetBool("jogging", false);
                }
                if (m > 0.3f)
                {
                    if (!states.running)
                    {
                        if (m <= 0.6f)
                        {
                            states.jogging = false;
                            states.anim.SetBool("jogging", false);
                            m = 0.3f;
                        }
                        else
                        {
                            states.jogging = true;
                            states.anim.SetBool("jogging", true);
                            m = 0.66f;
                        }
                    }
                    else
                    {
                        states.jogging = false;
                        states.anim.SetBool("jogging", false);
                        m = 1f;
                    }

                }
                states.moveAmount = m;
            }
            else
            {
                if (horizontal < 0.3f && horizontal > -0.3f && vertical < 0.3f && vertical > -0.3f)
                {
                    states.moveAmount = 0.0f;
                }
                else
                {
                    Debug.Log("aled");
                    states.moveAmount = m;
                }
            }
            
        }

    }

}

