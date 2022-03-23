// ----------------------------------------------------------------------------
// The MIT License
// Ugui bindings https://github.com/Leopotam/ecslite-unity-ugui
// for LeoECS Lite https://github.com/Leopotam/ecslite
// Copyright (c) 2021 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Leopotam.EcsLite.Unity.Ugui {
    /// <summary>
    /// Emitter system for uGui events to ECS world.
    /// </summary>
    public class EcsUguiEmitter : MonoBehaviour {
        EcsWorld _world;
        readonly Dictionary<int, GameObject> _actions = new Dictionary<int, GameObject> (64);

        internal void SetWorld (EcsWorld world) {
#if DEBUG
            if (_world != null) { throw new Exception ("World already attached."); }
#endif
            _world = world;
        }

        public virtual EcsWorld GetWorld () {
            return _world;
        }

        public virtual void SetNamedObject (string widgetName, GameObject go) {
            if (!string.IsNullOrEmpty (widgetName)) {
                var id = widgetName.GetHashCode ();
                if (_actions.ContainsKey (id)) {
                    if (!go) {
                        _actions.Remove (id);
                    } else {
                        throw new Exception ($"Action with \"{widgetName}\" name already registered");
                    }
                } else {
                    if ((object) go != null) {
                        _actions[id] = go.gameObject;
                    }
                }
            }
        }

        /// <summary>
        /// Gets link to named GameObject to use it later from code.
        /// </summary>
        /// <param name="widgetName">Logical name.</param>
        public virtual GameObject GetNamedObject (string widgetName) {
            _actions.TryGetValue (widgetName.GetHashCode (), out var retVal);
            return retVal;
        }
    }
}