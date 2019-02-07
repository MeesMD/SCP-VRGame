using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qt.AI
{
    public abstract class BaseState : MonoBehaviour
    {
        public StateMachine StateMachine { get; set; }
        
        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void Act();
        public abstract void Think();
    }
}
