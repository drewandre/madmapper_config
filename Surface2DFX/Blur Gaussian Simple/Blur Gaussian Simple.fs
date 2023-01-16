// original by existical https://www.shadertoy.com/view/Xltfzj
// adapted to madmapper by frz / 1024

/*{
    "CREDIT": "existical",
    "TAGS": ["graphics"],
    "VSN": 1.0,
    "DESCRIPTION": "1 pass Gaussian Blur",
    "MEDIA": {
        "REQUIRES_TEXTURE": true,
        "GL_TEXTURE_MIN_FILTER": "LINEAR_MIPMAP_LINEAR"
    },
    "INPUTS": [
        { "LABEL": "Samples", "NAME": "fx_samples", "TYPE": "float", "DEFAULT": 16., "MIN": 4., "MAX": 32. },
        { "LABEL": "HQ", "NAME": "fx_quality", "TYPE": "bool", "DEFAULT": true , "FLAGS":"button"},
        { "LABEL": "Size", "NAME": "fx_size", "TYPE": "float", "DEFAULT": 16., "MIN": 0.0, "MAX": 64. },
    ]
}*/

vec4 fxColorForPixel(vec2 mm_FragNormCoord)
{
	vec2 uv = mm_FragNormCoord;

   float Pi = 6.28318530718; // Pi*2
    
    // GAUSSIAN BLUR SETTINGS {{{
    float Directions = fx_samples; // BLUR DIRECTIONS (Default 16.0 - More is better but slower)
    float Quality = 2. + float(fx_quality)*2.; // BLUR QUALITY (Default 4.0 - More is better but slower)
    float Size = fx_size; // BLUR SIZE (Radius)
    // GAUSSIAN BLUR SETTINGS }}}
   
	vec2 res = FX_IMG_SIZE();
    vec2 Radius = Size/res.xy;
    
    // Pixel colour
    vec4 Color = FX_NORM_PIXEL_LOD(uv,fx_size/16);
    int count = 1;

    // Blur calculations
    if (fx_quality) {
        for( float d=0.0; d<Pi; d+=Pi/Directions)
        {
            Color += FX_NORM_PIXEL_LOD( uv+vec2(cos(d),sin(d))*Radius*1/4, fx_size/16);
            Color += FX_NORM_PIXEL_LOD( uv+vec2(cos(d),sin(d))*Radius*2/4, fx_size/12);
            Color += FX_NORM_PIXEL_LOD( uv+vec2(cos(d),sin(d))*Radius*3/4, fx_size/8);
            Color += FX_NORM_PIXEL_LOD( uv+vec2(cos(d),sin(d))*Radius*4/4, fx_size/4);

            count += 4;
        }
    } else {
        for( float d=0.0; d<Pi; d+=Pi/Directions)
        {
            Color += FX_NORM_PIXEL_LOD( uv+vec2(cos(d),sin(d))*Radius*1/2, fx_size/12);
            Color += FX_NORM_PIXEL_LOD( uv+vec2(cos(d),sin(d))*Radius*2/2, fx_size/8);

            count += 2;
        }
    }

    // Output to screen
    Color /= count;

	return Color;
}
