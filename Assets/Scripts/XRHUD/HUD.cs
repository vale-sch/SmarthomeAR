using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;

namespace SH.XRHUD
{
    public class HUD : MonoBehaviour
    {
        public static HUD Inst { get; private set; }

        [SerializeField] private GameObject _pfSmDialog;

        private void Awake()
        {
            Inst = this;
            PointerUtils.SetGazePointerBehavior(PointerBehavior.AlwaysOn);

      /*      Dialog dialogResponse = Dialog.Open(_pfSmDialog, DialogButtonType.Yes | DialogButtonType.No, "Handray Interaction", "Do you want to turn off the Handray System?" + "\n" + "Headtracking and Hand Interaction is always active.", true);
            dialogResponse.OnClosed += OnClosedDialogEvent;*/
        }

        public static void Popup(string msg, string title = "Info", System.Action<DialogResult> onClosed = null)
        {
            Debug.Log(title + ": " + msg);
            Dialog d = Dialog.Open(Inst._pfSmDialog, DialogButtonType.OK, title, msg, true);
            if (onClosed != null) d.OnClosed = onClosed;
        }

        public static void PopupWarning(string msg, System.Action<DialogResult> onClosed = null)
        {
            Debug.LogWarning("WARNING! " + msg);
            Dialog d = Dialog.Open(Inst._pfSmDialog, DialogButtonType.OK, "WARNING!", msg, true);
            if (onClosed != null) d.OnClosed = onClosed;
        }

      /*  private void OnClosedDialogEvent(DialogResult obj)
        {
            if (obj.Result == DialogButtonType.Yes)
            {
                PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff);
            }
        }*/
    }
}