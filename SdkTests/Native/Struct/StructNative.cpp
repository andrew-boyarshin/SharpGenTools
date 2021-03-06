#include "StructNative.h"
#include <cstdio>

struct Implementation : public Interface
{
	virtual int One() override
	{
		return 1;
	}
};

STRUCTLIB_FUNC(SimpleStruct) GetSimpleStruct()
{
	return { 10, 3 };
}


STRUCTLIB_FUNC(StructWithArray) PassThroughArray(StructWithArray param)
{
	return param;
}


STRUCTLIB_FUNC(TestUnion) PassThroughUnion(TestUnion param)
{
	return param;
}

STRUCTLIB_FUNC(UnionWithArray) PassThroughUnion2(UnionWithArray param)
{
	return param;
}

STRUCTLIB_FUNC(BitField) PassThroughBitfield(BitField param)
{
	return param;
}

STRUCTLIB_FUNC(AsciiTest) PassThroughAscii(AsciiTest param)
{
	return param;
}

STRUCTLIB_FUNC(Utf16Test) PassThroughUtf(Utf16Test param)
{
	return param;
}

STRUCTLIB_FUNC(NestedTest) PassThroughNested(NestedTest param)
{
	return param;
}

STRUCTLIB_FUNC(BoolToInt2) PassThroughBoolToInt(BoolToInt2 param)
{
	return param;
}

STRUCTLIB_FUNC(BoolArray) PassThroughBoolArray(BoolArray param)
{
	return param;
}

STRUCTLIB_FUNC(bool) VerifyReservedBits(BitField2 param)
{
	return param.reservedBits == 20;
}

STRUCTLIB_FUNC(void) CustomNativeNewTest(CustomNativeNew param)
{
}

STRUCTLIB_FUNC(StructWithInterface) GetStructWithInterface()
{
	return {new Implementation()};
}

STRUCTLIB_FUNC(StructWithInterface) PassThroughStructWithInterface(StructWithInterface param)
{
	return param;
}

STRUCTLIB_FUNC(PointerSizeMember) PassThroughPointerSizeMember(PointerSizeMember param)
{
	return param;
}