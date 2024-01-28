// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class MacroDefinitionRecord : PreprocessingDirective
{
    internal MacroDefinitionRecord(CXCursor handle) : base(handle, CXCursor_MacroDefinition)
    {
    }

    public bool IsFunctionLike => Handle.IsMacroFunctionLike;

    public bool IsBuiltinMacro => Handle.IsMacroBuiltIn;

    public string Name => Handle.Spelling.CString;
}
