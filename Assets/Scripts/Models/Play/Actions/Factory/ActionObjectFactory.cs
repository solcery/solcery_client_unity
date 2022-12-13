using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Solcery.Games.States.New.Actions;

namespace Solcery.Models.Play.Actions.Factory
{
    public sealed class ActionObjectFactory : IActionObjectFactory
    {
        private readonly List<Func<IActionObjectFactory, EcsWorld, UpdateAction, bool>> _creationMethods;

        public static IActionObjectFactory Create()
        {
            return new ActionObjectFactory();
        }

        private ActionObjectFactory()
        {
            _creationMethods = new List<Func<IActionObjectFactory, EcsWorld, UpdateAction, bool>>(10);
        }

        IActionObjectFactory IActionObjectFactory.RegistrationCreationMethod(Func<IActionObjectFactory, EcsWorld, UpdateAction, bool> creationMethod)
        {
            if (!_creationMethods.Contains(creationMethod))
            {
                _creationMethods.Add(creationMethod);
            }
            
            return this;
        }

        void IActionObjectFactory.ApplyAction(IActionObjectFactory factory, EcsWorld world, UpdateAction action)
        {
            foreach (var creationMethod in _creationMethods)
            {
                if (creationMethod.Invoke(factory, world, action))
                {
                    break;
                }
            }
        }
    }
}