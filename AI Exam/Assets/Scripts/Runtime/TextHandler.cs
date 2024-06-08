using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

public class CoroutineWithData
{
    public Coroutine coroutine { get; private set; }
    public object result;
    private IEnumerator target;

    public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
    {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (target.MoveNext())
        {
            result = target.Current;
            yield return result;
        }
    }
}

public class TextHandler : MonoBehaviour
{
    public string input;

    private static readonly string apiUrl = "https://api.openai.com/v1/embeddings";

    public void Start()
    {

    }

    private void Update()
    {

    }

    public void ReadStringInput(string s)
    {
        input = s;

        CoroutineWithData cd = new CoroutineWithData(this, GetVectorEmbedding(input));
        Debug.Log("result is " + cd.result);  

        // var vectorEmbedding = StartCoroutine(GetVectorEmbedding(input));
        // Debug.Log(vectorEmbedding);

    }

    private System.Collections.IEnumerator GetVectorEmbedding(string text)
    {
        var task = Task.Run(() => GetEmbeddingAsync(text));
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
          
            yield break;
        }

        var embedding = task.Result;
        yield return embedding;
    }

    private async Task<List<float>> GetEmbeddingAsync(string text)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                input = text,
                model = "text-embedding-3-small"
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError($"Error: {response.StatusCode}");
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<OpenAIResponse>(responseString);

            return responseObject.data[0].embedding;
        }
    }
}

public class OpenAIResponse
{
    public List<OpenAIData> data { get; set; }
}

public class OpenAIData
{
    public List<float> embedding { get; set; }
}
