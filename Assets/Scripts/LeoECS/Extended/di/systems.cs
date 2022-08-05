// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
using System;
using Unity.IL2CPP.CompilerServices;
#endif

namespace Leopotam.EcsLite.ExtendedSystems {
#if LEOECSLITE_DI
#if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
    public class EcsGroupSystemWithDi : EcsGroupSystem, IEcsInjectSystem {
        public EcsGroupSystemWithDi (string name, bool defaultState, string eventsWorldName, params IEcsSystem[] systems) : base (name, defaultState, eventsWorldName, systems) { }

        public void Inject (IEcsSystems systems, params object[] injects) {
            foreach (var system in GetNestedSystems ()) {
                if (system is IEcsInjectSystem injectSystem) {
                    injectSystem.Inject (systems, injects);
                    continue;
                }
                Di.ExtensionsDi.InjectToSystem (system, systems, injects);
            }
        }
    }
#endif
}