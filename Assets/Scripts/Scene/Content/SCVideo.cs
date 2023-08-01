using UnityEngine;
using UnityEngine.Video;
using SH.QR;

namespace SH.Scene.Content
{
    public class SCVideo : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private Sprite _playSprite;
        [SerializeField] private Sprite _stopSprite;
        [SerializeField] private SpriteRenderer _playButtonRenderer;


        [field: Header("Runtime")]
        [field: SerializeField] public string VideoID { get; private set; }
        [field: SerializeField] public SHQRCode Code { get; private set; }

        private void AssignMaterialAndRenderTexture()
        {
            RenderTexture renderTexture = new RenderTexture(1080, 720, 0);
            renderTexture.name = "NewRenderTexture";
            _videoPlayer.targetTexture = renderTexture;

            // Create a new material and assign the render texture to it
            Material newMaterial = new Material(Shader.Find("Standard")); // You can use any other shader you need
            newMaterial.name = "NewMaterial";
            newMaterial.mainTexture = renderTexture;
            _videoPlayer.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = newMaterial;
        }

        /// <summary>
        /// Initialize the video gameobject.
        /// </summary>
        /// <param name="code">QR code</param>
        /// <returns>video component instance of the newly created gameobject</returns>
        public SCVideo Init(SHQRCode code)
        {
            VideoID = GetVideoID(code);
            gameObject.name = "Video " + VideoID;

            Code = code;

            transform.SetPositionAndRotation(Code.Position, Code.Rotation);
            _videoPlayer.transform.localScale = Code.Scale;

            _videoPlayer.gameObject.AddComponent<BoxCollider>();
            AssignMaterialAndRenderTexture();
            _videoPlayer.url = code.URL;
            _videoPlayer.Play();

            return this;
        }

        /// <summary>
        /// Returns the video ID.
        /// </summary>
        /// <param name="code">QR code</param>
        /// <returns>video ID</returns>
        public static string GetVideoID(SHQRCode code)
        {
            return code.ID;
        }

        public void TogglePause()
        {
            if (_videoPlayer.isPlaying)
            {
                _playButtonRenderer.sprite = _playSprite;
                _videoPlayer.Pause();
            }
            else
            {
                _playButtonRenderer.sprite = _stopSprite;
                _videoPlayer.Play();
            }
        }
    }
}
