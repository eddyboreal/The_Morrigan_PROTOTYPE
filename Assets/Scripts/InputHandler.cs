using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {

        float vertical;
        float horizontal;

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
            UpdateStates();
            states.FixedTick(Time.deltaTime);
            camManager.Tick(delta);
        }

        private void Update()
        {
            states.Tick(delta);
        }

        void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
        }

        void UpdateStates()
        {
            states.horizontal = horizontal;
            states.vertical = vertical;

            Vector3 v = vertical * camManager.transform.forward;
            Vector3 h = horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            states.moveAmount = Mathf.Clamp01(m);
        }

    }

}

