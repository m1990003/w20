#version 330
precision highp float;
uniform sampler2D sampler;
uniform mat4 model_matrix;
uniform vec3 light_direction;
uniform vec4 light_ambient_color;
uniform vec4 light_diffuse_color;
in vec2 fragTexcoord;
in vec3 fragNormal;
out vec4 outputColor;
void main(){
	mat3 normalMatrix = mat3(model_matrix);
	vec3 normal = normalize(normalMatrix * fragNormal);
	float brightness = clamp(dot(normalize(normal), light_direction), 0, 1);
	vec4 surfaceColor = texture2D(sampler, fragTexcoord);
	outputColor = surfaceColor * (light_ambient_color + brightness * light_diffuse_color);}
