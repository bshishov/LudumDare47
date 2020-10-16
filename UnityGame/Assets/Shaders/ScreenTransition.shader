Shader "UI/ScreenTransition" 
{
    Properties {        
		[PerRendererData] _MainTex ("Texture", 2D) = "white" {}		
		_DivisorTex ("Divisor", 2D) = "white" {}		

		//_ColorBase ("Color Base", Color) = (0,0,0,1)
		//_ColorDmg ("Color Dmg", Color) = (1,1,1,1)
		//_ColorHp ("Color Hp", Color) = (1, 0, 0, 1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    SubShader {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

        Pass {
			Name "Default"
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

			#include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

			sampler2D _MainTex;
			sampler2D _DivisorTex;
            //fixed4 _ColorBase;
			//fixed4 _ColorDmg;
			//fixed4 _ColorHp;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
			uniform float4 _MainTex_TexelSize;

            float4 unity_ScreenParams;

            v2f vert (appdata_t  v) 
			{
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);				

				#if UNITY_COLORSPACE_GAMMA
				OUT.color = v.color;				
				#else
				OUT.color = float4(LinearToGammaSpace(v.color.rgb), v.color.a);
				#endif
                
                return OUT;
            }

            fixed4 frag (v2f i) : SV_Target 
			{                			    
                float ar = 16.0 / 9.0; //unity_ScreenParams.x / unity_ScreenParams.y; 
			    
                float threshold = i.color.z * 2;
                float2 center = i.color.xy;
			    half2 diff = i.texcoord - center;
			    diff.x *= ar;
                float distanceToCenter = length(diff);
                float k = max(0, sign(distanceToCenter - threshold));
                float4 color = float4(0, 0, 0, k * i.color.a);

			    #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

				return color;
            }
            ENDCG
        }
    }
}