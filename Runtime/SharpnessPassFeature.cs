using UnityEngine;

namespace CannoliCat.Stylized {
    public class SharpnessPassFeature : ScreenEffectFeature<SharpnessSettings> { }

    [System.Serializable]
    public class SharpnessSettings : IScreenEffectSettings {
        private static readonly int Amount = Shader.PropertyToID("_Amount");

        [SerializeField] [Range(-10.0f, 10.0f)] private float amount = 0.0f;

        public void Apply(Material material) {
            material.SetFloat(Amount, amount);
        }
    }
}
