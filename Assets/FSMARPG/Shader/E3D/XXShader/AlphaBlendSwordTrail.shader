Shader "XX/GlowingSwordTrail" {
    Properties {
        _MainTex ("主纹理", 2D) = "white" {}
        [HDR] _Color ("发光颜色", Color) = (2,1,0.5,1)
        _Intensity ("基础强度", Range(0, 5)) = 1
        _Emission ("自发光强度", Range(0, 10)) = 3
        _Speed ("纹理流动速度", Float) = 1
        _AlphaScale ("透明度缩放", Range(0, 1)) = 0.5
        _FresnelPower ("边缘发光强度", Range(0, 10)) = 2
        _FresnelScale ("边缘发光范围", Range(0, 5)) = 1
        
        [Space(20)]
        [Toggle(USE_GRADIENT)] _UseGradient ("使用颜色渐变", Float) = 0
        _GradientTex ("渐变纹理", 2D) = "white" {}
        
        [Space(20)]
        [Toggle(USE_DISTORTION)] _UseDistortion ("使用扭曲效果", Float) = 0
        _DistortionTex ("扭曲纹理", 2D) = "bump" {}
        _DistortionPower ("扭曲强度", Range(0, 0.2)) = 0.1
        _DistortionSpeed ("扭曲速度", Float) = 1
    }
    
    SubShader {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent+100"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off
        Fog { Mode Off }
        
        Pass {
            Name "MAIN"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature USE_GRADIENT
            #pragma shader_feature USE_DISTORTION
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                fixed4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float3 viewDir : TEXCOORD1;
                float3 normal : TEXCOORD2;
                #if USE_DISTORTION
                float3 tangent : TEXCOORD3;
                float3 bitangent : TEXCOORD4;
                #endif
                #if USE_GRADIENT
                float2 gradUV : TEXCOORD5;
                #endif
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Intensity;
            float _Emission;
            float _Speed;
            float _AlphaScale;
            float _FresnelPower;
            float _FresnelScale;
            
            #if USE_GRADIENT
            sampler2D _GradientTex;
            #endif
            
            #if USE_DISTORTION
            sampler2D _DistortionTex;
            float _DistortionPower;
            float _DistortionSpeed;
            #endif

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // UV动画
                o.uv.x += _Time.y * _Speed;
                
                #if USE_GRADIENT
                o.gradUV = float2(v.uv.x, 0);
                #endif
                
                // 视图方向和法线用于菲涅尔效果
                o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.normal = v.normal;
                
                #if USE_DISTORTION
                // 切线空间计算
                o.tangent = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0))).xyz;
                o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;
                #endif
                
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                #if USE_DISTORTION
                // 计算切线空间矩阵
                float3x3 tbn = float3x3(i.tangent, i.bitangent, i.normal);
                float3 viewDirTS = mul(tbn, i.viewDir);
                
                // 采样扭曲纹理
                float2 distortion = tex2D(_DistortionTex, i.uv + _Time.y * _DistortionSpeed * 0.1).rg;
                distortion = (distortion * 2 - 1) * _DistortionPower;
                
                // 应用扭曲到UV
                float2 distortedUV = i.uv + distortion;
                #else
                float2 distortedUV = i.uv;
                #endif
                
                // 采样主纹理
                fixed4 col = tex2D(_MainTex, distortedUV);
                
                // 应用渐变纹理
                #if USE_GRADIENT
                fixed4 gradient = tex2D(_GradientTex, i.gradUV);
                col.rgb *= gradient.rgb;
                col.a *= gradient.a;
                #endif
                
                // 菲涅尔效果 - 边缘发光
                float fresnel = dot(normalize(i.viewDir), normalize(i.normal));
                fresnel = pow(1.0 - saturate(fresnel), _FresnelPower) * _FresnelScale;
                
                // 结合顶点颜色和基础强度
                col *= i.color;
                col.rgb *= _Intensity;
                
                // 添加自发光和菲涅尔效果
                float3 emission = col.rgb * _Emission * (1 + fresnel);
                col.rgb += emission;
                
                // 透明度控制
                col.a *= _AlphaScale;
                
                // 根据UV的y值实现边缘淡化
                float edgeFade = 1 - abs(i.uv.y * 2 - 1);
                col.a *= edgeFade * edgeFade; // 二次方使边缘更柔和
                
                // 确保颜色不会过暗
                col.rgb = max(col.rgb, fixed3(0.1, 0.1, 0.1));
                
                return col;
            }
            ENDCG
        }
        
        // 第二个Pass用于增强发光效果（可选）
        Pass {
            Name "GLOW"
            Blend SrcAlpha One
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float3 viewDir : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Intensity;
            float _Emission;
            float _Speed;
            float _AlphaScale;
            float _FresnelPower;
            float _FresnelScale;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv.x += _Time.y * _Speed;
                
                o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
                o.normal = v.normal;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= i.color;
                
                // 菲涅尔效果
                float fresnel = dot(normalize(i.viewDir), normalize(i.normal));
                fresnel = pow(1.0 - saturate(fresnel), _FresnelPower) * _FresnelScale;
                
                // 发光Pass使用更高的强度
                col.rgb *= _Intensity * _Emission * 0.5 * (1 + fresnel);
                col.a *= _AlphaScale * 0.3; // 降低透明度
                
                // 边缘淡化
                float edgeFade = 1 - abs(i.uv.y * 2 - 1);
                col.a *= edgeFade;
                
                return col;
            }
            ENDCG
        }
    }
    
    FallBack "Mobile/Particles/Alpha Blended"
    CustomEditor "GlowingSwordTrailShaderEditor"
}