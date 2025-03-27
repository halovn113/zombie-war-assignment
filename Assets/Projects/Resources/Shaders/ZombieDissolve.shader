Shader "Custom/ZombieDissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _CutoffHeight ("Dissolve Height", Range(-1, 2)) = 0.0
        _EdgeColor ("Edge Color", Color) = (1,0.5,0,1)
        _EdgeWidth ("Edge Width", Range(0, 0.2)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            float _CutoffHeight;
            float _EdgeWidth;
            fixed4 _EdgeColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float height = i.worldPos.y;

                float dissolve = smoothstep(_CutoffHeight - _EdgeWidth, _CutoffHeight, height);

                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // Edge glow effect
                float edge = smoothstep(_CutoffHeight - _EdgeWidth, _CutoffHeight, height) * 
                             (1 - smoothstep(_CutoffHeight, _CutoffHeight + _EdgeWidth, height));

                col.rgb = lerp(_EdgeColor.rgb, col.rgb, 1 - edge);

                // Alpha clip
                clip(dissolve - 0.5); // Dissolve = 0 → clip

                return col;
            }
            ENDCG
        }
    }
}
