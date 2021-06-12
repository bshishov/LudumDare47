Shader "Custom/ToonSimple" {
	Properties
	{
		_Color("Main Color", Color) = (.5,.5,.5,1)		
		_MainTex("Base (RGB)", 2D) = "white" { }
		
	    [Header(Lighting Parameters)]
	    _Ramp("Color Ramp", 2D) = "white" {}
		[HideInInspector] _ShadowTint ("Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
	    [HideInInspector] _AlphaTest("Alpha Test", Range(0.0, 1.0)) = 0.5
	}

	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Geometry" }	    
        
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
		float3 _ShadowTint;

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
			o.Albedo = col.rgb;
			o.Alpha = col.a;
		}
		ENDCG
	}

	Fallback "Standard"
}
