using System.Collections.Generic;
using UnityEngine;
using SH.Scene.Content;
using SH.QR;

namespace SH.Scene
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager Inst { get; private set; }

        [SerializeField] private SCPhoto _pfPhoto;
        [SerializeField] private SCVideo _pfVideo;

        private static Dictionary<string, SCPhoto> _photos = new();
        private static Dictionary<string, SCVideo> _videos = new();

        private void Awake()
        {
            Inst = this;
        }

        private void Start()
        {
            // TODO load data from old session with BinaryFormatter
        }

        /// <summary>
        /// Add a photo to the scene. Supports photo with 1 and 2 QR code anchors.
        /// </summary>
        /// <param name="code1">QR code anchor 1</param>
        /// <param name="code2">QR code anchor 2 (optional)</param>
        /// <returns>true: photo added, false: photo already added</returns>
        public static bool AddPhoto(SHQRCode code1, SHQRCode code2 = null)
        {
            if (!_photos.ContainsKey(SCPhoto.GetPhotoID(code1, code2)))
            {
                SCPhoto photo = Instantiate(Inst._pfPhoto.gameObject, Inst.transform).GetComponent<SCPhoto>().Init(code1, code2);
                _photos.Add(photo.PhotoID, photo);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add a video to the scene.
        /// </summary>
        /// <param name="code">QR code anchor</param>
        /// <returns>true: video added, false: video already added</returns>
        public static bool AddVideo(SHQRCode code)
        {
            if (!_videos.ContainsKey(SCVideo.GetVideoID(code)))
            {
                SCVideo video = Instantiate(Inst._pfVideo.gameObject, Inst.transform).GetComponent<SCVideo>().Init(code);
                _videos.Add(video.VideoID, video);
                return true;
            }
            return false;
        }
    }
}
