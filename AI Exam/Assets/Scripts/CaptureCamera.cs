using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;  // Make sure you have a UI Image to display the texture

public class BarracudaInference : MonoBehaviour
{
    public NNModel modelAsset;
    private IWorker worker;
    public Camera cam;
    public RawImage debugImage;  // UI RawImage to display the texture

    void Start()
    {
        // Load the model and create a worker
        var model = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
        
    }

    string Classify_Image(){
        // Capture and process the image
        float[] inputTensor = CaptureAndProcessImage();
        
        // Create a tensor from the processed image
        Tensor input = new Tensor(1, 64, 64, 3, inputTensor);

        // Execute the model
        worker.Execute(input);

        // Get the output from our model
        Tensor output = worker.PeekOutput();
        float[] outputArray = output.ToReadOnlyArray();
        
        // Assuming output shape is (1, 1, 1, 2)
        //Debug.Log("Model output: " + string.Join(", ", outputArray));

        // Get classification result
        string prediction = GetClassIndex(outputArray);
        Debug.Log("Predicted class: " + prediction);

        // Display the captured and processed image for debugging
        // Texture2D debugTexture = ConvertToTexture2D(inputTensor, 64, 64);
        // debugImage.texture = debugTexture;

        // Cleanup
        input.Dispose();
        output.Dispose();
        
        return prediction;
    }
    string Classify_Image_With_Distance(){
        // Capture and process the image
        float[] inputTensor = CaptureAndProcessImage();

        
        // Create a tensor from the processed image
        Tensor input = new Tensor(1, 64, 64, 3, inputTensor);

        // Execute the model
        worker.Execute(input);

        // Get the output from our model
        Tensor output = worker.PeekOutput();
        float[] outputArray = output.ToReadOnlyArray();
        Tensor secondOutput = worker.PeekOutput("distance_output");
        float[] distance = secondOutput.ToReadOnlyArray();
        Debug.Log("Distance: " + distance[0]);
        // Assuming output shape is (1, 1, 1, 2)
        //Debug.Log("Model output: " + string.Join(", ", outputArray));

        // Get classification result
        string prediction = GetClassIndex(outputArray);
        Debug.Log("Predicted class: " + prediction);

        // Display the captured and processed image for debugging
        Texture2D debugTexture = ConvertToTexture2D(inputTensor, 64, 64);
        debugImage.texture = debugTexture;

        // Cleanup
        input.Dispose();
        output.Dispose();
        
        return prediction;
    }
    
    void OnDestroy()
    {
        worker.Dispose();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {  
            Debug.Log("Space key was pressed.");
            Classify_Image();
        }
        
    }
    
    private float[] CaptureAndProcessImage()
    {
        // Create a RenderTexture with the same size as the input expected by the model
        RenderTexture renderTexture = new RenderTexture(64, 64, 0);
        cam.targetTexture = renderTexture;
        cam.Render();

        // Create a new Texture2D to hold the captured image
        Texture2D texture = new Texture2D(64, 64, TextureFormat.RGB24, false);

        // Read pixels from the RenderTexture into the Texture2D
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, 64, 64), 0, 0);
        texture.Apply();

        // Cleanup
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(texture);

        // Convert the Texture2D to a format suitable for the model
        return ConvertToModelInput(texture);
    }

    private float[] ConvertToModelInput(Texture2D texture)
    {
        Color[] pixels = texture.GetPixels();
        float[] modelInput = new float[64 * 64 * 3];
        for (int i = 0; i < pixels.Length; i++)
        {
            modelInput[i * 3] = pixels[i].r ;
            modelInput[i * 3 + 1] = pixels[i].g ;
            modelInput[i * 3 + 2] = pixels[i].b ;
        }
        
        return modelInput;
    }
    

    private string GetClassIndex(float[] outputArray)
    {
       
        if (outputArray.Length != 5)
        {
            throw new System.ArgumentException("Output array length is not 2");
        }

        // Find the index of the highest value array of 5
        int maxIndex = 0;
        for (int i = 1; i < outputArray.Length; i++)
        {
            if (outputArray[i] > outputArray[maxIndex])
            {
                maxIndex = i;
            }
        }

        switch (maxIndex)
        {
            case 0:
                return "ball";
            case 1:
                return "bluePlayer";
            case 2:
                return "blueGoal";
            case 3: 
                return "purplePlayer";
            case 4:
                return "purpleGoal";
            default:
                return "Unknown";
        }
    }
    
    private void DebugInputTensor(float[] tensorData, int width, int height)
    {
        Debug.Log("Debugging Input Tensor:");
        for (int i = 0; i < height; i++)
        {
            string row = "";
            for (int j = 0; j < width; j++)
            {
                int index = (i * width + j) * 3;
                row += $"({tensorData[index]:F2}, {tensorData[index + 1]:F2}, {tensorData[index + 2]:F2}) ";
            }
            Debug.Log(row);
        }
    }
    
    private Texture2D ConvertToTexture2D(float[] tensorData, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            float r = tensorData[i * 3];
            float g = tensorData[i * 3 + 1];
            float b = tensorData[i * 3 + 2];
            pixels[i] = new Color(r, g, b, 1.0f);  // Assuming the tensor data is normalized
        }
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
