using System;
using System.Collections.Generic;
using UnityEngine;

namespace SH.Utils
{
    public class MainThreadQueue : MonoBehaviour
    {
        public static MainThreadQueue Inst { get; private set; }

        private static Queue<MainThreadAction> _actions = new();

        private void Awake()
        {
            Inst = this;
        }

        private void Update()
        {
            if (_actions.Count > 0)
            {
                int numActions = _actions.Count;
                while (numActions > 0)
                {
                    var curr = _actions.Dequeue();
                    curr.action.Invoke(curr.data);
                    numActions--;
                }
            }
        }

        public static void Add(MainThreadAction action)
        {
            _actions.Enqueue(action);
        }

        public static void Add(Action<object> action, object data = null)
        {
            Add(new MainThreadAction(action, data));
        }
    }

    public struct MainThreadAction
    {
        public Action<object> action;
        public object data;

        public MainThreadAction(Action<object> action, object data)
        {
            this.action = action;
            this.data = data;
        }
    }
}