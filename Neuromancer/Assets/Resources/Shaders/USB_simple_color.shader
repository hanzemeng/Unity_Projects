// Primary color model - has very little optimization in its code
Shader "USB / USB_simple_color"
{
    Properties
    {
        [Header(Texture properties)]
        [Space(10)]
        _MainTex ("Texture", 2D) = "white" {}
        // // declare the drawer Toggle
        // [KeywordEnum(Off, Red, Blue)]
        // _Options("Color Options", Float) = 0
        // declares the drawer
        // [PowerSlider(3.0)]
        // _Brightness ("Brightness", Range(0.01, 1)) = 0.08
        // [Space(10)]
        // [IntRange]_Samples ("Sample", Range(0,255)) = 100
        // [Enum(0ff, 0, Front, 1, Back, 2)]
        // _Face ("Face Culling", Float) = 0

        [Space(20)]
        [Header(Specular properties)]
        [Space(10)]
        _Specularity("Specularity", Range(0.01, 1)) = 0.08
        _Brightness("Brightness", Range(0.01, 1)) = 0.08
        _SpecularColor("Specular Color", Color) = (1,1,1,1)

    }
    SubShader
    {
        // "Queue" = defines how the surface of the object will look
        // DEFAULT: all volume is defined as Geometry, DOES NOT SUPPORT TRANSPARENCY
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
        LOD 100

        Cull[_Face]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile _OPTION_OFF _OPTION_RED _OPTION_BLUE

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;
            int _Samples;

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
                return col;
            }

            // half4 frag(v2f i): SV_Target
            // {
            //     half4 col = tex2D(_MainTex, i.uv);

            //     // generate conditions
            // #if _OPTION_OFF
            //     return col;
            // #elif _OPTION_RED
            //     return col * float4(1,0,0,1);
            // #elif _OPTION_BLUE
            //     return col * float4(0,0,1,1);
            // #endif
            // }
            ENDCG
        }
    }
}
