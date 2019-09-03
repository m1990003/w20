#version 330
precision highp float;

// "model_matrix" Matrix
uniform mat4 model_matrix;

// input from VAO-Datenstruktur
in vec3 in_position;
in vec3 in_normal; 
in vec2 in_uv; 

// "modelview_projection_matrix" as uniform parameter
uniform mat4 modelview_projection_matrix;

// "texcoord" is passed to the fragment shader
out vec2 fragTexcoord;

// the normal is also passed to the fragment shader
out vec3 fragNormal;

// ... and also the position of the vertex
out vec4 fragPosition;

void main()
{
	// "in_uv" (Texturkoordinate) wird direkt an den Fragment-Shader weitergereicht.
	fragTexcoord = in_uv;

	// die Normale wird an den Fragment-Shader gegeben.
	fragNormal = in_normal;

	// position is calculated by multiplying the 3d-data position with the model matrix, result is given to the fragment-shader
	fragPosition =  model_matrix * vec4(in_position,1);

	// in gl_Position die finalan Vertex-Position geschrieben ("modelview_projection_matrix" * "in_position")
	gl_Position = modelview_projection_matrix * vec4(in_position, 1);
}




