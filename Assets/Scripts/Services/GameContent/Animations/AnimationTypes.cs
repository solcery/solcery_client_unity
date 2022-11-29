using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Solcery.Games.States.New.Actions.Animation;
using Solcery.Utils;

namespace Solcery.Services.GameContent.Animations
{
    public sealed class AnimationTypes : IAnimationTypes
    {
        private readonly Dictionary<int, UpdateActionAnimationTypes> _animations;

        public static IAnimationTypes Create(JArray data)
        {
            return new AnimationTypes(data);
        }

        private AnimationTypes(JArray data)
        {
            _animations = new Dictionary<int, UpdateActionAnimationTypes>();

            foreach (var animationToken in data)
            {
                if (animationToken is JObject animationObject 
                    && animationObject.TryGetValue("id", out int animationId)
                    && animationObject.TryGetEnum("animation_type", out UpdateActionAnimationTypes animationType)
                    && !_animations.ContainsKey(animationId))
                {
                    _animations.Add(animationId, animationType);
                }
            }
        }

        UpdateActionAnimationTypes IAnimationTypes.TypeForId(int animationId)
        {
            return _animations.TryGetValue(animationId, out var animationType) ? 
                animationType : 
                UpdateActionAnimationTypes.None;
        }

        bool IAnimationTypes.TryTypeForId(int animationId, out UpdateActionAnimationTypes animationType)
        {
            return _animations.TryGetValue(animationId, out animationType);
        }

        void IAnimationTypes.Cleanup()
        {
            _animations.Clear();
        } 
    }
}