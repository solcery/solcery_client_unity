using Solcery.Games.States.New.Actions.Animation;

namespace Solcery.Services.GameContent.Animations
{
    public interface IAnimationTypes
    {
        UpdateActionAnimationTypes TypeForId(int animationId);
        bool TryTypeForId(int animationId, out UpdateActionAnimationTypes animationType);
        void Cleanup();
    }
}