using UnityEngine;
using System;
using System.IO;

namespace Crystal
{
    public class SafeAreaDemo : MonoBehaviour
    {
        [SerializeField] KeyCode KeySafeArea = KeyCode.A;
        [SerializeField] KeyCode KeyScreenShots = KeyCode.S;
        public bool useTexture2D = true;
#if SOFTCEN_DEBUG && UNITY_EDITOR
        SafeArea.SimDevice[] Sims;
        int SimIdx;
        private bool takingScreenshots;
        private int takingIndex;
        private bool takePicture;
        private bool grap = false;
        public string storePath;
        public string filename;

        public string[] resolutions =
        {
        "1284x2778", // 6.5"
        "1242x2208", // 5.5"
        "2048x2732", // 12.9"
        };


        void Awake ()
        {
            takingScreenshots = false;
            if (!Application.isEditor)
                Destroy (this);

            Sims = (SafeArea.SimDevice[])Enum.GetValues (typeof (SafeArea.SimDevice));
        }

        void Update ()
        {
            if (takingScreenshots)
            {
                if (!takePicture)
                {
                    if (takingIndex >= resolutions.Length)
                    {
                        takingScreenshots = false;
                        return;
                    }
                    if (takingIndex == 0)
                    {
                        SafeArea.Sim = Sims[1];
                        Debug.LogFormat("Switched to sim device index {0} {1}", SimIdx, Sims[1]);
                    } else
                    {
                        SafeArea.Sim = Sims[0];
                        Debug.LogFormat("Switched to sim device index {0} {1}", SimIdx, Sims[0]);
                    }
                    Debug.Log("Set resolution: " + resolutions[takingIndex]);
                    GameViewUtils.TrySetSize(resolutions[takingIndex]);
                    takePicture = true;
                }
                else if (grap != true)
                {
                    Debug.Log("Grap true");
                    grap = true;
                }
                return;
            }

            if (Input.GetKeyDown (KeySafeArea))
                ToggleSafeArea ();
            if (Input.GetKeyDown(KeyScreenShots))
            {
                Debug.Log("Start taking screenshots");
                takingScreenshots = true;
                takingIndex = 0;
                takePicture = false;
                grap = false;
            }
        }

        /// <summary>
        /// Toggle the safe area simulation device.
        /// </summary>
        void ToggleSafeArea ()
        {
            SimIdx++;

            if (SimIdx >= Sims.Length)
                SimIdx = 0;

            SafeArea.Sim = Sims[SimIdx];
            Debug.LogFormat ("Switched to sim device index {0} {1} with debug key '{2}'", SimIdx, Sims[SimIdx], KeySafeArea);
        }

        void OnPostRender()
        {
            if (grap)
            {
                string timeStr = System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                if (useTexture2D)
                {
                    Texture2D sshot = new Texture2D(Screen.width, Screen.height);
                    sshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                    sshot.Apply();
                    byte[] pngShot = sshot.EncodeToPNG();
                    Destroy(sshot);
                    string fullName = storePath + filename + "_" + timeStr + "_" + resolutions[takingIndex] + ".png";
                    File.WriteAllBytes(fullName, pngShot);
                    Debug.Log("Save " + fullName);
                } else
                {
                    string fullName2 = storePath + filename + "_" + timeStr + "_" + resolutions[takingIndex] + "_B.png";
                    ScreenCapture.CaptureScreenshot(fullName2);
                    Debug.Log("Save " + fullName2);
                }
                grap = false;
                takePicture = false;
                takingIndex++;
            }

        }

#endif
    }
}
