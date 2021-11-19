using System;
using UnityEngine;

namespace Solcery.Services.Resources.Loaders.WidgetPrefab
{
    public interface IPrefabLoader
    {
        string Name { get; }
        GameObject Prefab { get; }
        void Load(Action<IPrefabLoader> callback);
        void Destroy();
    }
}