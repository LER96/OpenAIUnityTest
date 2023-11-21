using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;


public class OpenAIManager : MonoBehaviour
{

    public OnResponseEvent responseEvent;

    [System.Serializable]
    public class OnResponseEvent: UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi("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", "org-M0fYAK45nwDQPlnAYjDUcUig");
    private List<ChatMessage> messages = new List<ChatMessage>();


    public async void AskGPT(string text)
    {
        ChatMessage chatMessage= new ChatMessage();
        chatMessage.Content = text;
        chatMessage.Role = "user";

        messages.Add(chatMessage);

        CreateChatCompletionRequest request= new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response= await openAI.CreateChatCompletion(request);
        if(response.Choices != null && response.Choices.Count>0)
        {
            var charResponse= response.Choices[0].Message;
            messages.Add(charResponse);
            Debug.Log(charResponse.Content);

            responseEvent.Invoke(charResponse.Content);
        }

    }
}
