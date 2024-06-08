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
    private object _result;
    public object result
    {
        get { return _result; }
        set { _result = value; }
    }
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
    private List<float> embeddingResult;

    private static readonly string apiUrl = "https://api.openai.com/v1/embeddings";
    private static readonly List<string> actionTypes = new List<string> { "Emote", "Defense", "Attack" };

public void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void ReadStringInput(string input)
    {

        StartCoroutine(ProcessActionTypes());

        CoroutineWithData cd = new CoroutineWithData(this, GetVectorEmbedding(input));
        StartCoroutine(WaitForResult(cd));



    }

    private IEnumerator ProcessActionTypes()
    {
        foreach (string actionType in actionTypes)
        {
            CoroutineWithData actionTypesVectorized = new CoroutineWithData(this, GetVectorEmbedding(actionType));
            yield return actionTypesVectorized.coroutine;

            if (actionTypesVectorized.result != null)
            {
                List<float> actionTypeEmbedding = actionTypesVectorized.result as List<float>;
                Debug.Log("Embedding for " + actionType + " is " + string.Join(", ", actionTypeEmbedding));
            }
            else
            {
                Debug.LogError("Failed to get embedding for " + actionType);
            }
        }
    }

    private IEnumerator WaitForResult(CoroutineWithData cd)
    {
        yield return cd.coroutine;

        if (cd.result != null)
        {
            embeddingResult = cd.result as List<float>;
            Debug.Log("Result for input is: " + string.Join(", ", embeddingResult));

        }
        else
        {
            Debug.LogError("Failed to get embedding.");
        }
    }

    private IEnumerator GetVectorEmbedding(string text)
    {
        var task = Task.Run(() => GetEmbeddingAsync(text));
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Error: {task.Exception.Message}");
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

    public List<float> GetStoredEmbedding()
    {
        return embeddingResult;
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
