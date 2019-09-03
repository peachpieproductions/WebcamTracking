using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Webcam : MonoBehaviour
{


    public RawImage rawImage;
    public Vector2 spaceExtents;
    public ParticleSystem particleSys;

    WebCamTexture webcamTex;
    Color32[] previousFrame;
    List<int> changedPixelsList = new List<int>();
    float hue;

    private void Start() {
        webcamTex = new WebCamTexture();
        webcamTex.Play();
        rawImage.texture = webcamTex;

        //List all devices
        foreach(WebCamDevice d in WebCamTexture.devices) {
            Debug.Log(d.name);
        }

        //List chosen device
        Debug.Log(webcamTex.deviceName);

        var pixels = webcamTex.GetPixels32();
        Debug.Log(pixels.Length);
        Debug.Log(webcamTex.width + " x " + webcamTex.height);

        StartCoroutine(CompareImageData());
    }

    public IEnumerator CompareImageData() {

        previousFrame = webcamTex.GetPixels32();
        yield return null;

        while (true) {

            var currentFrame = webcamTex.GetPixels32();
            changedPixelsList.Clear();

            for (int i = 0; i < currentFrame.Length; i += 6) {
                Color currentPixel = currentFrame[i];
                Color lastPixel = previousFrame[i];
                var diff = Vector3.Distance(new Vector3(currentPixel.r, currentPixel.g, currentPixel.b), new Vector3(lastPixel.r, lastPixel.g, lastPixel.b));

                if (diff > .3f) changedPixelsList.Add(i);
            }
            Debug.Log(changedPixelsList.Count);

            int j = 0;
            foreach(int pixelId in changedPixelsList) {
                j++;
                int yCoord = Mathf.FloorToInt(pixelId / webcamTex.width);
                int xCoord = Mathf.FloorToInt(pixelId - yCoord * webcamTex.width);
                Vector2 coords = new Vector2(xCoord, yCoord);
                if (j > 100 && j < 110) Debug.Log(pixelId + ", " + xCoord + ", " + yCoord);
                if (Random.value < .02f) {
                    particleSys.transform.localPosition = new Vector3((coords.x / webcamTex.width) * spaceExtents.x, (coords.y / webcamTex.height) * spaceExtents.y, 0);
                    var main = particleSys.main;
                    particleSys.Emit(1);
                    
                    main.startColor = Color.HSVToRGB(hue, .7f, 1);
                }
                //Debug.Log(coords);
            }

            hue = (hue + Time.deltaTime * 1f) % 1f;

            previousFrame = currentFrame;

            yield return new WaitForSeconds(.1f);

            
        }
    }




}
