#include "extcode.h"
#pragma pack(push)
#pragma pack(1)

#ifdef __cplusplus
extern "C" {
#endif
typedef struct {
	LVBoolean status;
	long code;
	LStrHandle source;
	} TD1;


void __stdcall OpAmpInverter(double FrequencyHz, double PeakAmplitudeV, 
	double DCOffsetV, unsigned short WaveformType, double SamplingRate, 
	double samplingTime, double OutputWaveform[], long len, TD1 *errorOut);

long __cdecl LVDLLStatus(char *errStr, int errStrLen, void *module);

#ifdef __cplusplus
} // extern "C"
#endif

#pragma pack(pop)

