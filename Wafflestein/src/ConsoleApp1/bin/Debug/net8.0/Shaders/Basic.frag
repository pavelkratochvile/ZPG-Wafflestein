#version 330 core

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
};
uniform Material material;

uniform vec3 cameraPosWorld;
uniform vec3 lightPosWorld;
uniform vec3 lightDirWorld;

uniform vec3 lightColor;
uniform float lightIntensity;

uniform int lightOn;
uniform float inerCutOff;
uniform float outerCutOff;

uniform bool hasTeleported;
uniform float beforeTeleportTime;
uniform float afterTeleportTime;

in vec3 normalWorld;
in vec3 fragmentWorld;

out vec4 outColor;

void main() {
    vec3 norm = normalize(normalWorld);
    vec3 ambient = material.diffuse * lightColor * 0.1;
    vec3 lightDir = normalize(lightPosWorld - fragmentWorld);
   
    float theta = dot(lightDir, normalize(lightDirWorld));
    float epsilon = inerCutOff - outerCutOff;
    float intensity = clamp((theta - outerCutOff) / epsilon, 0.0, 1.0);
   
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = material.diffuse * lightColor * diff * 0.3;
   
    vec3 viewDir = normalize(cameraPosWorld - fragmentWorld);
    vec3 reflectDir = reflect(-lightDir, norm);
    vec3 specular = material.specular * pow(max(dot(reflectDir, viewDir), 0.0), material.shininess) * lightColor * lightIntensity * 0.1;
   
    vec3 finalColor = diffuse * intensity * 2  + ambient * 3 + specular * intensity;
    
    if(!hasTeleported)
    {
        float part = beforeTeleportTime / 2000.0;
        finalColor += vec3(1.0, 1.0, 1.0) * part;

    }
    else if(hasTeleported)
    {
        float part = afterTeleportTime / 2000.0;
        finalColor += vec3(1.0, 1.0, 1.0) * (1-part);
    }
    outColor = vec4(finalColor, 1.0);
}
