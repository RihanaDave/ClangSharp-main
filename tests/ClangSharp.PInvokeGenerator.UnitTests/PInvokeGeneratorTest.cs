// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Xml.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTranslationUnit_Flags;

namespace ClangSharp.UnitTests;

public abstract class PInvokeGeneratorTest
{
    protected const string DefaultInputFileName = "ClangUnsavedFile.h";
    protected const string DefaultLibraryPath = "ClangSharpPInvokeGenerator";
    protected const string DefaultNamespaceName = "ClangSharp.Test";

    protected const CXTranslationUnit_Flags DefaultTranslationUnitFlags = CXTranslationUnit_IncludeAttributedTypes          // Include attributed types in CXType
                                                                        | CXTranslationUnit_VisitImplicitAttributes         // Implicit attributes should be visited
                                                                        | CXTranslationUnit_DetailedPreprocessingRecord;
    protected const string DefaultCStandard = "c17";
    protected const string DefaultCppStandard = "c++17";

    protected static readonly string[] DefaultCClangCommandLineArgs =
    [
        $"-std={DefaultCStandard}",                             // The input files should be compiled for C 17
        "-xc",                                  // The input files are C
    ];

    protected static readonly string[] DefaultCppClangCommandLineArgs =
    [
        $"-std={DefaultCppStandard}",                           // The input files should be compiled for C++ 17
        "-xc++",                                // The input files are C++
        "-Wno-pragma-once-outside-header",      // We are processing files which may be header files
        "-Wno-c++11-narrowing"
    ];

    protected static string EscapeXml(string value) => new XText(value).ToString();

    protected static Task ValidateGeneratedCSharpPreviewWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpPreviewUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpLatestWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateLatestCode | PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpLatestUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateLatestCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpDefaultWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpDefaultUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpCompatibleWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedCSharpCompatibleUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.CSharp, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlPreviewWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlPreviewUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GeneratePreviewCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlLatestWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateLatestCode | PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlLatestUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateLatestCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlDefaultWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.None | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlDefaultUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlCompatibleWindowsBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    protected static Task ValidateGeneratedXmlCompatibleUnixBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorConfigurationOptions additionalConfigOptions = PInvokeGeneratorConfigurationOptions.None, string[]? excludedNames = null, IReadOnlyDictionary<string, string>? remappedNames = null, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes = null, IReadOnlyDictionary<string, string>? withCallConvs = null, IReadOnlyDictionary<string, string>? withClasses = null, IReadOnlyDictionary<string, string>? withLibraryPaths = null, IReadOnlyDictionary<string, string>? withNamespaces = null, string[]? withSetLastErrors = null, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs = null, IReadOnlyDictionary<string, string>? withTypes = null, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings = null, IReadOnlyDictionary<string, string>? withPackings = null, IEnumerable<Diagnostic>? expectedDiagnostics = null, string libraryPath = DefaultLibraryPath, string[]? commandLineArgs = null, string language = "c++", string languageStandard = DefaultCppStandard)
        => ValidateGeneratedBindingsAsync(inputContents, expectedOutputContents, PInvokeGeneratorOutputMode.Xml, PInvokeGeneratorConfigurationOptions.GenerateCompatibleCode | PInvokeGeneratorConfigurationOptions.GenerateUnixTypes | additionalConfigOptions, excludedNames, remappedNames, withAccessSpecifiers, withAttributes, withCallConvs, withClasses, withLibraryPaths, withNamespaces, withSetLastErrors, withTransparentStructs, withTypes, withUsings, withPackings, expectedDiagnostics, libraryPath, commandLineArgs, language, languageStandard);

    private static async Task ValidateGeneratedBindingsAsync(string inputContents, string expectedOutputContents, PInvokeGeneratorOutputMode outputMode, PInvokeGeneratorConfigurationOptions configOptions, string[]? excludedNames, IReadOnlyDictionary<string, string>? remappedNames, IReadOnlyDictionary<string, AccessSpecifier>? withAccessSpecifiers, IReadOnlyDictionary<string, IReadOnlyList<string>>? withAttributes, IReadOnlyDictionary<string, string>? withCallConvs, IReadOnlyDictionary<string, string>? withClasses, IReadOnlyDictionary<string, string>? withLibraryPaths, IReadOnlyDictionary<string, string>? withNamespaces, string[]? withSetLastErrors, IReadOnlyDictionary<string, (string, PInvokeGeneratorTransparentStructKind)>? withTransparentStructs, IReadOnlyDictionary<string, string>? withTypes, IReadOnlyDictionary<string, IReadOnlyList<string>>? withUsings, IReadOnlyDictionary<string, string>? withPackings, IEnumerable<Diagnostic>? expectedDiagnostics, string libraryPath, string[]? commandLineArgs, string language, string languageStandard)
    {
        Assert.True(File.Exists(DefaultInputFileName));
        commandLineArgs ??= DefaultCppClangCommandLineArgs;

        configOptions |= PInvokeGeneratorConfigurationOptions.GenerateMacroBindings;

        using var outputStream = new MemoryStream();
        using var unsavedFile = CXUnsavedFile.Create(DefaultInputFileName, inputContents);

        var unsavedFiles = new CXUnsavedFile[] { unsavedFile };
        var config = new PInvokeGeneratorConfiguration(language, languageStandard, DefaultNamespaceName, Path.GetRandomFileName(), headerFile: null, outputMode, configOptions) {
            DefaultClass = null,
            ExcludedNames = excludedNames,
            IncludedNames = null,
            LibraryPath = libraryPath,
            MethodPrefixToStrip = null,
            RemappedNames = remappedNames,
            TraversalNames = null,
            TestOutputLocation = null,
            WithAccessSpecifiers = withAccessSpecifiers,
            WithAttributes = withAttributes,
            WithCallConvs = withCallConvs,
            WithClasses = withClasses,
            WithLibraryPaths = withLibraryPaths,
            WithManualImports = null,
            WithNamespaces = withNamespaces,
            WithSetLastErrors = withSetLastErrors,
            WithSuppressGCTransitions = null,
            WithTransparentStructs = withTransparentStructs,
            WithTypes = withTypes,
            WithUsings = withUsings,
            WithPackings = withPackings,
        };

        using (var pinvokeGenerator = new PInvokeGenerator(config, (path) => outputStream))
        {
            var handle = CXTranslationUnit.Parse(pinvokeGenerator.IndexHandle, DefaultInputFileName, commandLineArgs, unsavedFiles, DefaultTranslationUnitFlags);

            using var translationUnit = TranslationUnit.GetOrCreate(handle);
            Debug.Assert(translationUnit is not null);

            pinvokeGenerator.GenerateBindings(translationUnit, DefaultInputFileName, commandLineArgs, DefaultTranslationUnitFlags);

            if (expectedDiagnostics is null)
            {
                Assert.IsEmpty(pinvokeGenerator.Diagnostics);
            }
            else
            {
                Assert.AreEqual(expectedDiagnostics, pinvokeGenerator.Diagnostics);
            }
        }
        outputStream.Position = 0;

        using var streamReader = new StreamReader(outputStream);
        var actualOutputContents = await streamReader.ReadToEndAsync().ConfigureAwait(false);
        Assert.AreEqual(expectedOutputContents, actualOutputContents);
    }
}