using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Weight {

    public float growTimer = 0;

    private void FixedUpdate()
    {
        growTimer += Time.fixedDeltaTime;
    }

}
