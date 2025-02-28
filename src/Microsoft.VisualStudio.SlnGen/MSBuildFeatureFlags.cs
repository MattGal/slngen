﻿// Copyright (c) Microsoft Corporation.
//
// Licensed under the MIT license.

using System;

namespace Microsoft.VisualStudio.SlnGen
{
    /// <summary>
    /// Represents a class used to set MSBuild feature flags.
    /// </summary>
    internal sealed class MSBuildFeatureFlags : IDisposable
    {
        /// <summary>
        /// Represents the name of the environment variable to set that specifies to cache wildcard enumerations.
        /// </summary>
        private const string CacheFileEnumerationsEnvironmentVariableName = "MSBUILDCACHEFILEENUMERATIONS";

        /// <summary>
        /// Represents the name of the environment variable to set that specifies to load all files as read-only.
        /// </summary>
        private const string LoadAllFilesAsReadonlyEnvironmentVariableName = "MSBUILDLOADALLFILESASREADONLY";

        /// <summary>
        /// Represents the name of the environment variable to set that specifies the full path to MSBuild.exe.
        /// </summary>
        private const string MSBuildExePathEnvironmentVariableName = "MSBUILD_EXE_PATH";

        /// <summary>
        /// Represents the name of the environment variable to set that specifies a list of wildcard regular expressions to skip the evaluation of.
        /// </summary>
        private const string SkipWildcardEvaluationRegularExpressionsEnvironmentVariableName = "MSBUILDSKIPEAGERWILDCARDEVALUATIONREGEXES";

        /// <summary>
        /// Represents a regular expression that matches any file spec that contains a wildcard * or ? and does not end in "proj".
        /// </summary>
        private const string SkipWildcardRegularExpression = @"[*?]+.*(?<!proj)$";

        /// <summary>
        /// Represents the name of the environment variable to set that specifies to use the simple project root element cache.
        /// </summary>
        private const string UseSimpleProjectRootElementCacheConcurrencyEnvironmentVariableName = "MSBUILDUSESIMPLEPROJECTROOTELEMENTCACHECONCURRENCY";

        private readonly IEnvironmentProvider _environmentProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MSBuildFeatureFlags"/> class.
        /// </summary>
        /// <param name="environmentProvider">An <see cref="IEnvironmentProvider" /> instance to use when manipulating the environment.</param>
        public MSBuildFeatureFlags(IEnvironmentProvider environmentProvider)
        {
            _environmentProvider = environmentProvider;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MSBuildFeatureFlags"/> class.
        /// </summary>
        public MSBuildFeatureFlags()
            : this(SystemEnvironmentProvider.Instance)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether wildcard expansions for the entire process should be cached.
        /// </summary>
        /// <remarks>
        /// More info here: https://github.com/microsoft/msbuild/blob/master/src/Shared/Traits.cs#L55.
        /// </remarks>
        public bool CacheFileEnumerations
        {
            get => string.Equals(_environmentProvider.GetEnvironmentVariable(CacheFileEnumerationsEnvironmentVariableName), "1");
            set => _environmentProvider.SetEnvironmentVariable(CacheFileEnumerationsEnvironmentVariableName, value ? "1" : null);
        }

        /// <summary>
        /// Gets or sets a value indicating whether all projects should be treated as read-only which enables an optimized way of
        /// reading them.
        /// </summary>
        /// <remarks>
        /// More info here: https://github.com/microsoft/msbuild/blob/master/src/Build/ElementLocation/XmlDocumentWithLocation.cs#L392.
        /// </remarks>
        public bool LoadAllFilesAsReadOnly
        {
            get => string.Equals(_environmentProvider.GetEnvironmentVariable(LoadAllFilesAsReadonlyEnvironmentVariableName), "1");
            set => _environmentProvider.SetEnvironmentVariable(LoadAllFilesAsReadonlyEnvironmentVariableName, value ? "1" : null);
        }

        /// <summary>
        /// Gets or sets the full path to MSBuild that should be used to evaluate projects.
        /// </summary>
        /// <remarks>
        /// MSBuild is not installed globally anymore as of version 15.0.  Processes doing evaluations must set this environment variable for the toolsets
        /// to be found by MSBuild (stuff like $(MSBuildExtensionsPath).
        /// More info here: https://github.com/microsoft/msbuild/blob/master/src/Shared/BuildEnvironmentHelper.cs#L125.
        /// </remarks>
        public string MSBuildExePath
        {
            get => _environmentProvider.GetEnvironmentVariable(MSBuildExePathEnvironmentVariableName);
            set => _environmentProvider.SetEnvironmentVariable(MSBuildExePathEnvironmentVariableName, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether MSBuild should skip expanding wildcards.
        /// </summary>
        /// <remarks>
        /// More info here: https://github.com/microsoft/msbuild/blob/master/src/Build/Utilities/EngineFileUtilities.cs#L221.
        /// </remarks>
        public bool MSBuildSkipEagerWildCardEvaluationRegexes
        {
            get => !string.Equals(_environmentProvider.GetEnvironmentVariable(SkipWildcardEvaluationRegularExpressionsEnvironmentVariableName), null);
            set => _environmentProvider.SetEnvironmentVariable(SkipWildcardEvaluationRegularExpressionsEnvironmentVariableName, value ? SkipWildcardRegularExpression : null);
        }

        /// <summary>
        /// Gets or sets a value indicating whether MSBuild should use simple project root element cache concurrency.
        /// </summary>
        /// <remarks>
        /// More info here: https://github.com/microsoft/msbuild/blob/5d872c945f2fb42a26ed67791f4bdceb458f1402/src/Shared/Traits.cs#L50.
        /// </remarks>
        public bool UseSimpleProjectRootElementCacheConcurrency
        {
            get => !string.Equals(_environmentProvider.GetEnvironmentVariable(UseSimpleProjectRootElementCacheConcurrencyEnvironmentVariableName), "1");
            set => _environmentProvider.SetEnvironmentVariable(UseSimpleProjectRootElementCacheConcurrencyEnvironmentVariableName, value ? "1" : null);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _environmentProvider.SetEnvironmentVariable(CacheFileEnumerationsEnvironmentVariableName, null);
            _environmentProvider.SetEnvironmentVariable(LoadAllFilesAsReadonlyEnvironmentVariableName, null);
            _environmentProvider.SetEnvironmentVariable(MSBuildExePathEnvironmentVariableName, null);
            _environmentProvider.SetEnvironmentVariable(SkipWildcardEvaluationRegularExpressionsEnvironmentVariableName, null);
            _environmentProvider.SetEnvironmentVariable(UseSimpleProjectRootElementCacheConcurrencyEnvironmentVariableName, null);
        }
    }
}