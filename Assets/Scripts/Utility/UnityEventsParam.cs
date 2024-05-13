using System;
using UnityEngine.Events;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    [Serializable]
    public class TransformEvent : UnityEvent<Transform>
    {
    }

    [Serializable]
    public class Vector3Event : UnityEvent<Vector3>
    {
    }


    [Serializable]
    public class StringEvent : UnityEvent<string>
    {
    }

    [Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }

    [Serializable]
    public class FloatEvent : UnityEvent<float>
    {
    }
}