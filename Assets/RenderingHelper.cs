using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingHelper : MonoBehaviour
{
    Camera cam;

    public ComputeShader computeShader;
    RenderTexture renderTexture;

    ComputeBuffer sphereBuffer;

    private void Init()
    {
        cam = Camera.main;
    }

    private void initRenderTexture()
    {
        if (renderTexture == null || renderTexture.width != cam.pixelWidth || renderTexture.height != cam.pixelHeight)
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }
            renderTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Init();
        initRenderTexture();

        setScene();
        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.Dispatch(0, Mathf.CeilToInt(renderTexture.width / 8.0f), Mathf.CeilToInt(renderTexture.height / 8.0f), 1);

        Graphics.Blit(renderTexture, dest);
        sphereBuffer.Dispose();
    }


    void setScene()
    {
        computeShader.SetMatrix("inverseProjectionMatrix", cam.projectionMatrix.inverse);
        computeShader.SetMatrix("cameraToWorldMatrix", cam.cameraToWorldMatrix);
        createObjects();
    }

    void createObjects()
    {
        RaytracingSphere[] sphereObjects = FindObjectsOfType<RaytracingSphere>();
        Sphere[] spheres = new Sphere[sphereObjects.Length];

        for (int i = 0; i < spheres.Length; i++)
        {
            spheres[i] = new Sphere()
            {
                position = sphereObjects[i].transform.position,
                radius = sphereObjects[i].transform.localScale.x * 0.5f,
            };
        }

        sphereBuffer = new ComputeBuffer(spheres.Length, sizeof(float) * 4);
        sphereBuffer.SetData(spheres);
        computeShader.SetBuffer(0, "spheres", sphereBuffer);
        computeShader.SetInt("numSpheres", spheres.Length);
    }

    void OnDestroy()
    {
        if (sphereBuffer != null)
        {
            sphereBuffer.Dispose();
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}
