using System.IO;
using System.Collections;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using SH.Scene.Content;

namespace SH.Scene.UI
{
    public class ObjectLoader : MonoBehaviour
    {
        [SerializeField] private GameObject picturePrefab;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject buttonsContainer;

        private FileInfo[] _allFiles;
        public SCPhoto SCPhoto;

        private void Awake()
        {
            GenerateCollection(LoadSpritesFromStreamingAssets());
        }

        public void GenerateCollection(int amountOfLoadableSprites)
        {
            for (int i = 0; i < amountOfLoadableSprites; i++)
            {
                var photoInstance = Instantiate(picturePrefab);
                var buttonInstance = Instantiate(buttonPrefab);

                SCPhoto.LoadSprite(_allFiles[i].FullName, out Sprite sprite);
                photoInstance.GetComponentInChildren<SpriteRenderer>().sprite = sprite;

                buttonInstance.GetComponent<ButtonManager>().PictureContainer = photoInstance.transform.GetChild(0).gameObject;
                buttonInstance.GetComponent<ButtonManager>().ObjectToFollow = photoInstance.transform.GetChild(1).gameObject;
                buttonInstance.GetComponent<ButtonManager>().SCPhoto = SCPhoto;

                buttonInstance.transform.SetParent(buttonsContainer.transform, false);
                photoInstance.transform.SetParent(transform, false);
            }
            GetComponent<GridObjectCollection>().UpdateCollection();
        }

        public int LoadSpritesFromStreamingAssets()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, SHSettings.StreamingAssetsPath.PHOTOS));
            _allFiles = directoryInfo.GetFiles("*.jpg");
            return _allFiles.Length;
        }

        public void scrollPictures(float amount)
        {
            StartCoroutine(scrollPicturesRoutine(amount));
        }

        private IEnumerator scrollPicturesRoutine(float amount)
        {
            float xToBeReached = transform.localPosition.x + amount;
            float stepSpeed = 1f;
            while (true)
            {
                stepSpeed -= 0.005f;
                float step = stepSpeed * Time.deltaTime; // Adjust the speed of scrolling
                if (amount > 0)
                {
                    if (xToBeReached <= transform.localPosition.x + 0.1f)
                        break;
                }
                else
                {
                    if (xToBeReached >= transform.localPosition.x - 0.1f)
                        break;
                }
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x + amount, transform.localPosition.y, transform.localPosition.z), step);
                yield return null;
            }
        }
    }
}