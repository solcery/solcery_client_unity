using System;

namespace Solcery
{
    public class Service<T> where T : new()
    {
        private static readonly Lazy<T> Lazy = 
            new Lazy<T>(() => new T());

        public static T GetInstance()
        {
            return Lazy.Value;
        }
    }
}