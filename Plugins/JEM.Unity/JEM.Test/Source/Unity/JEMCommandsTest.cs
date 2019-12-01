//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.Core.Debugging.Commands;
using JEM.Core.Debugging.CVar;
using JEM.QNet.Extras;
using JEM.UnityEngine.Console;
using System.Collections;
using JEM.QNet.Messages;
using JEM.QNet.UnityEngine;
using JEM.QNet.UnityEngine.Simulation;
using UnityEngine;

namespace JEM.Test.Unity
{
    public struct CommandExecutorTest : IQNetSerializedMessage
    {
        public string Name { get; set; }

        public void Serialize(QNetMessageWriter writer)
        {
            writer.WriteString(Name);
        }

        public void DeSerialize(QNetMessageReader reader)
        {
            Name = reader.ReadString();
        }
    }

    internal class JEMCommandsTest : MonoBehaviour
    {
        private void Awake()
        {
            JEMCVarManager.CollectAllCVars();

            JEMConsole.Instance.OnCommandExecution += OnCommandExecution;

            var group = JEMCommandManager.RegisterGroup("system");
            group.Register("hello_world", () => { JEMLogger.Log("Hello World!", "COMMAND");}, "Hello world!");
            group.Register("seven", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("ultra", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("ille", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("wolf", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("star", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("said", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("online", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("offline", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("never", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("ko", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("misti", () => { JEMLogger.Log("Hello World!", "COMMAND"); }, "Hello world!");
            group.Register("world_helo", () => { JEMLogger.Log("World Hello!", "COMMAND"); }, "Hello world!");
            group.Register<QNetExecutor, string>("lul", (executor, str) => JEMLogger.Log(str + $" {((CommandExecutorTest) executor.Serialized).Name}", "COMMAND"), "LUL");
            group.Register<QNetExecutor, string>("netTest", (executor, str) => executor.SendLog($"Net test -> {str}"), scope: JEMCommandExecScope.ServerPeer);

            StartCoroutine(JEMBackgroundWorker());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                QNetTime.TickRate++;
            }
        }

        private bool OnCommandExecution(string command)
        {
            QNetManager.ExecuteCVarOrCommand(new CommandExecutorTest { Name = "Famram" }, command);
            return true;
        }

        private static IEnumerator JEMBackgroundWorker()
        {
            while (true)
            {
                JEMCVarManager.Update();
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
