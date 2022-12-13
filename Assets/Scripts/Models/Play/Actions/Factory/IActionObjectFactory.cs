using System;
using Leopotam.EcsLite;
using Solcery.Games.States.New.Actions;

namespace Solcery.Models.Play.Actions.Factory
{
    public interface IActionObjectFactory
    {
        IActionObjectFactory RegistrationCreationMethod(Func<IActionObjectFactory, EcsWorld, UpdateAction, bool> creationMethod);
        void ApplyAction(IActionObjectFactory factory, EcsWorld world, UpdateAction action);
    }
}