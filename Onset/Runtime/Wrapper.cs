﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using Onset.Convertation;
using Onset.Entities;
using Onset.Event;

namespace Onset.Runtime
{
    internal class Wrapper
    {
        internal const string RuntimeName = "COW.Runtime.dll";
        internal const int ApiVersion = 1;

        internal static Server Server { get; private set; }


        [DllImport(RuntimeName, EntryPoint = "execute_lua", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ExecuteLuaPtr([MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string data);

        internal static ReturnData ExecuteLua(string name, object data = null)
        {
            string json = "";
            if (data != null)
            {
                json = Escape(data);
            }
            return new ReturnData(Marshal.PtrToStringUTF8(ExecuteLuaPtr(name, json)));
        }

        [DllImport(RuntimeName, EntryPoint = "log_to_console", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void LogConsole([MarshalAs(UnmanagedType.LPStr)]string message);

        internal static bool ExecuteEvent(string name, string json)
        {
            try
            {
                if (name == "finish-wrapper")
                {
                    Server.Start();
                    LogConsole("COW: Finish-Trigger received! Wrapper has been finished and is now completely functional!");
                    return false;
                }

                if (name == "trigger-event")
                {
                    return Server.ExecuteServerEvent(new ReturnData(json));
                }

                if (name == "trigger-remote-event")
                {
                    ReturnData data = new ReturnData(json);
                    string[] args = data.Values<string>("args");
                    IPlayer player = Server.PlayerPool.GetPlayer(data.Value<int>("player"));
                    foreach (Registry<RemoteEvent>.Item item in Server.RemoteEventRegistry.GetAll(item => item.Data.Key == data.Value<string>("eventName")))
                    {
                        item.Invoke(Converts.Convert(args, item.Invoker.GetParameters(), player));
                    }
                }

                if (name == "trigger-command")
                {
                    ReturnData data = new ReturnData(json);
                    string[] args = data.ValuesAsStrings("args");
                    IPlayer player = Server.PlayerPool.GetPlayer(data.Value<int>("player"));
                    foreach (Registry<Command>.Item item in Server.CommandRegistry.GetAll(item => item.Data.Name == data.Value<string>("commandName")))
                    {
                        item.Invoke(Converts.Convert(args, item.Invoker.GetParameters(), player));
                    }
                }
            }
            catch (Exception e)
            {
                Server.Logger.Error("An unhandled exception occurred!", e);
            }
            return false;
        }

        internal static void Load()
        {
            LogConsole("COW: Loading Wrapper...");
            Server = new Server();
            LogConsole("COW: Wrapper loaded!");
        }

        internal static void Unload()
        {
            LogConsole("COW: Stopping Wrapper...");
            Server.Stop();
            LogConsole("COW: Wrapper stopped");
        }

        internal static string Escape(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
