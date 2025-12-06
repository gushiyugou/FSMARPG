Shader "Custom/SimpleVertexLit" {
    Properties {
        _VColor ("Vertex Color", Color) = (1,1,1,1)
        _VSpec ("Vertex Specular", Color) = (1,1,1,1)
        _VEmit ("Vertex Emission", Color) = (0,0,0,0)
        [PowerSlider(5.0)] _VShine ("Vertex Shininess", Range(0.01, 1)) = 0.7
        _VTex ("Vertex Texture", 2D) = "white" { }
    }
    
    SubShader {
        LOD 100
        Tags { 
            "RenderType" = "Opaque" 
            "Queue" = "Geometry+3"
            "LightMode" = "ForwardBase"
        }
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            // 使用完全不同的变量名
            sampler2D _VTex;
            float4 _VTex_ST;
            float4 _VColor;
            float4 _VSpec;
            float4 _VEmit;
            float _VShine;
            
            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _VTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                fixed4 texColor = tex2D(_VTex, i.uv);
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(i.viewDir);
                
                // 漫反射
                float ndotl = max(0, dot(worldNormal, lightDir));
                float3 diffuse = ndotl * _LightColor0.rgb;
                
                // 镜面反射
                float3 halfDir = normalize(lightDir + viewDir);
                float spec = pow(max(0, dot(worldNormal, halfDir)), _VShine * 128);
                float3 specular = spec * _VSpec.rgb * _LightColor0.rgb;
                
                // 阴影衰减
                float attenuation = LIGHT_ATTENUATION(i);
                
                fixed4 finalColor = texColor * _VColor;
                finalColor.rgb *= (diffuse * attenuation + UNITY_LIGHTMODEL_AMBIENT.rgb);
                finalColor.rgb += specular * attenuation;
                finalColor.rgb += _VEmit.rgb;
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    Fallback "Diffuse"
}