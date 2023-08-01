using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using SH.Scene.UI;
using SH.QR;
using SH.XRHUD;
using UnityEngine.Networking;
using System.Collections;

namespace SH.Scene.Content
{
    public class SCPhoto : MonoBehaviour
    {
        public const string PHOTO_ID_SEPERATOR = "$$";

        [Header("Setup")]
        [SerializeField] private GameObject _spriteGO;
        [SerializeField] private GameObject _ui;
        [SerializeField] private ObjectLoader _objectLoader;

        [field: Header("Settings")]
        [field: SerializeField] public string PhotoFilename { get; set; }

        [field: Header("Runtime")]
        [field: SerializeField] public string PhotoID { get; private set; }
        [field: SerializeField] public SHQRCode Code1 { get; private set; }

        /// <summary>
        /// True if Code2 is available.
        /// </summary>
        [field: SerializeField, Space] public bool Has2QRCodes { get; private set; }

        /// <summary>
        /// Only available if Has2QRCodes is true. Otherwise null.
        /// </summary>
        /// <value></value>
        [field: SerializeField] public SHQRCode Code2 { get; private set; }

        [field: SerializeField] public Sprite Sprite { get; private set; }


        /// <summary>
        /// Initialize the photo gameobject. Supports photo with 1 and 2 QR code anchors.
        /// </summary>
        /// <param name="code1">QR code 1</param>
        /// <param name="code2">QR code 2 (optional)</param>
        /// <param name="photoFilename">override the photo filename (optional)</param>
        /// <returns>photo component instance of the newly created gameobject</returns>
        public SCPhoto Init(SHQRCode code1, SHQRCode code2 = null, string photoFilename = null)
        {
            PhotoID = GetPhotoID(code1, code2);
            gameObject.name = "Photo " + PhotoID;

            Code1 = code1;
            Code2 = code2;
            Has2QRCodes = Code2 != null;
            if (!string.IsNullOrWhiteSpace(photoFilename)) PhotoFilename = photoFilename;
            LoadPhoto();

            if (Has2QRCodes)
            {
                // Determine photo size by the distance of the two QR codes (photo diagonal)
                Vector3 code1ToCode2Origin = Code2.Origin - Code1.Origin;
                transform.SetPositionAndRotation(Code1.Origin + code1ToCode2Origin * 0.5f, Quaternion.Lerp(Code1.Rotation, Code2.Rotation, 0.5f));
                Vector3 sizeVector = Vector3.ProjectOnPlane(code1ToCode2Origin, transform.forward);
                _spriteGO.transform.localScale = new Vector3(Mathf.Abs(sizeVector.x), Mathf.Abs(sizeVector.x), 0.1f);
            }
            else
            {
                transform.SetPositionAndRotation(Code1.Position, Code1.Rotation);
                _spriteGO.transform.localScale = Code1.Scale;
            }

            _spriteGO.AddComponent<BoxCollider>();
            _objectLoader.GenerateCollection(_objectLoader.LoadSpritesFromStreamingAssets());

            return this;
        }

        /// <summary>
        /// Returns the photo ID. Supports photo with 1 and 2 QR code anchors.
        /// </summary>
        /// <param name="code1">QR code 1</param>
        /// <param name="code2">QR code 2 (optional)</param>
        /// <returns>photo ID</returns>
        public static string GetPhotoID(SHQRCode code1, SHQRCode code2 = null)
        {
            return code1.ID + (code2 == null ? "" : PHOTO_ID_SEPERATOR + code2.ID);
        }

        public static bool LoadSprite(string photoFilename, out Sprite sprite)
        {
            sprite = null;
            string photoFilePath = Path.Combine(Application.streamingAssetsPath, SHSettings.StreamingAssetsPath.PHOTOS, photoFilename);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
            try
            {
                byte[] photoBytes = File.ReadAllBytes(photoFilePath);
                if (ImageConversion.LoadImage(tex, photoBytes, true))
                {
                    sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width);
                    return true;
                }
                else throw new System.Exception("Could not load image.");
            }
            catch
            {
                HUD.PopupWarning("Could not load image");
                return false;
            }
        }

        [ContextMenu("Load Photo")]
        public async void LoadPhoto()
        {
            if (string.IsNullOrWhiteSpace(Code1.URL))
            {
                LoadSprite(PhotoFilename, out Sprite newSprite);
                LoadPhoto(newSprite);
            }
            else
            {
                Sprite urlSprite = await LoadSpriteFromURL(Code1.URL);
                if (urlSprite == null) return;
                LoadPhoto(urlSprite);
            }
        }

        public void LoadPhoto(string photoFilename)
        {
            PhotoFilename = photoFilename;
            LoadPhoto();
        }

        public void LoadPhoto(Sprite sprite)
        {
            _spriteGO.GetComponent<SpriteRenderer>().sprite = sprite;
            Sprite = sprite;
        }

        public async Task<Sprite> LoadSpriteFromURL(string url)
        {
            Sprite sprite = null;
            bool done = false;
            StartCoroutine(LoadSpriteFromWebRoutine(url, (returnedSprite) =>
            {
                sprite = returnedSprite;
                done = true;
            }));
            while (!done) await Task.Yield();

            return sprite;
        }

        private IEnumerator LoadSpriteFromWebRoutine(string url, System.Action<Sprite> callback)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError || !string.IsNullOrWhiteSpace(request.error) || request.responseCode >= 400)
            {
                HUD.PopupWarning($"Error loading sprite from URL: {url} - {request.error}");
                callback(null); // Notify the callback of failure
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);

                callback(sprite); // Notify the callback with the loaded sprite
            }
        }
    }
}