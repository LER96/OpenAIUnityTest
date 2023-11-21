using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;
using System.Linq;

public class WebCamShoot : MonoBehaviour
{
    [SerializeField] RawImage _rawImage;
    [SerializeField] RenderTexture _photoTexture;

    void Start()
    {
        //Takin info from web cam
        WebCamTexture _webCamTexture = new WebCamTexture();
        Debug.Log(_webCamTexture.deviceName);

        //print it on quad object 
        _rawImage.texture = _webCamTexture;
        _webCamTexture.Play();
    }

    public void TakePicture()
    {
        RenderTexture.active = _photoTexture;
        Graphics.CopyTexture(_rawImage.texture, _photoTexture);
        Texture2D _rendderResult = new Texture2D(_photoTexture.width, _photoTexture.height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, _photoTexture.width, _photoTexture.height);
        _rendderResult.ReadPixels(rect, 0, 0);
        _rendderResult.Apply();

        byte[] byteArray = _rendderResult.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/CameraShot.png", byteArray);
    }
}
