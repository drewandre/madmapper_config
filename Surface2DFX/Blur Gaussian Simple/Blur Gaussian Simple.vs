/*
	Available uniforms

		// mm_Vertex.xy comes in (-1,-1)-(1,1), mm_ModelViewProjectionMatrix is a combination of the surface
		// perspective transform and the view transform
		uniform mat4 mm_ModelViewProjectionMatrix;

		// mm_TexCoord0.xy is the texture coordinate relative to the input rectangle bounding rectangle (0,0)-(1,1),
		// this matrix transforms from coordinates in input rectangle bounding rectangle to coordinates in the media
		// (it handles scaling and rotation of the uv rectangle)
		uniform mat4 mm_TextureMatrix;

		// mm_ModulationColor contains red, green, blue levels & surface opacity in alpha
		// The multiplication in the fragment shader is done by MadMapper, so you shouldn't need that
		uniform vec4 mm_ModulationColor;

		// Blend Mode, see possible values in Definitions below. This is handled by default by MadMapper so you should need it
		uniform int mm_BlendMode;

		// Used to know if we're rendering to the preview or to an output fullscreen / desktop window / syphon-spout-NDI
		uniform bool mm_IsRenderingPreview;

	Vertex Shader calls fxVsFunc() after having initialized its outputs:

	    // Position of this vertex in the surface output geometry (mm_TexCoord0.xy might be affected by mesh warping input points)
	    // This can be useful in the fragment shader
		mm_SurfaceCoord = mm_TexCoord0.zw;
		
		// Alpha of this point generated from surface / mask feathering
	    mm_Alpha = mm_Vertex.w;

	Definitions & Macros:

		#define BlendModeIgnoreAlpha 0
		#define BlendModeAdd 1
		#define BlendModeOver 2
		#define BlendModeOverPremultiplied 3
		#define BlendModeMultiply 4
		#define BlendModeSubtract 5
		#define BlendModeStencilLuma 6
		#define BlendModeSilhouetteLuma 7
		#define BlendModeSilhouetteAlpha 8

		vec4 FX_NORM_PIXEL(vec2 uv) => returns the color of the pixel at normalized position uv (0,0-1,1), whether it's a texture or a material

		vec2 FX_IMG_SIZE() => returns the dimensions of the input media or 1024x1024 if a Material  is being used

		#define IS_MATERIAL => defined only if a Material is being used as input media (the FX & the Material shaders are combined into a single Shader Program)

*/

void fxVsFunc()
{
	// Initialize Surface 2D fragment shader inputs
	mm_FragNormCoord = (mm_TextureMatrix*vec4(mm_TexCoord0.xy,0,1)).xy;

	// Tells OpenGL where this vertex should be on the output view
	gl_Position = mm_ModelViewProjectionMatrix * vec4(mm_Vertex.xy,0,1);
}
