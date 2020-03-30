// System pakages
using UnityEngine;
using UnityEngine.UI;

// This Class handle the orientation of the camera based on the orientation of phone
public class CameraScript : MonoBehaviour
{
    // Initial variables 
    private WebCamTexture backCamera;
    private bool cameraAvailable;
    private RawImage imageBackground;
    private AspectRatioFitter ratioFit;

    // Function the automatically is executed and define the back camera
    private void Start()
    {
        // Looking for possible cameras on the system
        WebCamDevice[] cameraList = WebCamTexture.devices;

        // Check if there are camera in the system
        if (cameraList.Length == 0)
        {
            Debug.Log("No camera detected");
            return;
        }

        // Loop through the possible cameras to set the back camera
        for (int i =0; i< cameraList.Length;i++)
        {
            if(!cameraList[i].isFrontFacing)
            {
                backCamera = new WebCamTexture(cameraList[i].name, Screen.width, Screen.height);
            }
        }

        // Check if a camera was select as the back camera
        if(backCamera == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        // Activate the back camera
        backCamera.Play();

        // Set the texture of the background as the back camera view
        imageBackground.texture = backCamera;

        // Activate variable that check if a camera is set up
        cameraAvailable = true;
    }

    //  Function that is called once per frame and fix the camera based on the orientation of the phone
    private void Update()
    {
        // Check if a camera is set up
        if (!cameraAvailable)
            return;

        // Update the ratio based on the orientation of the phone
        float ratio = (float)backCamera.width / (float)backCamera.height;
        ratioFit.aspectRatio = ratio;

        // Update the Y scale based on the orientation of the phone
        float scaleY = backCamera.videoVerticallyMirrored ? -1f : 1f;
        imageBackground.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        // Update the orientation based on the orientation of the phone
        int orientation = -backCamera.videoRotationAngle;
        imageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }
}
