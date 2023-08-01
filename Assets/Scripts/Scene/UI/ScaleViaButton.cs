using UnityEngine;

namespace SH.Scene.UI
{
    public class ScaleViaButton : MonoBehaviour
    {
        public void ScaleSprite(bool isScalePos)
        {
            float amountToScale;
            if (isScalePos)
                amountToScale = this.transform.localScale.x * 0.1f;
            else
                amountToScale = this.transform.localScale.x * -0.1f;

            this.transform.localScale = new Vector3(this.transform.localScale.x + amountToScale, this.transform.localScale.y + amountToScale, this.transform.localScale.z);

        }
    }
}
