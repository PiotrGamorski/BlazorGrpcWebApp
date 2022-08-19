varying vec3 vNormal;        

// #pragma glslify: perlin4d = require('../shaders/partials/perlin4d.glsl');

void main()
{
    vec3 newPosition = position;
    // float perlinStrength = perlin4d(vec4(position, 0.0));
    // newPosition += normal * perlinStrength;

    vec4 viewPosition = viewMatrix * vec4(position, 1.0);
    gl_Position = projectionMatrix * viewPosition;

    vNormal = normal;
}