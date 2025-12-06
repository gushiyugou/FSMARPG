Shader "Standard" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" { }
        _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _GlossMapScale ("Smoothness Scale", Range(0, 1)) = 1
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
        _MetallicGlossMap ("Metallic", 2D) = "white" { }
        [ToggleOff] _SpecularHighlights ("Specular Highlights", Float) = 1
        [ToggleOff] _GlossyReflections ("Glossy Reflections", Float) = 1
        _BumpScale ("Scale", Float) = 1
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" { }
        _Parallax ("Height Scale", Range(0.005, 0.08)) = 0.02
        _ParallaxMap ("Height Map", 2D) = "black" { }
        _OcclusionStrength ("Strength", Range(0, 1)) = 1
        _OcclusionMap ("Occlusion", 2D) = "white" { }
        _EmissionColor ("Color", Color) = (0,0,0,1)
        _EmissionMap ("Emission", 2D) = "white" { }
        _DetailMask ("Detail Mask", 2D) = "white" { }
        _DetailAlbedoMap ("Detail Albedo x2", 2D) = "grey" { }
        _DetailNormalMapScale ("Scale", Float) = 1
        [Normal] _DetailNormalMap ("Normal Map", 2D) = "bump" { }
        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0
        [HideInInspector] _Mode ("__mode", Float) = 0
        [HideInInspector] _SrcBlend ("__src", Float) = 1
        [HideInInspector] _DstBlend ("__dst", Float) = 0
        [HideInInspector] _ZWrite ("__zw", Float) = 1
    }
    
    SubShader {
        LOD 300
        Tags { "PerformanceChecks" = "False" "RenderType" = "Opaque" }
        
        // 添加Pass块
        Pass {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                fixed4 albedo = tex2D(_MainTex, i.uv) * _Color;
                fixed3 lightColor = LIGHT_ATTENUATION(i);
                fixed3 normal = normalize(i.worldNormal);
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed ndotl = max(0, dot(normal, lightDir));
                fixed4 col = albedo * ndotl * fixed4(lightColor, 1);
                return col;
            }
            ENDCG
        }
    }
    
    SubShader {
        LOD 150
        Tags { "PerformanceChecks" = "False" "RenderType" = "Opaque" }
        
        // 简化版本的Pass用于低LOD
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }
    }
    
    Fallback "VertexLit"
    CustomEditor "StandardShaderGUI"
}