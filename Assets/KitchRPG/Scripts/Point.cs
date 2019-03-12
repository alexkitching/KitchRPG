using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [ContextMenu("Pin To Ground")]
    private void PinToGround()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 100, ~LayerMask.NameToLayer(Layers.GROUND)))
        {
            transform.position = hitInfo.point;
        }
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
#endif
}
