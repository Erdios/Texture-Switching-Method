using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Foveate : MonoBehaviour
{
    private const int MAXIMUM_BUFFER_SIZE = 8192;

    public enum Eye
    {
        RIGHT, LEFT
    }
    public Eye myeye;
    public ReadEyeTrackingSample eyeData;
    public GameObject coneParent;
    //public variables
    public bool isLinearColorspace;
    public float foveaX;
    public float foveaY;
    public float foveationAlpha;
    public float focusAreaRadius;
    public MaterialsManager materialsManager;


    //private variables
    private int m_size;
    private Camera m_camera;
    private FoveationLUT foveationLUT;
    private RenderTexture m_input;
    private RenderTexture m_loPass;


    

    #region shaders
    private Shader m_colorShader;
    public Shader colorShader
    {
        get
        {
            if (m_colorShader == null)
                m_colorShader = Shader.Find("Hidden/ColorProcess");

            return m_colorShader;
        }
    }


    private Material m_colorMaterial;
    public Material colorMaterial
    {
        get
        {
            if (m_colorMaterial == null)
            {
                if (colorShader == null || colorShader.isSupported == false)
                    return null;

                m_colorMaterial = new Material(colorShader);
            }

            return m_colorMaterial;
        }
    }


    private Shader m_meanShader;
    public Shader meanShader
    {
        get
        {
            if (m_meanShader == null)
                m_meanShader = Shader.Find("Hidden/MeanMips");

            return m_meanShader;
        }
    }

    private Material m_meanMaterial;
    public Material meanMaterial
    {
        get
        {
            if (m_meanMaterial == null)
            {
                if (meanShader == null || meanShader.isSupported == false)
                    return null;

                m_meanMaterial = new Material(meanShader);
            }

            return m_meanMaterial;
        }
    }

    private Shader m_textureBlurShader;
   

    

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        foveationLUT = gameObject.AddComponent<FoveationLUT>();
        foveationLUT.Foveate = this;
    }

    void Start()
    {
        
    }

    // update is called once per frame
    void Update()
    {

        foveationLUT.Alpha = foveationAlpha;
        foveationLUT.Updating = true;
        foveationLUT.DownsampleFactor = 1;
        foveationLUT.Mode = FoveationMode.QUADRATIC;

        ReadEyeTrackingSample.RayInfo ri;
        if (myeye == Eye.RIGHT)
            ri = eyeData.getRightRay();
        else
            ri = eyeData.getleftRay();

        // perspective divide not needed, its a vector, will always project to the same pixel in screen space.
        if (m_camera == null) m_camera = GetComponent<Camera>();

        Vector3 pos = m_camera.projectionMatrix * ri.dir;
        foveaX = (1 + pos.x) / 2.0f;
        foveaY = (1 + pos.y) / 2.0f;


        // adjust the collider for the user's fovea area
        if (coneParent != null)
        {

            coneParent.transform.position = new Vector3(0, 0, 0);
            coneParent.transform.LookAt(transform.localToWorldMatrix * ri.dir);
            coneParent.transform.position = transform.position;
        }
    }

    // For blurring textures
    void OnPreRender()
    {
        /*if (materialsManager.peripheryCount > 0)
        {
            materialsManager.peripheryMat.SetTexture("_FoveationLUT", foveationLUT.EccentricityTex);
            materialsManager.peripheryMat.SetFloat("_edgeLOD", foveationLUT.edgeLOD);
        }*/   

        foreach (KeyValuePair<Material, int> material in materialsManager.materials)
        {
            if (material.Value > 0)
            {
                
                material.Key.SetTexture("_FoveationLUT", foveationLUT.EccentricityTex);
                material.Key.SetFloat("_edgeLOD", foveationLUT.edgeLOD);

                if (coneParent != null)
                {
                    material.Key.SetFloat("_focusAreaPercent", coneParent.GetComponentInChildren<ConeCollider>().angle / m_camera.fieldOfView);
                    //material.Key.SetFloat("_focusAreaPercent", 0.65f);
                }
                
            }
        }        
    }


    private void InitializeTextures()
    {
        m_input = new RenderTexture(m_size, m_size, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        m_input.filterMode = FilterMode.Trilinear;
        m_input.useMipMap = true;
        m_input.autoGenerateMips = true;
        m_input.Create();
        m_input.hideFlags = HideFlags.HideAndDontSave;

        m_loPass = new RenderTexture(m_size, m_size, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        m_loPass.filterMode = FilterMode.Trilinear;
        m_loPass.useMipMap = false;
        m_loPass.autoGenerateMips = false;
        m_loPass.Create();
        m_loPass.hideFlags = HideFlags.HideAndDontSave;
    }


/*    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_camera == null)
        {
            m_camera = GetComponent<Camera>();
        }
        int size = (int)Mathf.Max((float)m_camera.pixelWidth, (float)m_camera.pixelHeight);
        size = (int)Mathf.Min((float)Mathf.NextPowerOfTwo(size), (float)MAXIMUM_BUFFER_SIZE);
        if (size != m_size)
        {
            m_size = size;
            InitializeTextures();
        }
        BlurPeriphery(source, destination);

    }*/

    public void BlurPeriphery(RenderTexture source, RenderTexture destination)
    {
        colorMaterial.SetInt("_direction", 1);
        colorMaterial.SetInt("_isLinearColorSpace", isLinearColorspace ? 1 : 0);

        colorMaterial.SetFloat("_screenWidth", m_camera.pixelWidth);
        colorMaterial.SetFloat("_screenHeight", m_camera.pixelHeight);
        colorMaterial.SetFloat("_texSize", m_size);
        //print(m_size);

        Graphics.SetRenderTarget(m_input);
        Graphics.Blit(source, colorMaterial);

        meanMaterial.SetVector("_TexelSize", Vector2.one / (float)(m_size - 1));
        meanMaterial.SetTexture("_FoveationLUT", foveationLUT.EccentricityTex);
        meanMaterial.SetFloat("_screenWidth", m_camera.pixelWidth);
        meanMaterial.SetFloat("_screenHeight", m_camera.pixelHeight);
        meanMaterial.SetFloat("_texSize", m_size);

        Graphics.SetRenderTarget(m_loPass, 0);
        Graphics.Blit(m_input, meanMaterial);

        colorMaterial.SetFloat("_LOD", (float)0);

        colorMaterial.SetInt("_direction", -1);
        colorMaterial.SetInt("_isLinearColorSpace", isLinearColorspace ? 1 : 0);
        colorMaterial.SetFloat("_screenWidth", m_camera.pixelWidth);
        colorMaterial.SetFloat("_screenHeight", m_camera.pixelHeight);
        colorMaterial.SetFloat("_texSize", m_size);

        Graphics.Blit(m_loPass, destination, colorMaterial);
        //Calculate the mean (by looking up a lower mip level). Result doesn't need mips.
    }




    void OnDestroy()
    {
     
    }
}
