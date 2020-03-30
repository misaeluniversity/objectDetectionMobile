// System pakages
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// This Class handle the process of taking the photo and interact with the recognition service and voice generator
public class RESTapi : MonoBehaviour
{
    // Initial variables 
    private AudioSource audioSource;
    private RawImage background;
    private String labels;

    // Function the automatically is executed and define the audio source
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Function that is trigger by the front button on the app
    public void Recognition()
    {
        // It execute the request to the AWS Lambda function
        StartCoroutine(GetRequest("https://7fyk5orvj6.execute-api.us-east-2.amazonaws.com/default/recogntion"));
    }

    // Function the request the audio clip with the labels of the objects recognized on the picture
    IEnumerator GetAudioClip()
    {
        // Execute a web request with the labels of the objects recognized on the picture
        using (UnityWebRequest webRequest = UnityWebRequestMultimedia.GetAudioClip("https://translate.google.com/" +
            "translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q="+labels+"&tl=En-gb", AudioType.MPEG))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                // Download the audio clip of the labels
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(webRequest);
                // Setting the clip in the audio source
                audioSource.clip = audioClip;
                // Playing audio clip
                audioSource.Play();
            }
        }
    }

    // Function the request the labels of the objects recognized on the picture
    IEnumerator GetRequest(string uri)
    {
        // Initial web form
        WWWForm webForm = new WWWForm();

        // Path to the picture
        string filename = "picture.png";
        string pictureDirectory = Path.Combine(Application.persistentDataPath, filename);

        // Creating screenshot
        ScreenCapture.CaptureScreenshot("picture.png");
        // Reading picture from file
        byte[] file = File.ReadAllBytes(pictureDirectory);
        // Convert picture to string
        string base64String = Convert.ToBase64String(file);
        // Setting the form with the picture
        webForm.AddField("body", base64String);
        // Execute a web request with the form
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, webForm))
        {
            // Wait for result of the object recognition service.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (System.Collections.Generic.KeyValuePair<string, string> dict in webRequest.GetResponseHeaders())
                {
                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                }

                Dictionary<string, string> responseHeaders = webRequest.GetResponseHeaders();
                labels = webRequest.downloadHandler.text;
                StartCoroutine(GetAudioClip());

            }
        }
    }

}

