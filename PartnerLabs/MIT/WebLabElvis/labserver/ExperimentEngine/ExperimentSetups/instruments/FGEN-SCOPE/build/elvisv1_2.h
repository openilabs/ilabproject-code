#include "extcode.h"
#pragma pack(push)
#pragma pack(1)

#ifdef __cplusplus
extern "C" {
#endif

void __stdcall runExperiment(char DeviceName[], int32_t SCOPET_Slope, 
	uint16_t SCOPET_TriggerType, uint16_t SCOPET_TriggerSource, 
	uint16_t SCOPEChA_Source, double SCOPEChA_Range, double SCOPEChA_Offset, 
	uint16_t SCOPEChA_Coupling, uint16_t SCOPEChB_Source, double SCOPEChB_Range, 
	double SCOPEChB_Offset, uint16_t SCOPEChB_Coupling, 
	int32_t SCOPEH_RecordLength, double SCOPEH_SampleRateHz, 
	uint16_t SCOPEH_Acquire, uint16_t FGEN_WaveformType, double FGEN_Frequency, 
	double FGEN_DCOffset, double FGEN_Amplitude, double interleavedArray[], 
	int32_t len);

long __cdecl LVDLLStatus(char *errStr, int errStrLen, void *module);

#ifdef __cplusplus
} // extern "C"
#endif

#pragma pack(pop)

