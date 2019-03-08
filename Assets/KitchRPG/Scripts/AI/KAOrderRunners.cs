using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace KitchAI
{
    internal class QueuedOrderRunner
    {
        private LinkedList<Order> orders;
        private bool hasOrders = false;
        private int maxQueuedOrders = 2;
    
        private Order currentOrder;
    
        public QueuedOrderRunner(int a_maxQueuedOrders)
        {
            maxQueuedOrders = a_maxQueuedOrders;
            orders = new LinkedList<Order>();
        }
    
        public void Update(NavMeshAgent a_agent)
        {
            if (hasOrders == false)
                return;
    
    #if AgentOrderDebug
            if (orders.Count > 0)
            {
                LinkedListNode<Order> node = orders.First;
                node.Value.DrawGizmosActive(a_agent);
                while (node.Next != null)
                {
                    LinkedListNode<Order> nextNode = node.Next;
                    nextNode.Value.DrawGizmosQueued(a_agent, node.Value.GetTargetPos());
                    node = nextNode;
                }
            }
    
    #endif
    
            if (currentOrder == null)
            {
                if (orders.Count > 0 && TryDequeueOrder(a_agent) == false) // Dequeue Failed Recheck Count
                {
                    if (orders.Count <= 0)
                    {
                        hasOrders = false;
                    }
                }
            }
            else
            {
                OrderStatus status = currentOrder.Update(a_agent);
    
                if (status == OrderStatus.Running)
                    return;
    
                currentOrder.OnExit(a_agent, status);
                currentOrder = null;
                orders.RemoveFirst();
            }
        }
    
        public void QueueOrder(Order a_order)
        {
            if (orders.Count - 1 > maxQueuedOrders)
                return;
    
            orders.AddLast(new LinkedListNode<Order>(a_order));
    
            hasOrders = true;
        }
    
        public void CancelCurrentOrder(NavMeshAgent a_agent)
        {
            if (currentOrder == null) return;
    
            currentOrder.OnExit(a_agent, OrderStatus.Failure);
            currentOrder = null;
            orders.RemoveFirst();
    
            if (orders.Count <= 0)
            {
                hasOrders = false;
            }
        }
    
        public void CancelAllOrders(NavMeshAgent a_agent)
        {
            CancelCurrentOrder(a_agent);
    
            if (hasOrders)
            {
                orders.Clear();
                hasOrders = false;
            }
        }
    
        private bool TryDequeueOrder(NavMeshAgent a_agent)
        {
            Order newOrder = orders.First.Value;
    
            OrderStatus status = newOrder.OnEnter(a_agent);
    
            if (status == OrderStatus.Success) 
            {
                currentOrder = newOrder;
                return true;
            }
    
            return false;
        }
    }
}
