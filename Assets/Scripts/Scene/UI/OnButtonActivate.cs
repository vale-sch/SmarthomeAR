using UnityEngine;

public class OnButtonActivate : MonoBehaviour
{
    public void ActivateOnClick()
    {
        if (this.transform.parent)
            transform.parent = null;

        gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
