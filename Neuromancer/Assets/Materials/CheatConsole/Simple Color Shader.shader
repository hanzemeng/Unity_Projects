Shader "DeveloperConsole/Simple_Color_Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        
        [KeywordEnum(Off, Blunt, Sharp, Neutral)]
        _Options ("Obstacle Color", Float) = 0
    }
    SubShader
    {
        Tags { 
        "RenderType"="Transparent" 
        "Queue" = "Transparent"
        // "RenderPipeline" = "UniveralRenderPipeline"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            // Declare pragma
            #pragma multi_compile _OPTIONS_OFF _OPTIONS_BLUNT _OPTIONS_SHARP _OPTIONS_NEUTRAL
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            // Defines connection variables within CGPROGRAM
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _EmissionColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                #if _OPTIONS_OFF
                    col = col;
                #elif _OPTIONS_BLUNT
                    col = col * float4(1,0,0,1);
                #elif _OPTIONS_SHARP
                    col = col * float4(0,0,1,1);
                #elif _OPTIONS_NEUTRAL
                    col = col * float4(0,1,1,1);
                #endif

                return col + _EmissionColor;

            }
            ENDCG
        }
    }
}
