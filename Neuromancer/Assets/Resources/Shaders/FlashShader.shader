Shader "Custom/FlashShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlendAmount ("Blend Amount", Range(0, 1)) = 0.6
        _EmissionAmount ("Emission Amount", Range(0, 1)) = 0.5
        _ColorChange ("Color Change", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half _BlendAmount;
            half _EmissionAmount;
            fixed4 _ColorChange;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the original texture
                fixed4 originalColor = tex2D(_MainTex, i.uv);

                // Calculate the blended color manually
                fixed4 blendedColor = originalColor * (1 - _BlendAmount) + _ColorChange * _BlendAmount;

                // Add emissive glow
                fixed4 emissiveGlow = _ColorChange * _EmissionAmount;

                // Combine the blended color with the emissive glow
                fixed4 finalColor = blendedColor + emissiveGlow;

                return finalColor;
            }
            ENDCG
        }
    }
}
