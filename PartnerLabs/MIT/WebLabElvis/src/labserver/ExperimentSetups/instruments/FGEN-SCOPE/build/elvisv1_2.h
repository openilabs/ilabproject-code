#include "extcode.h"
#pragma pack(push)
#pragma pack(1)

#ifdef __cplusplus
extern "C" {
#endif

void __stdcall runExperiment(long SCOPET_Slope, 
	unsigned short SCOPET_TriggerType, unsigned short SCOPE_TriggerSource, 
	unsigned short SCOPEChA_Source, double SCOPEChA_Range, 
	double SCOPEChA_Offset, unsigned short SCOPEChA_Coupling, 
	unsigned short SCOPEChB_Source, double SCOPEChB_Range, 
	double SCOPEChB_Offset, unsigned short SCOPEChB_Coupling, 
	long SCOPEH_RecordLength, double SCOPEH_SampleRateHz, 
	unsigned short SCOPEH_Acquire, unsigned short FGEN_WaveformType, 
	double FGEN_Frequency, double FGEN_DCOffset, double FGEN_Amplitude, 
	double interleavedArray[], long len);

long __cdecl LVDLLStatus(char *errStr, int errStrLen, void *module);

#ifdef __cplusplus
} // extern "C"
#endif

#pragma pack(pop)

