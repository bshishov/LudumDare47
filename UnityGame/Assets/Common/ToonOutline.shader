Shader "Custom/ToonOutline" {
	Properties
	{
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(0.0, 10)) = 3
		_MainTex("Base (RGB)", 2D) = "white" { }
	    _TintColor("Tint Color", Color) = (1, 1, 1, 0)
		
	    [Header(Lighting Parameters)]
	    _Ramp("Color Ramp", 2D) = "white" {}
		[HideInInspector] _ShadowTint ("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
	    [HideInInspector] _AlphaTest("Alpha Test", Range(0.0, 1.0)) = 0.5
	}

	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Geometry" "IgnoreProjector" = "True" }	    
        
	    /*
		Pass {
			Stencil
			{
				Ref 64
				Comp always
				Pass replace
			}
			ColorMask A
		}*/

	    Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM

		#pragma surface surf Ramp alphatest:_AlphaTest keepalpha nofog addshadow 
		#pragma target 3.0
		
		#include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "AutoLight.cginc"

		sampler2D _Ramp;
		sampler2D _MainTex;
		fixed4 _Color;
		fixed3 _ShadowTint;
		fixed4 _TintColor;

		half4 LightingRamp(SurfaceOutput s, half3 lightDir, half shadowAttenuation) {
		    // https://github.com/ronja-tutorials/ShaderTutorials/blob/master/Assets/031_StepToon/SteppedToonLighting.shader
		    
            //how much does the normal point towards the light?
            float towardsLight = dot(s.Normal, lightDir);
            // make the lighting a hard cut
            float towardsLightChange = fwidth(towardsLight);
            float lightIntensity = smoothstep(0, towardsLightChange, towardsLight);		    

            #ifdef USING_DIRECTIONAL_LIGHT
                //for directional lights, get a hard vut in the middle of the shadow attenuation
                float attenuationChange = fwidth(shadowAttenuation) * 0.5;
                float shadow = smoothstep(0.5 - attenuationChange, 0.5 + attenuationChange, shadowAttenuation);
            #else
                //for other light types (point, spot), put the cutoff near black, so the falloff doesn't affect the range
                float attenuationChange = fwidth(shadowAttenuation);
                float shadow = smoothstep(0, attenuationChange, shadowAttenuation);
            #endif
            lightIntensity = lightIntensity * shadow;
		    float3 shadowColor = s.Albedo * _ShadowTint;
		    
			half NdotL = dot(s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half3 ramp = tex2D(_Ramp, float2(diff, diff)).rgb;
			half4 c;
			c.rgb = lerp(shadowColor, s.Albedo * _LightColor0.rgb * ramp, lightIntensity) * _LightColor0.rgb;		    
			c.a = s.Alpha;
			return c;
		}

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {		    
			fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = lerp(col.rgb, _TintColor.rgb, _TintColor.a);
			o.Alpha = col.a;
		}
		ENDCG
        
	    
		Pass
		{
			Name "OUTLINE"
			Tags{ "LightMode" = "Always" }

			//Stencil {
			//	Ref 64
			//	Comp NotEqual
			//	ZFail Zero
			//}

			//Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			//Cull Back
			ZWrite On
		    ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 pos : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform float _Outline;
			uniform float4 _OutlineColor;
			fixed4 _TintColor;

			v2f vert(appdata v)
			{
				v2f o;

			    // https://www.videopoetics.com/tutorials/pixel-perfect-outline-shaders-unity/
			       
                float4 clipPosition = UnityObjectToClipPos(v.vertex);
                float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, v.normal));			    
			    float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _Outline * clipPosition.w * 2;

			    // Global scaling
			    offset *= 0.5;
			    
                clipPosition.xy += offset;
			    o.pos = clipPosition;
			    
			    //float3 normal = normalize(v.normal);
			    //float3 outlineOffset = normal * _Outline;
			    //float3 position = v.vertex + outlineOffset;			    
			    //o.pos = UnityObjectToClipPos(position);
			    
				// float4 p = UnityObjectToClipPos(v.vertex);
				// float4 pnorm = UnityObjectToClipPos(v.vertex + v.normal);
				// float2 offset = (pnorm - p).xy;			    
				// o.pos.xy = p.xy + offset * _Outline;			    
				// o.pos.zw = p.zw;

				
				o.color = fixed4(lerp(_OutlineColor.rgb, _TintColor.rgb, _TintColor.a), _OutlineColor.a);
				o.uv = v.uv;
				return o;
			}

			half4 frag(v2f i) :COLOR
			{
				fixed4 tex = tex2D(_MainTex, i.uv);
				return i.color * tex.a;
			}
			ENDCG
		}
	    
	}

	Fallback "Standard"
}
