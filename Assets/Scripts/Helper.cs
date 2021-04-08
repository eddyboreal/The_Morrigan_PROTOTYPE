using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    [Range(0,1)]
    public float vertical;

    public Animator anim;
    public string animationName;

    public bool playAnim = false;
    public bool enableRM;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        enableRM = !anim.GetBool("canMove");
        anim.applyRootMotion = enableRM;

        if (enableRM)
        {
            return;
        }
        else
        {
            if (playAnim)
            {
                if(vertical > 0.85)
                {
                    anim.CrossFade("dashAttack", 0.2f);
                }
                else
                {
                    anim.CrossFade(animationName, 0.2f);       
                }
                vertical = 0;
                playAnim = false;
            }
            anim.SetFloat("vertical", vertical);
        }
    }
}
