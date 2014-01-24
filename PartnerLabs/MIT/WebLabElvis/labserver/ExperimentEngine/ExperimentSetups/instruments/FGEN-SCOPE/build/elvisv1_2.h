#include "extcode.h"
#pragma pack(push)
#pragma pack(1)

#ifdef __cplusplus
extern "C" {
#endif
typedef uint16_t  Enum;
#define Enum_Sine 0
#define Enum_Triangular 1
#define Enum_Square 2

/*!
 * runExperiment
 */
void __stdcall runExperiment(int32_t Slope, uint16_t TriggerType, 
	uint16_t TriggerSource, double ChBOffset, double ChAOffset, double ChBRange, 
	double ChARange, uint16_t ChB_Coupling, uint16_t ChACoupling, 
	uint16_t ChB_Source, uint16_t ChA_Source, uint16_t Acquire, 
	int32_t RecordLength, double SampleRateHz, Enum WaveformType, 
	double Frequency, double DCOffset, double Amplitude, char DeviceName[], 
	double interleavedArray[], int32_t len);

long __cdecl LVDLLStatus(char *errStr, int errStrLen, void *module);

#ifdef __cplusplus
} // extern "C"
#endif

#pragma pack(pop)

