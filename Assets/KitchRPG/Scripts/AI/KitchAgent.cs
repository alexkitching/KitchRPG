using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KitchAI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class KitchAgent : MonoBehaviour
    {
        private NavMeshAgent _internalAgent;
        private QueuedOrderRunner _orderRunner;

        public float speed = 5;

#region Mono

        private void Awake()
        {
            _internalAgent = GetComponent<NavMeshAgent>();
            _orderRunner = new QueuedOrderRunner(3);
        }

        private void Start () 
        {
		
        }
	

        private void Update () 
        {
            _orderRunner.Update(_internalAgent);
        }

#endregion

        public void MoveToTarget(Transform a_target)
        {
            MoveToTargetOrder order = new MoveToTargetOrder(a_target, speed);
            _orderRunner.CancelAllOrders(_internalAgent);
            _orderRunner.QueueOrder(order);
        }

        public void MoveToPosition(Vector3 a_position, bool a_override)
        {
            MoveToPositionOrder order = new MoveToPositionOrder(a_position, speed);

            if (a_override)
            {
                _orderRunner.CancelAllOrders(_internalAgent);
            }

            _orderRunner.QueueOrder(order);
        }
    }
}

