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
        }


        // Update is called once per frame
        void Update()
        {
            delta = Time.deltaTime;
            camManager.Tick(delta);
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

            states.Tick(Time.deltaTime);
        }

    }

}

