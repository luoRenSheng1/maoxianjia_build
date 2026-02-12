/*
 * Copyright (c) 2016 Made With Monster Love (Pty) Ltd
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included 
 * in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using Engine;
using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace MonsterLove.StateMachine
{
	public enum StateTransition
	{
		Safe,
		Overwrite,
	}

    public enum States
    {
        Idle,   // 空闲
        Run,    // 行走
        Attack, // 攻击
        Patrol, // 巡逻
        Die,    // 死亡
    };

    public interface IStateMachine
	{
		MapMoveObject Component { get; }
		StateMapping CurrentStateMap { get; }
		bool IsInTransition { get; }
	}

    public class StateMachineDefine
    {
        public enum ActionType
        {
            Enter,
            Exit,
            Finally,
            Update,
            LateUpdate,
            FixedUpdate,
        };

        private static Dictionary<int, Dictionary<int, string>> s_mapActionString = new Dictionary<int, Dictionary<int, string>>();

        public static string GetActionString(States nState, StateMachineDefine.ActionType nAction)
        {
            Dictionary<int, string> value1 = null;

            if (!s_mapActionString.TryGetValue((int)nState, out value1))
            {
                value1 = new Dictionary<int, string>();
                s_mapActionString.Add((int)nState, value1);
            }

            if (value1 == null)
            {
                return string.Empty;
            }

            string value2 = null;

            if (!value1.TryGetValue((int)nAction, out value2))
            {
                value2 = nState.ToString() + nAction.ToString();
                value1.Add((int)nAction, value2);
            }

            return value2;
        }
    }

    public class StateMachine : IStateMachine
	{
		public event Action<States> Changed;

		private StateMachineRunner engine;
		private MapMoveObject component;

		private StateMapping lastState;
		private StateMapping currentState;
		private StateMapping destinationState;

		private Dictionary<States, StateMapping> stateLookup;

		//private readonly string[] ignoredNames = new[] { "add", "remove", "get", "set" };

		private bool isInTransition = false;
		private IEnumerator currentTransition;
		private IEnumerator exitRoutine;
		private IEnumerator enterRoutine;
		private IEnumerator queuedChange;

		public StateMachine(StateMachineRunner engine, MapMoveObject component)
		{
			this.engine = engine;
			this.component = component;
			Type t = typeof(MapMoveObject);

			//Define States
			stateLookup = new Dictionary<States, StateMapping>();
            string strTmp = null;
            MethodInfo methodTmp = null;
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (States state in Enum.GetValues(typeof(States)))
            {
                var targetState = new StateMapping(state);
                stateLookup.Add(targetState.state, targetState);

                if (targetState == null)
                {
                    continue;
                }

                // Enter
                strTmp = StateMachineDefine.GetActionString(state, StateMachineDefine.ActionType.Enter);
                methodTmp = t.GetMethod(strTmp, bindingFlags);

                if (methodTmp != null)
                {
                    if (methodTmp.ReturnType == typeof(IEnumerator))
                    {
                        targetState.hasEnterRoutine = true;
                        targetState.EnterRoutine = CreateDelegate<Func<IEnumerator>>(methodTmp, component);
                    }
                    else
                    {
                        targetState.hasEnterRoutine = false;
                        targetState.EnterCall = CreateDelegate<Action>(methodTmp, component);
                    }
                }

                // Exit
                strTmp = StateMachineDefine.GetActionString(state, StateMachineDefine.ActionType.Exit);
                methodTmp = t.GetMethod(strTmp, bindingFlags);

                if (methodTmp != null)
                {
                    if (methodTmp.ReturnType == typeof(IEnumerator))
                    {
                        targetState.hasExitRoutine = true;
                        targetState.ExitRoutine = CreateDelegate<Func<IEnumerator>>(methodTmp, component);
                    }
                    else
                    {
                        targetState.hasExitRoutine = false;
                        targetState.ExitCall = CreateDelegate<Action<object>>(methodTmp, component);
                    }
                }

                // "Finally"
                strTmp = StateMachineDefine.GetActionString(state, StateMachineDefine.ActionType.Finally);
                methodTmp = t.GetMethod(strTmp, bindingFlags);

                if (methodTmp != null)
                {
                    targetState.Finally = CreateDelegate<Action>(methodTmp, component);
                }

                // "Update"
                strTmp = StateMachineDefine.GetActionString(state, StateMachineDefine.ActionType.Update);
                methodTmp = t.GetMethod(strTmp, bindingFlags);

                if (methodTmp != null)
                {
                    targetState.Update = CreateDelegate<Action>(methodTmp, component);
                }

                // "LateUpdate"
                strTmp = StateMachineDefine.GetActionString(state, StateMachineDefine.ActionType.LateUpdate);
                methodTmp = t.GetMethod(strTmp, bindingFlags);

                if (methodTmp != null)
                {
                    targetState.LateUpdate = CreateDelegate<Action>(methodTmp, component);
                }

                // "FixedUpdate"
                strTmp = StateMachineDefine.GetActionString(state, StateMachineDefine.ActionType.FixedUpdate);
                methodTmp = t.GetMethod(strTmp, bindingFlags);

                if (methodTmp != null)
                {
                    targetState.FixedUpdate = CreateDelegate<Action>(methodTmp, component);
                }
            }

            //Create nil state mapping
            currentState = new StateMapping(States.Idle);
		}

		private V CreateDelegate<V>(MethodInfo method, Object target) where V : class
		{
			var ret = (Delegate.CreateDelegate(typeof(V), target, method) as V);

			if (ret == null)
			{
				throw new ArgumentException("Unabled to create delegate for method called " + method.Name);
			}
			return ret;

		}

		public void ChangeState(States newState)
		{
			ChangeState(newState, StateTransition.Safe);
		}

		public void ChangeState(States newState, StateTransition transition)
		{
			if (stateLookup == null)
			{
				throw new Exception("States have not been configured, please call initialized before trying to set state");
			}

			if (!stateLookup.ContainsKey(newState))
			{
				throw new Exception("No state with the name " + newState.ToString() + " can be found. Please make sure you are called the correct type the statemachine was initialized with");
			}

			var nextState = stateLookup[newState];

			if (currentState == nextState) return;

			// Cancel any queued changes.
			if (queuedChange != null)
			{
				engine.StopCoroutine(queuedChange);
				queuedChange = null;
			}

			switch (transition)
			{
				//case StateMachineTransition.Blend:
				//Do nothing - allows the state transitions to overlap each other. This is a dumb idea, as previous state might trigger new changes. 
				//A better way would be to start the two couroutines at the same time. IE don't wait for exit before starting start.
				//How does this work in terms of overwrite?
				//Is there a way to make this safe, I don't think so? 
				//break;
				case StateTransition.Safe:
					if (isInTransition)
					{
						if (exitRoutine != null) //We are already exiting current state on our way to our previous target state
						{
							//Overwrite with our new target
							destinationState = nextState;
							return;
						}

						if (enterRoutine != null) //We are already entering our previous target state. Need to wait for that to finish and call the exit routine.
						{
							//Damn, I need to test this hard
							queuedChange = WaitForPreviousTransition(nextState);
							engine.StartCoroutine(queuedChange);
							return;
						}
					}
					break;
				case StateTransition.Overwrite:
					if (currentTransition != null)
					{
						engine.StopCoroutine(currentTransition);
					}
					if (exitRoutine != null)
					{
						engine.StopCoroutine(exitRoutine);
					}
					if (enterRoutine != null)
					{
						engine.StopCoroutine(enterRoutine);
					}

					//Note: if we are currently in an EnterRoutine and Exit is also a routine, this will be skipped in ChangeToNewStateRoutine()
					break;
			}


			if ((currentState != null && currentState.hasExitRoutine) || nextState.hasEnterRoutine)
			{
				isInTransition = true;
				currentTransition = ChangeToNewStateRoutine(nextState, transition);
				engine.StartCoroutine(currentTransition);
			}
			else //Same frame transition, no coroutines are present
			{
				if (currentState != null)
				{
					currentState.ExitCall(nextState.state);
					currentState.Finally();
				}

				lastState = currentState;
				currentState = nextState;
				if (currentState != null)
				{
					currentState.EnterCall();
					if (Changed != null)
					{
						Changed(currentState.state);
					}
				}
				isInTransition = false;
			}
		}

		private IEnumerator ChangeToNewStateRoutine(StateMapping newState, StateTransition transition)
		{
			destinationState = newState; //Chache this so that we can overwrite it and hijack a transition

			if (currentState != null)
			{
				if (currentState.hasExitRoutine)
				{
					exitRoutine = currentState.ExitRoutine();

					if (exitRoutine != null && transition != StateTransition.Overwrite) //Don't wait for exit if we are overwriting
					{
						yield return engine.StartCoroutine(exitRoutine);
					}

					exitRoutine = null;
				}
				else
				{
					currentState.ExitCall(destinationState.state);
				}

				currentState.Finally();
			}

			lastState = currentState;
			currentState = destinationState;

			if (currentState != null)
			{
				if (currentState.hasEnterRoutine)
				{
					enterRoutine = currentState.EnterRoutine();

					if (enterRoutine != null)
					{
						yield return engine.StartCoroutine(enterRoutine);
					}

					enterRoutine = null;
				}
				else
				{
					currentState.EnterCall();
				}

				//Broadcast change only after enter transition has begun. 
				if (Changed != null)
				{
					Changed(currentState.state);
				}
			}

			isInTransition = false;
		}

		IEnumerator WaitForPreviousTransition(StateMapping nextState)
		{
			while (isInTransition)
			{
				yield return null;
			}

			ChangeState(nextState.state);
		}

		public States LastState
		{
			get
			{
				if (lastState == null) return default(States);

				return lastState.state;
			}
		}

		public States State
		{
			get { return currentState.state; }
		}

		public bool IsInTransition
		{
			get { return isInTransition; }
		}

		public StateMapping CurrentStateMap
		{
			get { return currentState; }
		}

		public MapMoveObject Component
		{
			get { return component; }
		}

		/// <summary>
		/// Inspects a MonoBehaviour for state methods as definied by the supplied Enum, and returns a stateMachine instance used to trasition states. 
		/// </summary>
		/// <param name="component">The component with defined state methods</param>
		/// <param name="startState">The default starting state</param>
		/// <returns>A valid stateMachine instance to manage MonoBehaviour state transitions</returns>
		public static StateMachine Initialize(States startState, MapMoveObject component)
		{
            var engine = component.gameObj.GetComponent<StateMachineRunner>();
            if (engine == null) engine = component.gameObj.AddComponent<StateMachineRunner>();

			return engine.Initialize(component, startState);
		}
	}
}
