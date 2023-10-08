using Cinemachine;
using UnityEngine;

namespace Canvastique3D
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    class CinemachineZoom : MonoBehaviour
    {
        private CinemachineFreeLook freelook;
        private CinemachineFreeLook.Orbit[] originalOrbits;

        private CinemachineInputProvider inputProvider;

        [Tooltip("Sensitivity of zoom due to Z input")]
        public float zoomSensitivity = 0.05f;
        [Tooltip("Minimum allowed zoom percent")]
        public float minZoomPercent = 0.2f;
        [Tooltip("Maximum allowed zoom percent")]
        public float maxZoomPercent = 1.0f;

        private float currentZoomPercent = 1.0f;

        public void Awake()
        {
            freelook = GetComponentInChildren<CinemachineFreeLook>();
            inputProvider = GetComponent<CinemachineInputProvider>();

            originalOrbits = new CinemachineFreeLook.Orbit[freelook.m_Orbits.Length];
            for (int i = 0; i < freelook.m_Orbits.Length; i++)
            {
                originalOrbits[i].m_Height = freelook.m_Orbits[i].m_Height;
                originalOrbits[i].m_Radius = freelook.m_Orbits[i].m_Radius;
            }
        }

        public void Update()
        {
            if (inputProvider != null)
            {
                // Directly modify the currentZoomPercent based on the Z input.
                currentZoomPercent -= inputProvider.GetAxisValue(2) * zoomSensitivity; // Subtracting for intuitive zoom direction (scroll up to zoom in)
                currentZoomPercent = Mathf.Clamp(currentZoomPercent, minZoomPercent, maxZoomPercent);

                for (int i = 0; i < freelook.m_Orbits.Length; i++)
                {
                    freelook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * currentZoomPercent;
                    freelook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * currentZoomPercent;
                }
            }
        }
    }
}
