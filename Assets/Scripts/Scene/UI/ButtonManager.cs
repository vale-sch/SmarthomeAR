using UnityEngine;
using SH.Scene.Content;

namespace SH.Scene.UI
{
    public class ButtonManager : MonoBehaviour
    {
        public GameObject ObjectToFollow;
        public GameObject PictureContainer;
        public SCPhoto SCPhoto;

        private void Update()
        {
            // If the photo fades out the button also is disabled
            transform.SetPositionAndRotation(ObjectToFollow.transform.position, ObjectToFollow.transform.rotation);
            if (PictureContainer.activeSelf)
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
            else
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        public void SwitchSprite()
        {
            SpriteRenderer containerSpriteRenderer = PictureContainer.GetComponent<SpriteRenderer>();
            Sprite oldSprite = SCPhoto.Sprite;

            SCPhoto.LoadPhoto(containerSpriteRenderer.sprite);
            containerSpriteRenderer.sprite = oldSprite;
        }
    }
}
