using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.OpenXR;
using SH.XRHUD;

namespace SH.QR
{
    [System.Serializable]
    public class SHQRCode
    {
        public static class TypeParamKeys
        {
            public static class General
            {
                public const string OPPOSITE_ORIGIN = "opposite_origin";
                public const string WIDTH = "width";
            }
            public static class Photo
            {
                public const string LINK_ID = "link_id";
            }
        }

        public const float DEFAULT_SPATIAL_DEPTH = 0.01f;

        // Parsed data infos
        [field: SerializeField] public string ID { get; private set; }
        [field: SerializeField] public SHContentType Type { get; private set; }
        [field: SerializeField] public string URL { get; private set; }
        [SerializeField] private Dictionary<string, string> _parameters; // see getter


        // Spatial infos
        [field: SerializeField] public Vector3 Origin { get; private set; }
        [field: SerializeField] public Vector3 Position { get; private set; }
        [field: SerializeField] public Quaternion Rotation { get; private set; }
        [field: SerializeField] public Vector3 Scale { get; private set; }
        private Pose _pose;

        /// <summary>
        /// If the QR code is invalid its Type is SHContentType.Invalid after instantiation.
        /// </summary>
        /// <param name="qrCode">Basic QRCode infos</param>
        public SHQRCode(QRCode qrCode)
        {
            try
            {
                // Parse QRCode data
                ParsedData.Parse(qrCode.Data, out ParsedData data);
                ID = data.ID;
                Type = data.Type;
                URL = data.URL;
                _parameters = data.Parameters;

                GetSpatialInfos(qrCode);

                PostProcessing();
            }
            catch (System.Exception e)
            {
                Type = SHContentType.Invalid;
                if (!string.IsNullOrWhiteSpace(e.Message)) HUD.PopupWarning(e.Message);
            }
        }

        /// <summary>
        /// On failure an error is thrown.
        /// </summary>
        private void GetSpatialInfos(QRCode qrCode)
        {
            SpatialGraphNode spatialGN = SpatialGraphNode.FromStaticNodeId(qrCode.SpatialGraphNodeId);
            if (spatialGN != null && spatialGN.TryLocate(FrameTime.OnUpdate, out Pose pose))
            {
                Origin = pose.position;
                Position = pose.position + (pose.right + pose.up) * (qrCode.PhysicalSideLength * 0.5f);
                Rotation = pose.rotation;
                Scale = new Vector3(qrCode.PhysicalSideLength, qrCode.PhysicalSideLength, DEFAULT_SPATIAL_DEPTH);
                _pose = pose;
            }
            else throw new SHQRCodeException();
        }

        /// <summary>
        /// On failure an error is thrown.
        /// </summary>
        private void PostProcessing()
        {
            // Handle general parameters (order matters due to changes to the spatial infos)::
            // opposite_origin:
            if (TryGetParam(TypeParamKeys.General.OPPOSITE_ORIGIN, out string opposite_origin))
            {
                if (opposite_origin == "true" || opposite_origin == "false")
                {
                    if (opposite_origin == "true")
                    {
                        Origin += (Position - Origin) * 2f;
                    }
                }
                else throw new SHQRCodeException("QR code is corrupt: " + TypeParamKeys.General.OPPOSITE_ORIGIN);
            }

            // width:
            if (TryGetParam(TypeParamKeys.General.WIDTH, out string width))
            {
                if (uint.TryParse(width, out uint cm))
                {
                    float m = cm / 100f;
                    Position = Origin + (_pose.right + _pose.up) * (m * 0.5f);
                    Scale = new Vector3(m, m, DEFAULT_SPATIAL_DEPTH);
                }
                else throw new SHQRCodeException("QR code is corrupt: " + TypeParamKeys.General.WIDTH);
            }

            // Handle type-specific parameters::
            switch (Type)
            {
                case SHContentType.Photo:
                    break;
                case SHContentType.Video:
                    if (string.IsNullOrWhiteSpace(URL)) throw new SHQRCodeException("Video QR code has no required url parameter.");
                    break;
                default:
                    throw new SHQRCodeException("QR code is invalid.");
            }
        }

        /// <summary>
        /// Try to get a parameter string by key.
        /// </summary>
        /// <param name="key">key string</param>
        /// <param name="param">parameter string output</param>
        /// <returns>true: key found, key not found or no parameters</returns>
        public bool TryGetParam(string key, out string param)
        {
            if (_parameters == null)
            {
                param = null;
                return false;
            }
            return _parameters.TryGetValue(key, out param);
        }

        [System.Serializable]
        private struct ParsedData
        {
            public const string VALID_HEADER = "SHQR";
            public const string PARAM_KEY_VALUE_SEPERATOR = ":";

            // From JSON
            [SerializeField] private string header;
            [SerializeField] private string id;
            [SerializeField] private string type;
            [SerializeField] private string url;
            [SerializeField] private string[] parameters;

            // Processed data
            public string ID;
            public SHContentType Type;
            public string URL;
            public Dictionary<string, string> Parameters;

            /// <summary>
            /// Parse the json string data.
            /// If parsing failed an error is thrown and Type is SHContentType.Invalid.
            /// </summary>
            /// <param name="data">json string data</param>
            /// <param name="parsedData">parsed data output</param>
            public static void Parse(string data, out ParsedData parsedData)
            {
                // Parse JSON string data
                try
                {
                    parsedData = JsonUtility.FromJson<ParsedData>(data);
                }
                catch
                {
                    parsedData = default(ParsedData);
                    throw new SHQRCodeException("QR code data is invalid: json string");
                }

                // Process data
                if (parsedData.header != VALID_HEADER) throw new SHQRCodeException("QR code data is invalid: header");
                if (string.IsNullOrWhiteSpace(parsedData.id)) throw new SHQRCodeException("QR code data is invalid: id");
                else parsedData.ID = parsedData.id;
                if (!System.Enum.TryParse(parsedData.type, out parsedData.Type)) throw new SHQRCodeException("QR code data is invalid: type");
                if (!string.IsNullOrWhiteSpace(parsedData.url)) parsedData.URL = parsedData.url;

                if (parsedData.parameters?.Length > 0)
                {
                    parsedData.Parameters = new Dictionary<string, string>();
                    foreach (string param in parsedData.parameters)
                    {
                        string[] keyValuePair = param.Split(PARAM_KEY_VALUE_SEPERATOR);
                        if (keyValuePair.Length != 2 || !parsedData.Parameters.TryAdd(keyValuePair[0], keyValuePair[1]))
                        {
                            parsedData.Type = SHContentType.Invalid;
                            throw new SHQRCodeException("QR code data is invalid: parameters");
                        }
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class SHQRCodeException : System.Exception
    {
        public SHQRCodeException(string message = "") : base(message) { }
        public SHQRCodeException(string message, System.Exception inner) : base(message, inner) { }
        protected SHQRCodeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
