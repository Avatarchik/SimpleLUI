//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;

namespace JEM.Core.Common
{
    /// <inheritdoc />
    /// <summary>
    ///     A key based database class.
    ///     It is a small helper with managing Dictionary(string, object).
    ///     Implements object change events and parse objects type to original type.
    /// </summary>
    [Serializable]
    public class JEMKeyBasedDatabase : ICloneable
    {
        /// <summary/>
        public Dictionary<string, object> SystemObjects = new Dictionary<string, object>();
        private Dictionary<string, Action<object>> OnChangedEvents { get; } = new Dictionary<string, Action<object>>();

        /// <summary>
        ///     Checks system object dictionary for any changes.
        /// </summary>
        public bool CheckForChanges(Dictionary<string, object> otherSystemObjects)
        {
            foreach (var obj in SystemObjects)
            {
                if (!otherSystemObjects.ContainsKey(obj.Key))
                    continue;

                if (otherSystemObjects[obj.Key] != obj.Value)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Writes value to system objects.
        /// </summary>
        public void WriteToSystem(string key, object obj, bool silently = false)
        {
            if (SystemObjects.ContainsKey(key))
            {
                var prev = SystemObjects[key] == obj;
                SystemObjects[key] = obj;
                if (prev)
                    return;
            }
            else
            {
                SystemObjects.Add(key, obj);
            }

            if (!silently)
            {
                foreach (var onChangedEvent in OnChangedEvents)
                    if (onChangedEvent.Key == key)
                    {
                        onChangedEvent.Value?.Invoke(obj);
                    }
            }
        }

        /// <summary>
        ///     Resolves system object from given key.
        /// </summary>
        public T ResolveFromSystem<T>(string key)
        {
            var obj = ResolveFromSystem(key);
            return obj == null ? default(T) : JEMFixedJsonType.Fix<T>(obj);
        }

        /// <summary>
        ///     Checks if given key exists.
        /// </summary>
        public bool HasKey(string key)
        {
            return SystemObjects.ContainsKey(key) && SystemObjects[key] != null;
        }

        /// <summary>
        ///     Resolves system object from given key.
        /// </summary>
        public object ResolveFromSystem(string key)
        {
            return SystemObjects.ContainsKey(key) ? SystemObjects[key] : null;
        }

        /// <summary>
        ///     Checks if system object of given name exists.
        /// </summary>
        public int CheckSystemObject(string key, object defaultValue)
        {
            if (ResolveFromSystem(key) == null)
            {
                WriteToSystem(key, defaultValue);
                return 1;
            }

            return 0;
        }

        /// <summary>
        ///     Registers on object changed event for given system key.
        /// </summary>
        public Action<object> RegisterObjectChange(string systemKey, Action<object> onObjectChanged, bool invokeAtStart = true)
        {
            if (OnChangedEvents.ContainsKey(systemKey))
                OnChangedEvents[systemKey] += onObjectChanged;
            else
                OnChangedEvents.Add(systemKey, onObjectChanged);

            if (invokeAtStart)
            {
                var k = ResolveFromSystem(systemKey);
                if (k != null)
                {
                    onObjectChanged?.Invoke(k);
                }
            }

            return onObjectChanged;
        }

        /// <summary>
        ///     Register change event.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public Action<object> RegisterObjectChange<T>(string systemKey, Action<T> onValueChanged, bool invokeAtStart = true)
        {
            if (onValueChanged == null) throw new ArgumentNullException(nameof(onValueChanged));
            if (!OnChangedEvents.ContainsKey(systemKey))
            {
                void New(object obj) { }
                OnChangedEvents.Add(systemKey, New);
            }

            var a = new Action<object>(obj =>
            {
                if (JEMFixedJsonType.Fix<T>(obj, out var type))
                {
                    onValueChanged.Invoke(type);
                }
            });

            OnChangedEvents[systemKey] += a;
            if (invokeAtStart)
            {
                var k = ResolveFromSystem(systemKey);
                if (k != null)
                {
                    if (JEMFixedJsonType.Fix<T>(k, out var type))
                    {
                        onValueChanged(type);
                    }
                }
            }

            return a;
        }

        /// <summary>
        ///     Makes unhandled call of all OnChangedEvents passing last value.
        /// </summary>
        public void UnhandledCallOfEverything()
        {
            foreach (var e in OnChangedEvents)
            {
                var k = ResolveFromSystem(e.Key);
                e.Value?.Invoke(k);
            }
        }

        /// <summary>
        ///     Unregisters given on object changed event for given system key.
        /// </summary>
        public void UnregisterObjectChange(string systemKey, Action<object> onObjectChange)
        {
            if (OnChangedEvents.ContainsKey(systemKey))
                OnChangedEvents[systemKey] -= onObjectChange;
        }

        /// <inheritdoc />
        public object Clone()
        {
            var db = new JEMKeyBasedDatabase();
            foreach (var s in SystemObjects)
            {
                db.SystemObjects.Add(s.Key, s.Value);
            }

            return db;
        }
    }
}
