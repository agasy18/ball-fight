
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

public static class DebugHelper
{
    private static DateTime last = DateTime.Now;


    public static string GetTimeDesc()
    {
        var timeNow = DateTime.Now;
        string te = timeNow.ToString("hh:mm:ss.fff") + "(" + (timeNow - last).ToString() + "): ";
        last = timeNow;
        return te;
    }
    public static void Log(params object[] parameters)
    {
        StringBuilder str = new StringBuilder();
        str.Append(GetTimeDesc());
        foreach (var param in parameters)
        {
            str.Append((param ?? "null").ToString() + ",");
        }
        System.Diagnostics.Debug.WriteLine(str.Remove(str.Length - 1, 1).ToString());
    }
    public static void Log(this Object obj, params object[] parameters)
    {
        StringBuilder str = new StringBuilder();
        str.Append(GetTimeDesc());
        foreach (var param in parameters)
        {
            str.Append((param ?? "null").ToString() + ",");
        }
        str.Remove(str.Length - 1, 1);
        str.Append(" = " + (obj ?? "null").ToString());
        System.Diagnostics.Debug.WriteLine(str.ToString());
    }

    public static T Log<T>(this T obj, params object[] parameters)
    {
        StringBuilder str = new StringBuilder();
        str.Append(GetTimeDesc());
        foreach (var param in parameters)
        {
            str.Append((param ?? "null").ToString() + ",");
        }
        str.Remove(str.Length - 1, 1);
        str.Append(" = " + (obj == null ? "null" : obj.ToString()));
        System.Diagnostics.Debug.WriteLine(str.ToString());
        return obj;
    }
    public static T Log<T>(this T obj)
    {
        System.Diagnostics.Debug.WriteLine(GetTimeDesc() + obj == null ? "null" : obj.ToString());
        return obj;
    }

    public static bool Assert(this bool t, string mes = "Condition is false")
    {
        if (!t)
        {
            //  ShowDebugInfo("Assert", new Exception(mes));
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }
        return t;
    }

    public static T Assert<T>(this T t, string mes = "Object is null") where T : class
    {
        if (t == null)
        {
            // ShowDebugInfo("Assert", new Exception(mes));
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
        }
        return t;
    }
    static string bugKey = "bugKeywp8Hello";
    static int bugIndex = 0;


    public static string DebugDesc(this object obj)
    {
        if (obj == null)
            return "null";
        StringBuilder str = new StringBuilder();
        str.AppendLine("Type : " + obj.GetType().ToString());
        if (obj is NavigationFailedEventArgs)
        {
            var o = obj as NavigationFailedEventArgs;
            str.AppendLine("Handled = " + o.Handled);
            str.AppendLine(DebugDesc(o.Exception));
        }
        else
            if (obj is Exception)
            {
                var o = obj as Exception;
                str.AppendLine("Message = " + o.Message);
                str.AppendLine("InnerException =\n" + o.InnerException.DebugDesc());
                str.AppendLine("HelpLink = " + o.HelpLink);
                str.AppendLine("Data =\n" + o.Data.DebugDesc());
                str.AppendLine("Source = " + o.Source);
                str.AppendLine("Source = " + o.Source);
                str.AppendLine("StackTrace = " + o.StackTrace);
            }
            else if (obj is IDictionary)
            {
                var o = obj as IDictionary;
                if (o.Count == 0)
                {
                    str.Append("{empty}");
                }
                else
                {
                    str.AppendLine("{\n");

                    var ok = o.Keys.GetEnumerator();
                    var ov = o.Values.GetEnumerator();
                    for (int i = 0; i < o.Count; i++)
                    {
                        str.AppendLine(ok.Current + " : " + ov.Current);
                        ok.MoveNext();
                        ov.MoveNext();
                    }
                    str.AppendLine("\n}");
                }
            }
            else if (obj is ICollection)
            {

                var o = obj as ICollection;
                if (o.Count == 0)
                {
                    str.Append("{empty}");
                }
                else
                {
                    str.Append("[\n");
                    foreach (var item in o)
                    {
                        str.Append(item);
                        str.AppendLine(",");
                    }
                    str.AppendLine("\n]");
                }
            }
            else
            {
                obj.ToString();
            }
        return str.ToString();
    }


}
