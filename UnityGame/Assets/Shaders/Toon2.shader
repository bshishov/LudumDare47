Shader "Custom/Toon2"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _RampTex("Ramp", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Geometry" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                half lightCoeff : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _RampTex;
            float4 _MainTex_ST;
            fixed4 _Color;            
            fixed4 _LightColor0;            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.lightCoeff = dot(worldNormal, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5;                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                fixed4 rampValue = tex2D(_RampTex, i.lightCoeff).r;
                fixed4 col = tex2D(_MainTex, i.uv) * _Color * _LightColor0;
                col.rgb *= rampValue;
                return col;
            }
            ENDCG
        }
    }
    
    Fallback "Unlit/Texture"
}
