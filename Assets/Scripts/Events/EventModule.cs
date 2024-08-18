using System;
using System.Collections.Generic;
using UnityEngine;

public class EventModule
{
    //�����¼�ί�У�����������ͬ�����¼��Ķ��ί��
    public delegate void CallBack();
    public delegate void CallBack<T>(T arg1);
    public delegate void CallBack<T, X>(T arg1, X arg2);
    public delegate void CallBack<T, X, Y>(T arg1, X arg2, Y arg3);
    public delegate void CallBack<T, X, Y, Z>(T arg1, X arg2, Y arg3, Z arg4);
    public delegate void CallBack<T, X, Y, Z, W>(T arg1, X arg2, Y arg3, Z arg4, W arg5);

    //�����¼��ֵ䣬���ڰ󶨼����¼������Ӧ��ί��
    private static Dictionary<string, Delegate> m_EventTable;

    static EventModule()
    {
        m_EventTable = new Dictionary<string, Delegate>();
    }
    /// <summary>
    /// ����¼�
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
    /// �Ƴ��¼�
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
    /// �������¼�ί�еĺϷ���
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    /// <exception cref="Exception"></exception>
    private static void OnListenerAdding(string eventType, Delegate callBack)
    {
        if (!m_EventTable.ContainsKey(eventType))
        {
            throw new Exception(string.Format("����¼�ί�д����¼�{0}������",
                                              eventType));
        }

        //�¼�ί�����Ͳ�ƥ��
        Delegate eventCallBack = m_EventTable[eventType];

        if (eventCallBack != null && eventCallBack.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("����¼�ί�д��󣺳���Ϊ�¼�{0}��Ӳ�ͬ���͵�ί�У���ǰ�¼�����Ӧ��ί����{1}��Ҫ��ӵ�ί������Ϊ{2}",
                                              eventType, eventCallBack.GetType(), callBack.GetType()));
        }
    }

    /// <summary>
    /// ����Ƴ��¼�ί�еĺϷ���
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    /// <exception cref="Exception"></exception>
    private static void OnListenerRemoving(string eventType, Delegate callBack)
    {
        if (m_EventTable.ContainsKey(eventType))
        {
            Delegate eventCallBack = m_EventTable[eventType];

            //�����¼������ڶ�Ӧί��
            if (eventCallBack == null)
            {
                throw new Exception(String.Format("�Ƴ��¼�ί�д����¼�{0}û�ж�Ӧ��ί��", eventType));
            }
            //�¼�ί�����Ͳ�ƥ��
            else if (eventCallBack.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("�Ƴ��¼�ί�д��󣺳���Ϊ�¼�{0}�Ƴ���ͬ���͵�ί�У���ǰί������Ϊ{1}��Ҫ�Ƴ���ί������Ϊ{2}",
                                                  eventType, eventCallBack.GetType(), callBack.GetType()));
            }
        }
        //�����¼�������
        else
        {
            throw new Exception(String.Format("�Ƴ������¼������¼�{0}������", eventType));
        }
    }

    /// <summary>
    /// ����¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener(string eventType, CallBack callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// ����¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T>(string eventType, CallBack<T> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// ����¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T, X>(string eventType, CallBack<T, X> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// ����¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T, X, Y>(string eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// ����¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T, X, Y, Z>(string eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// ����¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void AddListener<T, X, Y, Z, W>(string eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] + callBack;
    }

    /// <summary>
    /// �Ƴ��¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener(string eventType, CallBack callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// �Ƴ��¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T>(string eventType, CallBack<T> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// �Ƴ��¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T, X>(string eventType, CallBack<T, X> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// �Ƴ��¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T, X, Y>(string eventType, CallBack<T, X, Y> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// �Ƴ��¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T, X, Y, Z>(string eventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// �Ƴ��¼�ί��
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="callBack"></param>
    public static void RemoveListener<T, X, Y, Z, W>(string eventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerRemoving(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] - callBack;
    }

    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast(string eventType)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack callBack = (CallBack)tempDelegate;

            //�¼�ί��ƥ��
            if (callBack != null)
            {
                callBack();
            }
            //�¼�ί�в�ƥ��
            else
            {
                throw new Exception(string.Format("���������¼������¼�{0}��Ӧί�о��в�ͬ������",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T>(string eventType, T arg1)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T> callBack = (CallBack<T>)tempDelegate;

            //�¼�ί��ƥ��
            if (callBack != null)
            {
                callBack(arg1);
            }
            //�¼�ί�в�ƥ��
            else
            {
                throw new Exception(string.Format("���������¼������¼�{0}��Ӧί�о��в�ͬ������",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T, X>(string eventType, T arg1, X arg2)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T, X> callBack = (CallBack<T, X>)tempDelegate;

            //�¼�ί��ƥ��
            if (callBack != null)
            {
                callBack(arg1, arg2);
            }
            //�¼�ί�в�ƥ��
            else
            {
                throw new Exception(string.Format("���������¼������¼�{0}��Ӧί�о��в�ͬ������",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T, X, Y>(string eventType, T arg1, X arg2, Y arg3)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T, X, Y> callBack = (CallBack<T, X, Y>)tempDelegate;

            //�¼�ί��ƥ��
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);
            }
            //�¼�ί�в�ƥ��
            else
            {
                throw new Exception(string.Format("���������¼������¼�{0}��Ӧί�о��в�ͬ������",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T, X, Y, Z>(string eventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T, X, Y, Z> callBack = (CallBack<T, X, Y, Z>)tempDelegate;

            //�¼�ί��ƥ��
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
            }
            //�¼�ί�в�ƥ��
            else
            {
                throw new Exception(string.Format("���������¼������¼�{0}��Ӧί�о��в�ͬ������",
                                                  eventType));
            }
        }
    }

    /// <summary>
    /// ���������¼�
    /// </summary>
    /// <param name="eventType"></param>
    public static void Broadcast<T, X, Y, Z, W>(string eventType, T arg1, X arg2, Y arg3, Z arg4, W arg5)
    {
        Delegate tempDelegate;

        if (m_EventTable.TryGetValue(eventType, out tempDelegate))
        {
            CallBack<T, X, Y, Z, W> callBack = (CallBack<T, X, Y, Z, W>)tempDelegate;

            //�¼�ί��ƥ��
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4, arg5);
            }
            //�¼�ί�в�ƥ��
            else
            {
                throw new Exception(string.Format("���������¼������¼�{0}��Ӧί�о��в�ͬ������",
                                                  eventType));
            }
        }
    }

}