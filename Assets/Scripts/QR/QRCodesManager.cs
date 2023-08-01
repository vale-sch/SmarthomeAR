using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.QR;
using SH.Scene;

namespace SH.QR
{
    public class QRCodesManager : MonoBehaviour
    {
        public static QRCodesManager Inst { get; private set; }

        // (SHQRCode) ID as string, SHQRCode
        private static Dictionary<string, SHQRCode> _shqrCodes = new();

        private void Awake()
        {
            Inst = this;
        }

        /// <summary>
        /// Adds a SHQRCode to the manager's dictionary.
        /// Throws SHQRCodeException if the QR code is invalid.
        /// </summary>
        /// <param name="shqrCode">SHQRCode</param>
        /// <returns>true: QR code added, false: QR code already added</returns>
        public static bool AddSHQRCode(SHQRCode shqrCode)
        {
            if (shqrCode.Type == SHContentType.Invalid) throw new SHQRCodeException();
            if (!_shqrCodes.ContainsKey(shqrCode.ID))
            {
                SHQRCodePostProcessing(shqrCode);

                _shqrCodes.Add(shqrCode.ID, shqrCode); // only add QR code to list after every process went through
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a SHQRCode to the manager's dictionary.
        /// Throws SHQRCodeException if the QR code is invalid.
        /// </summary>
        /// <param name="qrCode">Microsoft QRCode</param>
        /// <returns>true: QR code added, false: QR code already added</returns>
        public static bool AddQRCode(QRCode qrCode)
        {
            SHQRCode shqrCode = new SHQRCode(qrCode);
            return AddSHQRCode(shqrCode);
        }

        /// <summary>
        /// Remove a SHQRCode from the manager's dictionary.
        /// </summary>
        /// <param name="ID">(SHQRCode) ID</param>
        /// <returns>true: QR code removed, false: ID not found</returns>
        public static bool RemoveSHQRCode(string ID)
        {
            return _shqrCodes.Remove(ID);
        }

        /// <summary>
        /// Try to get a SHQRCode by ID.
        /// </summary>
        /// <param name="ID">(SHQRCode) ID</param>
        /// <param name="shqrCode">SHQRCode result, null if not found</param>
        /// <returns>true: ID found, false: ID not found</returns>
        public static bool TryGetSHQRCode(string ID, out SHQRCode shqrCode)
        {
            return _shqrCodes.TryGetValue(ID, out shqrCode);
        }

        /// <summary>
        /// Throws SHQRCodeException if the QR code is invalid.
        /// </summary>
        private static void SHQRCodePostProcessing(SHQRCode shqrCode)
        {
            switch (shqrCode.Type)
            {
                case SHContentType.Photo:
                    if (shqrCode.TryGetParam(SHQRCode.TypeParamKeys.Photo.LINK_ID, out string link_id))
                    {
                        // 2 QR code version
                        if (_shqrCodes.TryGetValue(link_id, out SHQRCode link_shqrCode))
                        {
                            // QR code partner found -> photo recognized
                            // Maintain the order of the QR codes by sorting them
                            if (string.Compare(shqrCode.ID, link_shqrCode.ID) < 0) SceneManager.AddPhoto(shqrCode, link_shqrCode);
                            else SceneManager.AddPhoto(link_shqrCode, shqrCode);
                        }
                    }
                    else
                    {
                        // 1 QR code version
                        SceneManager.AddPhoto(shqrCode);
                    }
                    break;
                case SHContentType.Video:
                    SceneManager.AddVideo(shqrCode);
                    break;
                default:
                    throw new SHQRCodeException("QR code is invalid.");
            }
        }
    }
}