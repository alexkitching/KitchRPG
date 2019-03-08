using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KitchAI;

[RequireComponent(typeof(Camera))]
public class ClickOrderer : MonoBehaviour
{
    public KitchAgent a_targetAgent;
    private Camera camera;

    public LayerMask GroundMask;
    public LayerMask SelectableMask;

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

	void Start () 
    {
		
	}

	void Update ()
	{
	    if (Input.GetMouseButtonDown(1))
	    {
	        if (a_targetAgent == null)
	            return;

            CastRayFromMouse(OnMoveOrder, GroundMask);
	    }
        else if (Input.GetMouseButtonDown(0))
	    {
	        CastRayFromMouse(OnSelect, SelectableMask);
	    }
	}

    private delegate void RayHitCallback(bool a_success, RaycastHit a_hit);

    private void CastRayFromMouse(RayHitCallback a_callback, LayerMask a_layer)
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = camera.ScreenPointToRay(mousePos);
        RaycastHit hitInfo;
        bool hitSuccess = Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, a_layer);
        
        a_callback(hitSuccess, hitInfo);
    }

    private void OnMoveOrder(bool a_success, RaycastHit a_hit)
    {
        if (a_success == false)
            return;

        bool shiftPressed = Input.GetKey(KeyCode.LeftShift);
        a_targetAgent.MoveToPosition(a_hit.point, shiftPressed == false);
    }

    private void OnSelect(bool a_success, RaycastHit a_hit)
    {
        if (a_success)
        {
            GameObject go = a_hit.collider.gameObject;

            KitchAgent ka = go.GetComponent<KitchAgent>();

            if (ka)
            {
                a_targetAgent = ka;
                return;
            }
        }
        a_targetAgent = null;
    }

}
