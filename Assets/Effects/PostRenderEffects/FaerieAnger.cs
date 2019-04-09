using UnityEngine;

public class FaerieAnger : MonoBehaviour {
    [SerializeField] private Shader shaderEffect = null;
    [SerializeField] private Texture overlayTexture = null;
    [SerializeField] private Color overlayColor = Color.black;
    [SerializeField] private AnimationCurve alphaCurve = null;

    private Material _shaderMaterial;
    private float _effectMaxDuration = 0;
    private float _currentEffectTime = float.MaxValue;

    private void OnEnable() {
        if (shaderEffect != null) {
            _shaderMaterial = new Material(shaderEffect);
            _shaderMaterial.SetTexture("_OverlayTexture", overlayTexture);
        }
    }

    private void Update() {
        if (_currentEffectTime < _effectMaxDuration)
            _currentEffectTime += Time.deltaTime;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_shaderMaterial != null && _currentEffectTime < _effectMaxDuration) {
            _shaderMaterial.SetFloat("_Alpha", CalculateAlpha());
            _shaderMaterial.SetColor("_Color", overlayColor);
            Graphics.Blit(source, destination, _shaderMaterial);
        }
        else
            Graphics.Blit(source, destination);
    }

    private float CalculateAlpha() {
        return alphaCurve.Evaluate(_currentEffectTime / _effectMaxDuration);
    }

    public void StartEffect(float duration) {
        _effectMaxDuration = duration;
        _currentEffectTime = 0;
    }

    private void OnDisable() {
        if (_shaderMaterial != null)
            DestroyImmediate(_shaderMaterial);
    }
}