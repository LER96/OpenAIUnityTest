using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;
using System.Linq;
using OpenAI;
using UnityEngine.Events;
using TMPro;

public class TestAnalyze : MonoBehaviour
{
    [SerializeField] RawImage _rawImage;
    [SerializeField] RenderTexture _photoTexture;
    [SerializeField] TMP_Text questionText;

    public OnResponseEvent responseEvent;

    #region RespondEventClass
    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }
    #endregion

    private OpenAIApi openAI = new OpenAIApi("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", "org-M0fYAK45nwDQPlnAYjDUcUig");
    private List<ChatMessage> messages = new List<ChatMessage>();

    void Start()
    {
        //Takin info from web cam
        WebCamTexture _webCamTexture = new WebCamTexture();
        Debug.Log(_webCamTexture.deviceName);

        //print it on quad object 
        _rawImage.texture = _webCamTexture;
        _webCamTexture.Play();
    }

    public async void AskGPT(string text)
    {
        ChatMessage chatMessage = new ChatMessage();
        chatMessage.Content = text;
        chatMessage.Role = "user";

        messages.Add(chatMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);
        if (response.Choices != null && response.Choices.Count > 0)
        {
            var charResponse = response.Choices[0].Message;
            messages.Add(charResponse);
            Debug.Log(charResponse.Content);

            responseEvent.Invoke(charResponse.Content);
        }

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

        string base64Image = System.Convert.ToBase64String(byteArray);

        string q = questionText + base64Image;

        AskGPT(q);
    }
}
