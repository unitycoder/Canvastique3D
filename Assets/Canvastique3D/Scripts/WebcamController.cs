using System.Collections.Generic;
using UnityEngine;

namespace Canvastique3D
{
    // Handles operations related to the webcam
    public class WebcamController : MonoBehaviour
    {
        private WebCamTexture webcamTexture;

        private Color32[] webcamData;

        public void Init()
        {
            LoadCameraNames();
        }

        // Returns a list of available webcam device names
        private void LoadCameraNames()
        {
            List<string> cameraNames = new List<string>();

            foreach (var device in WebCamTexture.devices)
            {
                cameraNames.Add(device.name);
            }

            if(cameraNames.Count == 0)
            {
                EventManager.instance.TriggerWarning("No camera device found!");
            }

            EventManager.instance.TriggerCameraNames(cameraNames);
        }

        // Initializes the webcam with the given device name, resolution, and frames per second
        public void InitializeWebcam(string deviceName, string resolution, int fps)
        {
            if (webcamTexture != null)
            {
                Debug.LogWarning("Webcam is already initialized. Stop the current webcam before initializing a new one!");
                return;
            }

            var requestedWidth = int.Parse(resolution.Split("x")[0]);
            var requestedHeight = int.Parse(resolution.Split("x")[1]);

            webcamTexture = new WebCamTexture(deviceName, requestedWidth, requestedHeight, fps);
        }

        // Starts the webcam
        public void StartWebcam()
        {
            if (webcamTexture == null)
            {
                EventManager.instance.TriggerWarning("Webcam is not initialized!");
                return;
            }

            webcamTexture.Play();

            // If webcamData array already exists and has the correct size, no need to reallocate
            if (webcamData == null || webcamData.Length != webcamTexture.width * webcamTexture.height)
            {
                webcamData = new Color32[webcamTexture.width * webcamTexture.height];
            }

            EventManager.instance.TriggerCameraStarted();
        }

        // Stops the webcam and destroys the webcam texture
        public void StopWebcam()
        {
            if (webcamTexture != null)
            {
                webcamTexture.Stop();
                Destroy(webcamTexture);
                webcamTexture = null;

                EventManager.instance.TriggerCameraStopped();
            }
        }

        public void RestartWebcam()
        {
            if (webcamTexture != null)
            {
                webcamTexture.Stop();
                Destroy(webcamTexture);
                webcamTexture = null;

                StartWebcam();
            }
        }

        // Returns a raw frame from the webcam
        public Color32[] RawFrame()
        {
            if (webcamTexture != null)
            {
                return webcamTexture.GetPixels32(webcamData);
            }
            else
            {
                Debug.LogWarning("Webcam is not initialized!");
                return null;
            }
        }

        // Checks if the webcam is playing
        public bool IsPlaying()
        {
            return webcamTexture != null && webcamTexture.isPlaying;
        }

        // Returns the resolution of the webcam
        public string Resolution
        {
            get
            {
                if (webcamTexture != null)
                {
                    var resolution = webcamTexture.width + "x" + webcamTexture.height;
                    return resolution;
                }
                else
                {
                    Debug.LogWarning("Webcam is not initialized!");
                    return null;
                }
            }
        }

        // Getters for the webcam's width and height
        public int Width => webcamTexture.width;

        public int Height => webcamTexture.height;
    }
}
