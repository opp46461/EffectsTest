using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public Transform point;
    public Move move;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (point.localPosition.x >= 52.5f)
        {
            if (move)
            {
                move.enabled = true;
            }
            Destroy(this);
        }
    }
}
