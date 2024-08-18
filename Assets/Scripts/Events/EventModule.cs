using System;
using System.Collections.Generic;
using UnityEngine;

public class EventModule
{
    //声明事件委托，用于整合相同监听事件的多个委托
    public delegate void CallBack();
    public delegate void CallBack<T>(T arg1);
    public delegate void CallBack<T, X>(T arg1, X arg2);
    public delegate void CallBack<T, X, Y>(T arg1, X arg2, Y arg3);
    public delegate void CallBack<T, X, Y, Z>(T arg1, X arg2, Y arg3, Z arg4);
    public delegate void CallBack<T, X, Y, Z, W>(T arg1, X arg2, Y arg3, Z arg4, W arg5);

    //声明事件字典，用于绑定监听事件与其对应的委托
    private static Dictionary<string, Delegate> m_EventTable;

    static EventModule()
    {
        m_EventTable = new Dictionary<string, Delegate>();
    }
    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="eventType"></param>
    public static void AddEvent(string eventType)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Add(eventType, null);
        }
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="eventType"></param>
    public static void RemoveEvent(string eventType)
    {
        if(m_EventTable.ContainsKey(eventType))
        {
            m_EventTable.Remove(eventType);
        }
    }

    /// <summary>
    /// 检测添加事件委托的合法性
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    /// <exception cref="Exception"></exception>
    private static void OnListenerAdding(string eventType, Delegate callBack)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            throw new Exception(string.Format("添加事件委托错误：事件{0}不存在",
                                              eventType));
        }

        //事件委托类型不匹配
        Delegate eventCallBack = m_EventTable[eventType];

        if (eventCallBack != null && eventCallBack.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("添加事件委托错误：尝试为事件{0}添加不同类型的委托，当前事件所对应的委托是{1}，要添加的委托类型为{2}",
                                              eventType, eventCallBack.GetType(), callBack.GetType()));
        }
    }

    /// <summary>
    /// 检测移除事件委托的合法性
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    /// <exception cref="Exception"></exception>
    private static void OnListenerRemoving(string eventType, Delegate callBack)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            Delegate eventCallBack = m_EventTable[eventType];

            //监听事件不存在对应委托
            if (eventCallBack == null)
            {
                throw new Exception(String.Format("移除事件委托错误：事件{0}没有对应的委托", eventType));
            }
            //事件委托类型不匹配
            else if (eventCallBack.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移除事件委托错误：尝试为事件{0}移除不同类型的委托，当前委托类型为{1}，要移除的委托类型为{2}",
                                                  eventType, eventCallBack.GetType(), callBack.GetType()));
            }
        }
        //监听事件不存在
        else
        {
            throw new Exception(String.Format("移除监听事件错误：事件{0}不存在", eventType));
        }
    }

    /// <summary>
    /// 添加事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener(string eventType, CallBack callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T>(string eventType, CallBack<T> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T, X>(string eventType, CallBack<T, X> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T, X, Y>(string eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T, X, Y, Z>(string eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// 添加事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T, X, Y, Z, W>(string eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// 移除事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener(string eventType, CallBack callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// 移除事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T>(string eventType, CallBack<T> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// 移除事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T, X>(string eventType, CallBack<T, X> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// 移除事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T, X, Y>(string eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// 移除事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T, X, Y, Z>(string eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// 移除事件委托
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T, X, Y, Z, W>(string eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// 触发监听事件
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast(string eventType)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack callBack = (CallBack)tempDelegate;

            //事件委托匹配
            if (callBack != null)
            {
                callBack();
            }
            //事件委托不匹配
            else
            {
                throw new Exception(string.Format("触发监听事件错误：事件{0}对应委托具有不同的类型",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// 触发监听事件
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T>(string eventType, T arg1)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T> callBack = (CallBack<T>)tempDelegate;

            //事件委托匹配
            if (callBack != null)
            {
                callBack(arg1);
            }
            //事件委托不匹配
            else
            {
                throw new Exception(string.Format("触发监听事件错误：事件{0}对应委托具有不同的类型",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// 触发监听事件
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T, X>(string eventType, T arg1, X arg2)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T, X> callBack = (CallBack<T, X>)tempDelegate;

            //事件委托匹配
            if (callBack != null)
            {
                callBack(arg1, arg2);
            }
            //事件委托不匹配
            else
            {
                throw new Exception(string.Format("触发监听事件错误：事件{0}对应委托具有不同的类型",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// 触发监听事件
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T, X, Y>(string eventType, T arg1, X arg2, Y arg3)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T, X, Y> callBack = (CallBack<T, X, Y>)tempDelegate;

            //事件委托匹配
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);
            }
            //事件委托不匹配
            else
            {
                throw new Exception(string.Format("触发监听事件错误：事件{0}对应委托具有不同的类型",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// 触发监听事件
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T, X, Y, Z>(string eventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T, X, Y, Z> callBack = (CallBack<T, X, Y, Z>)tempDelegate;

            //事件委托匹配
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
            }
            //事件委托不匹配
            else
            {
                throw new Exception(string.Format("触发监听事件错误：事件{0}对应委托具有不同的类型",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// 触发监听事件
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T, X, Y, Z, W>(string eventType, T arg1, X arg2, Y arg3, Z arg4, W arg5)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T, X, Y, Z, W> callBack = (CallBack<T, X, Y, Z, W>)tempDelegate;

            //事件委托匹配
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4, arg5);
            }
            //事件委托不匹配
            else
            {
                throw new Exception(string.Format("触发监听事件错误：事件{0}对应委托具有不同的类型",
                                                  eventType));
            }
        }
    }

}