using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyScriptLinker : MonoBehaviour
{
    public bool prayer;
    // Start is called before the first frame update
    void Start()
    {
        if (prayer)
        {
            GetComponent<Animator>().SetBool("prayer", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckHit()
    {
        GetComponentInParent<EnnemyController>().CheckHit();
    }
}
