// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/TexBlur"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Bumpmap", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {

        Tags { "RenderType"="Opaque" }
        
        LOD 200
        
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        float _edgeLOD;
        

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _FoveationLUT;
        
        float _focusAreaPercent;
     

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float4 vertex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertex = v.vertex;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // get screen coordinate for the vertex
            float4 vertexNDC = UnityObjectToClipPos(IN.vertex);
            vertexNDC = vertexNDC / vertexNDC.w;
            vertexNDC = (vertexNDC + float4(1.0, 1.0, 1.0, 0.0) ) * 0.5;

            // fetch lod from the fovation lookup table through the screen cooridnate 
            float lod = tex2D(_FoveationLUT, float2(vertexNDC.x, 1-vertexNDC.y)).r;
            lod = saturate(lod / (_edgeLOD * _focusAreaPercent));


            // Albedo comes from a texture tinted by color
            fixed4 c = tex2Dlod(_MainTex, float4(IN.uv_MainTex.xy, 0.0, lod ))* _Color;

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Normal = UnpackNormal(tex2Dlod(_BumpMap, float4(IN.uv_BumpMap.xy, 0.0, lod  )));
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
