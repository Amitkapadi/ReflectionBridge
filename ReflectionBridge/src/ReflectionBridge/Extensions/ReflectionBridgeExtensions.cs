﻿using System;
using System.Reflection;

#if REFLECTIONBRIDGE
using System.Collections.Generic;
using System.Linq;
#endif

namespace ReflectionBridge.Extensions
{
    public static class ReflectionBridgeExtensions
    {
        public static Assembly GetAssembly(this Type type)
        {
#if REFLECTIONBRIDGE
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if REFLECTIONBRIDGE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsNullableEnum(this Type type)
        {
            return type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>) && type.GetGenericArguments()[0].IsEnum();
        }

        public static bool IsFromLocalAssembly(this Type type)
        {
            var assemblyName = type.GetAssembly().GetName().Name;

            try
            {
#if REFLECTIONBRIDGE
                Assembly.Load(new AssemblyName { Name = assemblyName });
#else
                Assembly.Load(assemblyName);
#endif
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsGenericType(this Type type)
        {
#if REFLECTIONBRIDGE
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static bool IsInterface(this Type type)
        {
#if REFLECTIONBRIDGE
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        public static Type BaseType(this Type type)
        {
#if REFLECTIONBRIDGE
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if REFLECTIONBRIDGE
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        public static Type UnwrapNullable(this Type type)
        {
            if (!type.IsGenericType())
                return type;
            if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                return type;
            return type.GetGenericArguments()[0];
        }

        public static T GetPropertyValue<T>(this Type type, string propertyName, object target)
        {
#if REFLECTIONBRIDGE
            PropertyInfo property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            return (T)property.GetValue(target);
#else
            return (T) type.InvokeMember(propertyName, BindingFlags.GetProperty, null, target, null);
#endif
        }

        public static void SetPropertyValue(this Type type, string propertyName, object target, object value)
        {
#if REFLECTIONBRIDGE
            PropertyInfo property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
            property.SetValue(target, value);
#else
            type.InvokeMember(propertyName, BindingFlags.SetProperty, null, target, new object[] { value });
#endif
        }

        public static void SetFieldValue(this Type type, string fieldName, object target, object value)
        {
#if REFLECTIONBRIDGE
            FieldInfo field = type.GetTypeInfo().GetDeclaredField(fieldName);
            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                type.SetPropertyValue(fieldName, target, value);
            }
#else
            type.InvokeMember(fieldName, BindingFlags.SetField | BindingFlags.SetProperty, null, target, new object[] { value });
#endif
        }

        public static void InvokeMethod<T>(this Type type, string methodName, object target, T value)
        {
#if REFLECTIONBRIDGE
            MethodInfo method = type.GetTypeInfo().GetDeclaredMethod(methodName);
            method.Invoke(target, new object[] { value });
#else
            type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, new object[] {value});
#endif
        }

#if REFLECTIONBRIDGE
        public static IEnumerable<MethodInfo> GetMethods(this Type someType)
        {
            var t = someType;
            while (t != null)
            {
                var ti = t.GetTypeInfo();
                foreach (var m in ti.DeclaredMethods)
                    yield return m;
                t = ti.BaseType;
            }
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
        }

        public static bool IsSubclassOf(this Type type, Type c)
        {
            return type.GetTypeInfo().IsSubclassOf(c);
        }

        public static Attribute[] GetCustomAttributes(this Type type)
        {
            return type.GetTypeInfo().GetCustomAttributes().ToArray();
        }
#endif
    }
}