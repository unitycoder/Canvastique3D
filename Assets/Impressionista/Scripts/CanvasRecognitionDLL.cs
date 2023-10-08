using System.Runtime.InteropServices;
using UnityEngine;

namespace Canvastique3D
{
    // This class is responsible for communicating with a DLL to perform canvas recognition.
    public class CanvasRecognitionDLL : MonoBehaviour
    {
        // Points used for canvas recognition
        private float[] points = new float[8];

        // Initializes the points array with default values based on video width and height
        public void InitializePoints(int videoWidth, int videoHeight)
        {
            // This array holds the values that will be overridden by the OpenCV plugin
            // with points defining the corners of the recognized canvas.
            points[0] = 0f;
            points[1] = 0f;
            points[2] = videoWidth;
            points[3] = 0f;
            points[4] = 0f;
            points[5] = videoHeight;
            points[6] = videoWidth;
            points[7] = videoHeight;
        }

        // Performs canvas recognition using the provided raw frame and parameters
        public void PerformRecognition(ref Color32[] rawFrame, int width, int height, int blurKSize, int threshValue, int threshMax, double epsilonFactor, float areaFactor, int step)
        {
            // Check the size of the array before calling the DLL function
            if (points.Length != 8)
            {
                Debug.LogError("Points array length is not 8!");
                return;
            }

            CanvasRecognition(ref rawFrame, width, height, blurKSize, threshValue, threshMax, epsilonFactor, areaFactor, step, ref points);
        }

        // Returns a reference to the points array
        public ref float[] Points => ref points;

        // Importing the CanvasRecognition function from the DLL
        [DllImport("CanvasRecognition")]
        private static extern void CanvasRecognition(ref Color32[] raw, int width, int height, int blurKSize, int threshValue, int threshMax, double epsilonFactor, float areaFactor, int step, ref float[] pts);
    }
}
