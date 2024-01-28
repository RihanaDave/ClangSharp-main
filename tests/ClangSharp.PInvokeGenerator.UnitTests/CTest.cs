// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

public sealed class CTest : PInvokeGeneratorTest
{
    [Test]
    public Task BasicTest()
    {
        var inputContents = @"typedef enum MyEnum {
    MyEnum_Value0,
    MyEnum_Value1,
    MyEnum_Value2,
} enum_t;

typedef struct MyStruct {
    enum_t _field;
} struct_t;
";
        string expectedOutputContents;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            expectedOutputContents = @"namespace ClangSharp.Test
{
    public enum MyEnum
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }

    public partial struct MyStruct
    {
        [NativeTypeName(""enum_t"")]
        public MyEnum _field;
    }
}
";
        }
        else
        {
            expectedOutputContents = @"namespace ClangSharp.Test
{
    [NativeTypeName(""unsigned int"")]
    public enum MyEnum : uint
    {
        MyEnum_Value0,
        MyEnum_Value1,
        MyEnum_Value2,
    }

    public partial struct MyStruct
    {
        [NativeTypeName(""enum_t"")]
        public MyEnum _field;
    }
}
";
        }

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task EnumTest()
    {
        var inputContents = @"enum {
    VALUE1 = 0,
    VALUE2,
    VALUE3
};

struct MyStruct {
    enum {
        VALUEA = 0,
        VALUEB,
        VALUEC
    } field;
};
";
        string expectedOutputContents;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName(""__AnonymousEnum_ClangUnsavedFile_L8_C5"")]
        public int field;

        public const int VALUEA = 0;
        public const int VALUEB = 1;
        public const int VALUEC = 2;
    }

    public static partial class Methods
    {
        public const int VALUE1 = 0;
        public const int VALUE2 = 1;
        public const int VALUE3 = 2;
    }
}
";
        }
        else
        {
            expectedOutputContents = @"namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName(""__AnonymousEnum_ClangUnsavedFile_L8_C5"")]
        public uint field;

        public const uint VALUEA = 0;
        public const uint VALUEB = 1;
        public const uint VALUEC = 2;
    }

    public static partial class Methods
    {
        public const uint VALUE1 = 0;
        public const uint VALUE2 = 1;
        public const uint VALUE3 = 2;
    }
}
";
        }

        var diagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L1_C1. Mapping values as constants in: Methods", "Line 1, Column 1 in ClangUnsavedFile.h"),
            new Diagnostic(DiagnosticLevel.Info, "Found anonymous enum: __AnonymousEnum_ClangUnsavedFile_L8_C5. Mapping values as constants in: Methods", "Line 8, Column 5 in ClangUnsavedFile.h")
        };
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, expectedDiagnostics: diagnostics, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task StructTest()
    {
        var inputContents = @"typedef struct _MyStruct
{
    int _field;
} MyStruct;

typedef struct _MyOtherStruct
{
    MyStruct _field1;
    MyStruct* _field2;
} MyOtherStruct;

typedef struct _MyStructWithAnonymousStruct
{
    struct {
        int _field;
    } _anonymousStructField1;
} MyStructWithAnonymousStruct;

typedef struct _MyStructWithAnonymousUnion
{
    union {
        int _field1;
        int* _field2;
    } union1;
} MyStructWithAnonymousUnion;
";
        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct _MyStruct
    {
        public int _field;
    }

    public unsafe partial struct _MyOtherStruct
    {
        [NativeTypeName(""MyStruct"")]
        public _MyStruct _field1;

        [NativeTypeName(""MyStruct *"")]
        public _MyStruct* _field2;
    }

    public partial struct _MyStructWithAnonymousStruct
    {
        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L14_C5"")]
        public __anonymousStructField1_e__Struct _anonymousStructField1;

        public partial struct __anonymousStructField1_e__Struct
        {
            public int _field;
        }
    }

    public partial struct _MyStructWithAnonymousUnion
    {
        [NativeTypeName(""__AnonymousRecord_ClangUnsavedFile_L21_C5"")]
        public _union1_e__Union union1;

        [StructLayout(LayoutKind.Explicit)]
        public unsafe partial struct _union1_e__Union
        {
            [FieldOffset(0)]
            public int _field1;

            [FieldOffset(0)]
            public int* _field2;
        }
    }
}
";
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task UnionTest()
    {
        var inputContents = @"typedef union _MyUnion
{
    int _field;
} MyUnion;

typedef union _MyOtherUnion
{
    MyUnion _field1;
    MyUnion* _field2;
} MyOtherUnion;
";
        var expectedOutputContents = @"using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct _MyUnion
    {
        [FieldOffset(0)]
        public int _field;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe partial struct _MyOtherUnion
    {
        [FieldOffset(0)]
        [NativeTypeName(""MyUnion"")]
        public _MyUnion _field1;

        [FieldOffset(0)]
        [NativeTypeName(""MyUnion *"")]
        public _MyUnion* _field2;
    }
}
";
        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }

    [Test]
    public Task MacroTest()
    {
        var inputContents = @"#define MyMacro1 5
#define MyMacro2 MyMacro1";

        var expectedOutputContents = @"namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [NativeTypeName(""#define MyMacro1 5"")]
        public const int MyMacro1 = 5;

        [NativeTypeName(""#define MyMacro2 MyMacro1"")]
        public const int MyMacro2 = 5;
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(inputContents, expectedOutputContents, commandLineArgs: DefaultCClangCommandLineArgs, language: "c", languageStandard: DefaultCStandard);
    }
}
