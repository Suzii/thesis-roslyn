// Copyright (c) Zuzana Dankovcikova. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace BugHunter.Core.Constants
{
    /// <summary>
    /// Enum defining the analyzer categories
    /// </summary>
    public enum AnalyzerCategories
    {
        /// <summary>
        /// Category for analyzers reporting usage of specific implementation over an appropriate abstraction
        /// </summary>
        AbstractionOverImplementation,

        /// <summary>
        /// Category for analyzers reporting unintended or error-prone usage of CMS API
        /// </summary>
        CmsApiGuidelines,

        /// <summary>
        /// Category for analyzers looking for standard .NET properties and methods with CMS equivalent that should be used instead
        /// </summary>
        CmsApiReplacements,

        /// <summary>
        /// Category for analyzers reporting inheritance of conventional .NET class over a CMS abstract class
        /// </summary>
        CmsBaseClasses,

        /// <summary>
        /// Category for analyzers reporting suspicious usage of String comparison and manipulation methods without culture specification
        /// </summary>
        StringAndCulture,
    }
}
