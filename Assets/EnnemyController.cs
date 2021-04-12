using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyController : MonoBehaviour
{
    [Header("Init")]
    [Space(10)]
    public GameObject activeModel;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rigid;


    // Start is called before the first frame update
    void Start()
    {
        
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
