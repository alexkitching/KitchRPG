using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KitchAI
{
    interface Order
    {
        OrderStatus OnEnter(NavMeshAgent a_agent);
        OrderStatus Update(NavMeshAgent a_agent);
        void OnExit(NavMeshAgent a_agent, OrderStatus a_status);
    
#if AgentOrderDebug
        void DrawGizmosActive(NavMeshAgent a_agent);
        void DrawGizmosQueued(NavMeshAgent a_agent, Vector3 a_startPosition);
        Vector3 GetTargetPos();
#endif
    }
    
    public enum OrderStatus
    {
        Failure,
        Running,
        Success
    }
    
    public class MoveToTargetOrder : Order
    {
        private Transform target;
        private float speed;
    
        private float stoppingDistance = 0.1f;
    
        public MoveToTargetOrder(Transform a_target, float a_speed)
        {
            target = a_target;
            speed = a_speed;
        }
    
        OrderStatus Order.OnEnter(NavMeshAgent a_agent)
        {
            a_agent.isStopped = false;
            a_agent.speed = speed;
            return OrderStatus.Success;
        }
    
        OrderStatus Order.Update(NavMeshAgent a_agent)
        {
            Vector3 targetPos = target.position;
            a_agent.SetDestination(targetPos);
    
            if (a_agent.remainingDistance < stoppingDistance)
                return OrderStatus.Success;
    
            return OrderStatus.Running;
        }
    
#if AgentOrderDebug
        void Order.DrawGizmosActive(NavMeshAgent a_agent)
        {
            Color color = Color.green;
            Debug.DrawLine(a_agent.transform.position, target.position, color);
        }
    
        void Order.DrawGizmosQueued(NavMeshAgent a_agent, Vector3 a_startPosition)
        {
            Color color = Color.yellow;
            Debug.DrawLine(a_startPosition, target.position, color);
        }
    
        Vector3 Order.GetTargetPos()
        {
            return target.position;
        }
    
#endif
    
        void Order.OnExit(NavMeshAgent a_agent, OrderStatus a_status)
        {
            a_agent.isStopped = true;
        }
    }
    
    public class MoveToPositionOrder : Order
    {
        private Vector3 target;
        private float speed;
    
        private float stoppingDistance = 0.5f;
    
        public MoveToPositionOrder(Vector3 a_targetPosition, float a_speed)
        {
            target = a_targetPosition;
            speed = a_speed;
        }
    
        OrderStatus Order.OnEnter(NavMeshAgent a_agent)
        {
            a_agent.isStopped = false;
            a_agent.speed = speed;
            a_agent.SetDestination(target);
    
            return OrderStatus.Success;
        }
    
        OrderStatus Order.Update(NavMeshAgent a_agent)
        {
            if (a_agent.remainingDistance < stoppingDistance)
                return OrderStatus.Success;
    
            return OrderStatus.Running;
        }
    
        void Order.OnExit(NavMeshAgent a_agent, OrderStatus a_status)
        {
            a_agent.isStopped = true;
        }
    
#if AgentOrderDebug
        void Order.DrawGizmosActive(NavMeshAgent a_agent)
        {
            Color color = Color.green;
            Debug.DrawLine(a_agent.transform.position, target, color);
        }
    
        void Order.DrawGizmosQueued(NavMeshAgent a_agent, Vector3 a_startPosition)
        {
            Color color = Color.yellow;
            Debug.DrawLine(a_startPosition, target, color);
        }
    
        Vector3 Order.GetTargetPos()
        {
            return target;
        }
    
#endif
    }
}

