using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.MixedReality.QR;
using SH.XRHUD;
using SH.Utils;

namespace SH.QR
{
    public class QRCodeScanner : MonoBehaviour
    {
        public static QRCodeScanner Inst { get; private set; }

        private const int QR_CODE_RESCAN_DELAY_AFTER_ERROR = 5000; // in ms

        private static QRCodeWatcher _qrWatcher;
        private static HashSet<System.Guid> _scannedQRCodeIDs = new();

        private void Awake()
        {
            Inst = this;
        }

        private async void Start()
        {
            if (QRCodeWatcher.IsSupported())
            {
                QRCodeWatcherAccessStatus qrAccessStatus = await QRCodeWatcher.RequestAccessAsync();
                if (qrAccessStatus == QRCodeWatcherAccessStatus.Allowed)
                {
                    _qrWatcher = new QRCodeWatcher();
                    _qrWatcher.Updated += QRCodeUpdated;
                    _qrWatcher.Start();
                }
                else
                {
                    HUD.PopupWarning("QR codes scanning not allowed.");
                }
            }
            else
            {
                HUD.PopupWarning("QR codes not supported.");
            }
        }

        private void QRCodeUpdated(object sender, QRCodeUpdatedEventArgs e)
        {
            // Check if QRCode Id was already scanned (later a different (SHQRCode) ID is used)
            if (_scannedQRCodeIDs.Contains(e.Code.Id)) return;

            // New QR code scanned
            _scannedQRCodeIDs.Add(e.Code.Id);
            MainThreadQueue.Add(async (data) =>
            {
                try
                {
                    QRCodesManager.AddQRCode(data as QRCode);
                }
                catch (SHQRCodeException qrEx)
                {
                    if (!string.IsNullOrWhiteSpace(qrEx.Message)) HUD.PopupWarning(qrEx.Message);

                    await Task.Delay(QR_CODE_RESCAN_DELAY_AFTER_ERROR); // wait to prevent infinite popups hell
                    _scannedQRCodeIDs.Remove((data as QRCode).Id);
                }
                catch (System.Exception ex)
                {
                    HUD.PopupWarning(ex.Message);
                }
            }, e.Code);
        }
    }
}