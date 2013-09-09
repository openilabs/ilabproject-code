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


void __stdcall Initialize(TD1 *errorOut, LVRefNum *FGENRefnumOut);
void __stdcall RunExperiment(double FrequencyHz, double PeakAmplitudeV, 
	double DCOffsetV, unsigned short WaveformType, TD1 *errorIn, TD1 *errorOut, 
	LVRefNum *FGENRefnumIn, LVRefNum *FGENRefnumOut);
void __stdcall FGenClose(TD1 *errorIn, TD1 *errorOut, LVRefNum *FGENRefnumIn, 
	LVRefNum *NIELVISRefnumOut);

long __cdecl LVDLLStatus(char *errStr, int errStrLen, void *module);

#ifdef __cplusplus
} // extern "C"
#endif

#pragma pack(pop)

