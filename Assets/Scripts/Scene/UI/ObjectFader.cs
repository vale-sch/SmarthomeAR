using UnityEngine;

namespace SH.Scene.UI
{
    public class ObjectFader : MonoBehaviour
    {
        [SerializeField] private GameObject spriteContainer;
        [SerializeField] private Transform thresholdPos;
        [SerializeField] private float fadeDistance = 0.01f;

        private SpriteRenderer[] _spriteRenderers;
        private float _thresholdDis;

        private void Start()
        {
            _spriteRenderers = spriteContainer.GetComponentsInChildren<SpriteRenderer>();
            _thresholdDis = Vector3.Distance(transform.position, thresholdPos.position);
        }

        private void Update()
        {
            _thresholdDis = Vector3.Distance(transform.position, thresholdPos.position);

            foreach (var renderer in _spriteRenderers)
            {
                float dis = Vector3.Distance(transform.position, renderer.transform.position);
                Color color = renderer.color;
                color.a = 1f - Mathf.InverseLerp(_thresholdDis, _thresholdDis + fadeDistance, dis);
                renderer.color = color;
            }
        }
    }

}