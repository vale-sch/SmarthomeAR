#if UNITY_EDITOR_OSX

using UnityEngine;

namespace SH.Utils {
    public static class MacVSyncFix {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void Fix() {
            QualitySettings.vSyncCount = 2;
        }
    }
}

#endif
