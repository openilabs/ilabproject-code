#ifndef _fundtypes_H
#define _fundtypes_H
/*
	NI CONFIDENTIAL
	© Copyright 1990-2004 by National Instruments Corp.
	All rights reserved.

	Owners: brian.powell, greg.richardson, paul.austin

	fundtypes.h - Fundamental data types.

	Header intended for use by C and C++. No // comments.
*/

#ifdef DefineHeaderRCSId
#define rcsid_fundtypes "$Id$"
#endif

#include "platdefines.h"

#if Mac || Palm/*##############################################################*/

	typedef char				int8;
	typedef unsigned char		uInt8;
	typedef uInt8				uChar;
	typedef short				int16;
	typedef unsigned short		uInt16;
	typedef long				int32;
	typedef unsigned long		uInt32;
	typedef float				float32;
	typedef double			float64;

	#if (ProcessorType == kM68000) || ((ProcessorType == kPPC) && (Compiler == kMPW))
		typedef long double floatExt;
		#define ExtHiLo 0
		#define CompilerSupportsExt	1
	#elif MacOSX && (ProcessorType == kPPC)
		typedef double floatExt;
		#define CompilerSupportsExt	1
	#elif MacOSX && (ProcessorType == kX86)
		typedef struct { /* force GCC to make this struct 10 bytes */
			int32	mlo __attribute__((packed,aligned(2)));
			int32	mhi __attribute__((packed,aligned(2)));
			int16	e __attribute__((packed,aligned(2)));
		} floatExt;
		#define CompilerSupportsExt	0
	#else
		typedef struct {
			double hi;
			double lo;
		} floatExt;
		#define ExtHiLo 1
		#define CompilerSupportsExt	1
	#endif	

	#define CompilerSupportsInt64 1
	typedef long long			int64;
	typedef unsigned long long	uInt64;

#elif Unix /*###########################################################*/

	typedef signed char			int8;
	typedef unsigned char		uInt8;
	typedef uInt8				uChar;
	typedef short int			int16;
	typedef unsigned short int	uInt16;
	typedef int					int32;
	typedef unsigned int		uInt32;
	typedef float				float32;
	typedef double				float64;
	#define ExtHiLo 0
	#if ProcessorType==kPARISC || ProcessorType==kMIPS || (Linux && PowerPC)
		typedef double			floatExt;
		#define CompilerSupportsExt	1
	#elif ProcessorType==kX86
		#if (Compiler==kGCC)
			typedef struct { /* force GCC to make this struct 10 bytes */
				int32	mlo __attribute__((packed,aligned(2)));
				int32	mhi __attribute__((packed,aligned(2)));
				int16	e __attribute__((packed,aligned(2)));
			} floatExt;
		#else
			typedef struct {
				int32	mlo, mhi;
				int16	e;
			} floatExt;
		#endif
		#define CompilerSupportsExt	0
	#else
		typedef long double		floatExt;
		#define CompilerSupportsExt	1
	#endif

	#if OpSystem==kPowerUnix || (OpSystem == kHPUX && defined(__cplusplus))
		#define CompilerSupportsInt64 0
		typedef struct {
			int32	hi, lo;
		} int64, uInt64;
	#else
		#define CompilerSupportsInt64 1
		typedef long long		int64;
		typedef unsigned long long	uInt64;
	#endif

#elif MSWin /*##########################################################*/

	typedef char				int8;
	typedef unsigned char		uInt8;
	typedef uInt8				uChar;
	typedef short int			int16;
	typedef unsigned short int	uInt16;
	typedef long				int32;
	typedef unsigned long		uInt32;
	typedef float				float32;
	typedef double				float64;

	#if defined(_NI_VC_) || defined(_NI_SC_) || defined(_NI_BC_)
		#define CompilerSupportsInt64 0
		typedef struct {
			int32	hi, lo;
		} int64, uInt64;
	#else
		#define CompilerSupportsInt64 1
		#if Compiler==kMetroWerks
		typedef long long				int64;
		typedef unsigned long long		uInt64;
		#else		
		typedef __int64				int64;
		typedef unsigned __int64	uInt64;
		#endif
	#endif

	#define ExtHiLo 0
	#if ProcessorType == kX86
		typedef struct {
			int32	mlo, mhi;
			int16	e;
		} floatExt;
		#define CompilerSupportsExt	0
	#else
		typedef double floatExt;
		#define CompilerSupportsExt	1
	#endif
	
#endif

/* Bool32 is based on our own int32 type. */
typedef int32		Bool32;
#undef	TRUE
#undef	FALSE
#define TRUE		1L
#define FALSE		0L

/* For making void* pts that are distinguishable. */
#define NIPrivatePtr(x)	typedef struct x ## _t { void *p; } *x

/* Limits for fundamental types. */
#define uInt8_MAX		0xff
#define uInt16_MAX		0xffff
#define uInt32_MAX		0xffffffff

#define int8_MIN		(-128)
#define int8_MAX		(127)
#define int16_MIN		(-32768)
#define int16_MAX		32767
#define int32_MIN		(-2147483647L - 1)
#define int32_MAX		2147483647L

#define float32_MAX     3.402823466e+38F       
#define float64_MAX     1.7976931348623158e+308 

#if CompilerSupportsInt64
	/* 64-bit integer constants */
	#if MSWin && (_MSC_VER < 1300)
		/* Before VC 7.1, we need to use this special way of adorning constants */
		#define I64Const(n)	n##i64
		#define U64Const(n)	n##ui64
	#else
		#define I64Const(n)	n##LL
		#define U64Const(n)	n##ULL
	#endif

	#define int64_MIN    ( I64Const(-9223372036854775807) - 1)
	#define int64_MAX    I64Const(9223372036854775807)
	#define uInt64_MAX   U64Const(0xffffffffffffffff)
#endif

#if PointerSize == k32bitPointer
	typedef int32 PtrSizedInt;
	typedef uInt32 PtrSizedUInt;
#elif PointerSize == k64bitPointer
	#if CompilerSupportsInt64
		typedef int64 PtrSizedInt;
		typedef uInt64 PtrSizedUInt;
	#else
		#error "Identity crisis - how can a 64-bit system not support 64-bit ints?"
	#endif
#else
	#error "We don't know the size of pointers on this platform"
#endif

#ifndef _NI_HRESULT_DEFINED_
#define _NI_HRESULT_DEFINED_
	typedef long   HRESULT;
	typedef int32 SCODE;
#endif

#ifndef Unused
/* The macro Unused can be used to avoid compiler warnings for
	unused parameters or locals. */
#ifdef __cplusplus
	/* This implementation of Unused is safe for const parameters. */
	#define Unused(var_or_param)	_Unused((const void *)&var_or_param)
	inline void _Unused(const void *) {}
#elif MSWin
	/* This implementation of Unused is not safe for const parameters. */
	#define Unused(var_or_param)	var_or_param=var_or_param
#else
	#define Unused(var_or_param)
#endif
#endif // Unused

#ifndef ConstNotEq
/* The ConstNotEq macro can be used to avoid 
"expression is constant" warnings in "if" 
statements -- only avoids the warning if you
compile with a C++ compiler. */
#ifdef __cplusplus
	inline bool _ConstNotEq(const int32 a, const int32 b) { return a != b; }
	#define ConstNotEq(a, b) _ConstNotEq(a, b)
#else
	#define ConstNotEq(a, b) (a != b)
#endif // __cplusplus
#endif // ConstNotEq

#ifndef ConstEvalBool
/* The ConstEvalBool macro can be used to avoid 
"expression is constant" warnings in "if" 
statements -- only avoids the warning if you
compile with a C++ compiler. */
#ifdef __cplusplus
	inline bool _ConstEvalBool(const bool a) { return a; }
	#define ConstEvalBool(a) _ConstEvalBool(a)
#else
	#define ConstEvalBool(a) (a)
#endif // __cplusplus
#endif // ConstEvalBool

/* Get the system definitions for 
	size_t, NULL, ptrdiff_t, offsetof, wchar_t */

#include <stddef.h>

#endif /* _fundtypes_H */
