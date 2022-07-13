using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Solcery.Widgets_new.Eclipse.Effects
{
    public class TokenEffectLayout : MonoBehaviour
    {
        public Image Image;
        public RectTransform RectTransform;

        [SerializeField]
        private GameObject createAnimation;
        [SerializeField]
        private GameObject moveAnimation;
        [SerializeField]
        private GameObject destroyAnimation;

        [SerializeField]
        private List<ParticleSystem> createParticleSystems;
        [SerializeField]
        private List<ParticleSystem> moveParticleSystems;
        [SerializeField]
        private List<ParticleSystem> destroyParticleSystems;

        public void UpdateCreateAnimation(bool isShow)
        {
            createAnimation.SetActive(isShow);
        }

        public void UpdateMoveAnimation(bool isShow)
        {
            moveAnimation.SetActive(isShow);
        }

        public void UpdateDestroyAnimation(bool isShow)
        {
            destroyAnimation.SetActive(isShow);
        }

        public void UpdateEffectColor(Color color)
        {
            foreach (var ps in createParticleSystems)
            {
                var main = ps.main;
                main.startColor = UpdateColor(main.startColor, color);
            }
            
            foreach (var ps in moveParticleSystems)
            {
                var main = ps.main;
                main.startColor = UpdateColor(main.startColor, color);

                var trail = ps.trails;
                if (trail.enabled)
                {
                    trail.colorOverTrail = UpdateColor(trail.colorOverTrail, color);
                    trail.colorOverLifetime = UpdateColor(trail.colorOverLifetime, color);
                }
            }
            
            foreach (var ps in destroyParticleSystems)
            {
                var main = ps.main;
                main.startColor = UpdateColor(main.startColor, color);
            }
        }

        private ParticleSystem.MinMaxGradient UpdateColor(ParticleSystem.MinMaxGradient source, Color color)
        {
            switch (source.mode)
            {
                case ParticleSystemGradientMode.Color:
                    return source.color * color;
                
                case ParticleSystemGradientMode.TwoColors:
                    return new ParticleSystem.MinMaxGradient(source.colorMin * color, source.colorMax * color);
                
                case ParticleSystemGradientMode.Gradient:
                    var grad = new Gradient
                    {
                        mode = source.gradient.mode
                    };
                    var ck = new GradientColorKey[source.gradient.colorKeys.Length];
                    var index = 0;
                    foreach (var colorKey in source.gradient.colorKeys)
                    {
                        ck[index] = new GradientColorKey(colorKey.color * color, colorKey.time);
                        ++index;
                    }
                    grad.SetKeys(ck, source.gradient.alphaKeys);
                    return grad;

                case ParticleSystemGradientMode.TwoGradients:
                    var gradMin = new Gradient
                    {
                        mode = source.gradientMin.mode
                    };
                    
                    var ckMin = new GradientColorKey[source.gradientMin.colorKeys.Length];
                    var indexMin = 0;
                    foreach (var colorKey in source.gradientMin.colorKeys)
                    {
                        ckMin[indexMin] = new GradientColorKey(colorKey.color * color, colorKey.time);
                        ++indexMin;
                    }
                    gradMin.SetKeys(ckMin, source.gradientMin.alphaKeys);
                    
                    var gradMax = new Gradient
                    {
                        mode = source.gradientMax.mode
                    };
                    
                    var ckMax = new GradientColorKey[source.gradientMax.colorKeys.Length];
                    var indexMax = 0;
                    foreach (var colorKey in source.gradientMax.colorKeys)
                    {
                        ckMax[indexMax] = new GradientColorKey(colorKey.color * color, colorKey.time);
                        ++indexMax;
                    }
                    gradMin.SetKeys(ckMax, source.gradientMax.alphaKeys);


                    return new ParticleSystem.MinMaxGradient(gradMin, gradMax);
            }

            return color;
        }
    }
}
