#ifndef CLOTH_MOVEMENT_INCLUDED
#define CLOTH_MOVEMENT_INCLUDED

void windanim_float(half3 vertex_xyz, half2 color, half _WaveFreq, half _WaveHeight, half _WaveScale, out float3 wind_xyz){
	half phase_slow = _Time * _WaveFreq;
	half phase_med = _Time * 4 * _WaveFreq;
	           
	half offset = (vertex_xyz.x + (vertex_xyz.z * _WaveScale)) * _WaveScale;
	half offset2 = (vertex_xyz.x + (vertex_xyz.z * _WaveScale * 2)) * _WaveScale * 2;
	         
	half sin1 = sin(phase_slow + offset);
	half sin2 = sin(phase_med + offset2);          
	 
	half sin_combined = (sin1 * 4) + sin2 ;
	           
	half wind_x =  sin_combined * _WaveHeight * 0.1;
	wind_xyz = half3(wind_x, wind_x * 2, wind_x);

	wind_xyz = wind_xyz * pow(color.r, 2);
}

#endif
