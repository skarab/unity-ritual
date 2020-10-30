using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public class SceneParticles : MonoBehaviour
{
    public GameObject CameraParticlesDepth;
    public GameObject CameraParticlesNormals;
    public GameObject CameraCompositingPass;
    public RectTransform Render;
    public Camera ParticlesCamDepth;
    public Camera ParticlesCamNormals;
    public Material ParticlesCamNormalsMat;
    public Material CompositingPass;
    public Transform DoorLeft;
    public Transform DoorRight;
    public VisualEffect Particles;
    public Transform LightTrs;
    public Transform LightTrs2;
    public Transform FinalDoor;
    public Material CarpetMat;
    public float CarpetTexSpeed;
    public Transform[] CarpetRolls;
    public float CarpetRollSpeed;
    public Material FinalRender;
    public VisualEffect LogoParticles;
    public AudioSource Music;
    public RawImage Credits;

    private const float _FadeInTime = 20.0f;
    private const float _FadeOutTime = 6.0f;

    private void Awake()
    {
        CameraParticlesDepth.SetActive(true);
        CameraParticlesNormals.SetActive(true);
        CameraCompositingPass.SetActive(true);
    }

    void Start()
    {
        RenderTexture particles_depth = new RenderTexture((int)Render.rect.width, (int)Render.rect.height, 0, RenderTextureFormat.RFloat);
        particles_depth.Create();
        ParticlesCamDepth.targetTexture = particles_depth;
        ParticlesCamNormalsMat.SetTexture("_ParticlesDepth", particles_depth);
        CompositingPass.SetTexture("_ParticlesDepth", particles_depth);
        
        RenderTexture particles_normal = new RenderTexture((int)Render.rect.width, (int)Render.rect.height, 0, RenderTextureFormat.ARGBFloat);
        particles_normal.Create();
        ParticlesCamNormals.targetTexture = particles_normal;
        CompositingPass.SetTexture("_ParticlesNormals", particles_normal);

        Particles.SetFloat("LogoRate", 0.0f);
        LogoParticles.SetFloat("LogoRate", 0.0f);

        FinalRender.SetFloat("_FadeOut", 1.0f);

        Credits.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }
    
    void Update()
    {
        DoorLeft.position = new Vector3(1.5f+(Mathf.Sin(Music.time*0.2f)*0.5f+0.5f)*1.5f, DoorLeft.position.y, DoorLeft.position.z);
        DoorRight.position = new Vector3(-1.5f-(Mathf.Sin(Music.time*0.2f)*0.5f+0.5f)*1.5f, DoorRight.position.y, DoorRight.position.z);
        Particles.SetVector3("DoorLeft", DoorLeft.position);
        Particles.SetVector3("DoorRight", DoorRight.position);

        Particles.SetFloat("Rate", 8000.0f+Mathf.Sin(Music.time*0.1f)*8000.0f);

        CompositingPass.SetVector("_LightDirection", LightTrs.forward);
        CompositingPass.SetVector("_LightDirection2", LightTrs2.forward);

        if (Music.time>=103.0f)
            FinalDoor.position = new Vector3(FinalDoor.position.x, Mathf.MoveTowards(FinalDoor.position.y, -8.0f, Time.deltaTime*0.5f), FinalDoor.position.z);
        Particles.SetVector3("FinalDoor", FinalDoor.position);        

        if (Music.time>=108.0f)
            Particles.SetFloat("LogoRate", Mathf.Lerp(Particles.GetFloat("LogoRate"), 100000.0f, Time.deltaTime*10.0f));
        if (Music.time>=122.0f)
            LogoParticles.SetFloat("LogoRate", Mathf.Lerp(LogoParticles.GetFloat("LogoRate"), 100000.0f, Time.deltaTime*10.0f));

        CarpetMat.SetTextureOffset("_BaseColorMap", new Vector2(CarpetMat.GetTextureOffset("_BaseColorMap").x, CarpetMat.GetTextureOffset("_BaseColorMap").y-Time.deltaTime*CarpetTexSpeed));
        
        for (int i=0 ; i<CarpetRolls.Length ; ++i)
        {
            for (int j=0 ; j<CarpetRolls[i].childCount ; ++j)
                CarpetRolls[i].GetChild(j).Rotate(new Vector3(0.0f, 0.0f, Time.deltaTime*CarpetRollSpeed));
        }

        // usual fade & scenes switch
        if (Music.isPlaying && Music.time<_FadeInTime*2.0f)
        {
            FinalRender.SetFloat("_FadeOut", Mathf.MoveTowards(FinalRender.GetFloat("_FadeOut"), 0.0f, Time.deltaTime*(1.0f/_FadeInTime)));
        }
        else if (Music.time>=167.0f-_FadeOutTime)
        {
            FinalRender.SetFloat("_FadeOut", Mathf.MoveTowards(FinalRender.GetFloat("_FadeOut"), 1.0f, Time.deltaTime*(1.0f/_FadeOutTime)));
        }

        if (Music.time>=141.2f)
            Credits.color = new Color(1.0f, 1.0f, 1.0f, Mathf.MoveTowards(Credits.color.a, 1.0f, Time.deltaTime*2.0f));
    }
}
