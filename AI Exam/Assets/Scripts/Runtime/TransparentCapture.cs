using UnityEngine;
using System.IO;
using System.Collections;

public class TransparentCapture : MonoBehaviour
{
    public GameObject targetObject;
    public Camera screenshotCamera;
    public int imageWidth = 256;
    public int imageHeight = 256;
    public float maxAngle = 360f;
    public int numberOfAngles = 36;
    public float[] distances = new float[] { 3f, 5f, 8f, 10f };

    private void Start()
    {
        StartCoroutine(CaptureImages());
    }

    private IEnumerator CaptureImages()
    {
        RenderTexture renderTexture = new RenderTexture(imageWidth, imageHeight, 24, RenderTextureFormat.ARGB32);
        screenshotCamera.targetTexture = renderTexture;

        for (int d = 0; d < distances.Length; d++)
        {
            float distance = distances[d];
            for (int i = 0; i < numberOfAngles; i++)
            {
                float angle = (maxAngle / numberOfAngles) * i;
                Vector3 cameraPosition = targetObject.transform.position + new Vector3(distance * Mathf.Sin(angle * Mathf.Deg2Rad), 0.25f, distance * Mathf.Cos(angle * Mathf.Deg2Rad));
                screenshotCamera.transform.position = cameraPosition;
                screenshotCamera.transform.LookAt(targetObject.transform);
                yield return new WaitForEndOfFrame();
                float _distance = cameraPosition.magnitude;
                //Reduce to 2 decimal place
                _distance = Mathf.Round(_distance * 100f) / 100f;
                RenderTexture.active = renderTexture;
                Texture2D screenshot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGBA32, false);
                screenshot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
                screenshot.Apply();

                byte[] bytes = screenshot.EncodeToPNG();
                File.WriteAllBytes($"Screenshots/with_distance_labels/{targetObject.name}_{_distance}_angle{i}.png", bytes);

                RenderTexture.active = null;
            }
        }

        screenshotCamera.targetTexture = null;
        Destroy(renderTexture);
    }
}
