using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoveationMode
{
    LINEAR,
    QUADRATIC
}

public class FoveationLUT : MonoBehaviour
{

    private Camera camera;

    public RenderTexture eccentricityTex;
    // This sets the amount the resolution is reduced in this LUT
    // A factor of 1 will give original camera resolution, 2 will give half resolution, etc.
    private int downsampleFactor = 1;

    private float alpha = 0.1f;


    private FoveationMode mode;


    private Foveate foveate;
    private bool updating;

    private Shader m_shader;



    [HideInInspector] public float edgeLOD;
    private bool needUpdateEdgeLOD = false;

    public Shader shader
    {
        get
        {
            if (m_shader == null)
                m_shader = Shader.Find("Hidden/FoveationLUT");

            return m_shader;
        }
    }

    private Material m_material;
    public Material material
    {
        get
        {
            if (m_material == null)
            {
                if (shader == null || shader.isSupported == false)
                    return null;
                m_material = new Material(shader);
            }
            return m_material;
        }
    }

    public Foveate Foveate { get => foveate; set => foveate = value; }
    public bool Updating { get => updating; set => updating = value; }
    public float Alpha
    {
        get => alpha;
        set
        {
            if (alpha != value)
            {
                // update alpha
                alpha = value;

                // !! if alpha changed, the edge LOD have to be updated
                needUpdateEdgeLOD = true;

                // generate a eccentricity texture with fixation position at the center of the screen
                if (Foveate.myeye == Foveate.Eye.LEFT)
                    UpdateEccentricityTex(new Vector2(0.5f, 0.5f));
                else
                    UpdateEccentricityTex(new Vector2(0.45f, 0.5f));

                // load lod from eccentricity texture
                Texture2D tex = new Texture2D(eccentricityTex.width, eccentricityTex.height, TextureFormat.RFloat, false);
                tex.ReadPixels(new Rect(0, 0, eccentricityTex.width, eccentricityTex.height), 0, 0);

                // update edge lod
                edgeLOD = tex.GetPixel(0, 0).r;


                // set flage to false
                needUpdateEdgeLOD = false;

            }
        }
    }

    public int DownsampleFactor { get => downsampleFactor; set => downsampleFactor = value; }
    public RenderTexture EccentricityTex { get => eccentricityTex; set => eccentricityTex = value; }
    public FoveationMode Mode { get => mode; set => mode = value; }

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        if(EccentricityTex != null)
        {
            EccentricityTex.Release();
        }

        EccentricityTex = new RenderTexture(
            camera.pixelWidth / DownsampleFactor,
            camera.pixelHeight / DownsampleFactor,
            0, RenderTextureFormat.RFloat);

    }

    // Update is called once per frame
    void Update()
    {

        if (eccentricityTex.width != camera.pixelWidth)
        {
            Start();

            // !! if alpha changed, the edge LOD have to be updated
            needUpdateEdgeLOD = true;

            // generate a eccentricity texture with fixation position at the center of the screen
            if (Foveate.myeye == Foveate.Eye.LEFT)
                UpdateEccentricityTex(new Vector2(0.5f, 0.5f));
            else
                UpdateEccentricityTex(new Vector2(0.45f, 0.5f));

            // load lod from eccentricity texture
            Texture2D tex = new Texture2D(eccentricityTex.width, eccentricityTex.height, TextureFormat.RFloat, false);
            tex.ReadPixels(new Rect(0, 0, eccentricityTex.width, eccentricityTex.height), 0, 0);

            // update edge lod
            edgeLOD = tex.GetPixel(0, 0).r;

            // set flage to false
            needUpdateEdgeLOD = false;
        }



        if (Updating && needUpdateEdgeLOD == false)
            UpdateEccentricityTex(new Vector2(Foveate.foveaX, Foveate.foveaY));

    }

 

    public void UpdateEccentricityTex(Vector2 fixationPos)
    {
        Matrix4x4 projMat = camera.projectionMatrix;
        Matrix4x4 invProjMat = projMat.inverse;

        Vector4 fixationVec = invProjMat * new Vector4(fixationPos.x * 2.0f - 1.0f, fixationPos.y * 2.0f - 1.0f, 1.0f, 1.0f);
        fixationVec /= Mathf.Sqrt(fixationVec.x * fixationVec.x + fixationVec.y * fixationVec.y + fixationVec.z * fixationVec.z);

        

        material.SetMatrix("invProjMat", invProjMat);
        material.SetVector("fixationVec", fixationVec);
        material.SetFloat("alpha", Alpha);
        int modeInt = Mode == FoveationMode.LINEAR ? 0 : 1;
        material.SetInt("mode", modeInt);
        material.SetFloat("planeDist", 1.0f / Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView * 0.5f));
        material.SetFloat("imPixelSize", 0.5f * (camera.pixelWidth + camera.pixelHeight));

        Graphics.SetRenderTarget(EccentricityTex);
        Graphics.Blit(null, material);
    }
}
