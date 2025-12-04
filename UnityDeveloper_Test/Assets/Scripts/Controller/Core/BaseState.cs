using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseState<T> : IState where T : Object 
{
    protected T _controller;

    public BaseState(T m_Controller)
    {
        _controller = m_Controller;
    }

    public virtual void EnterState()
    {
    }

    public virtual void ExitState()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Update()
    {
    }
}
