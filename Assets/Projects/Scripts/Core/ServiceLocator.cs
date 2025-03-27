using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static Dictionary<Type, object> _services = new Dictionary<Type, object>();
    private static Dictionary<Type, Func<object>> _servicesFunc = new Dictionary<Type, Func<object>>();

    public static void Register<T>(T service) where T : class
    {
        if (_services.ContainsKey(typeof(T)))
            return;
        _services[typeof(T)] = service;
    }

    public static T Get<T>() where T : class
    {
        return _services[typeof(T)] as T;
    }

    public static void RegisterFunc<T>(Func<object> func) where T : class
    {
        if (_servicesFunc.ContainsKey(typeof(T)))
            return;
        _servicesFunc[typeof(T)] = func;
    }

    public static T GetFunc<T>() where T : class
    {
        return _servicesFunc[typeof(T)]() as T;
    }

    public static void Clear()
    {
        _services.Clear();
        _servicesFunc.Clear();
    }

    public static void RemoveFunc<T>() where T : class
    {
        _servicesFunc.Remove(typeof(T));
    }

    public static void Remove<T>() where T : class
    {
        _services.Remove(typeof(T));
    }
}