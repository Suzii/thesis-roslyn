﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BugHunter.CsRules {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CsResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CsResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BugHunter.CsRules.CsResources", typeof(CsResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Replace with &quot;{0}&quot;..
        /// </summary>
        internal static string ApiReplacements_CodeFix {
            get {
                return ResourceManager.GetString("ApiReplacements_CodeFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} should not be used. Use {1} instead..
        /// </summary>
        internal static string ApiReplacements_Description {
            get {
                return ResourceManager.GetString("ApiReplacements_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is accessed directly from {1}..
        /// </summary>
        internal static string ApiReplacements_MessageFormat {
            get {
                return ResourceManager.GetString("ApiReplacements_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} should not be used. Use {1} instead..
        /// </summary>
        internal static string ApiReplacements_Title {
            get {
                return ResourceManager.GetString("ApiReplacements_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Replace with {0}()..
        /// </summary>
        internal static string BH1000_CodeFix {
            get {
                return ResourceManager.GetString("BH1000_CodeFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Consider using one of WhereStartsWith(), WhereEndsWith() or WhereContains() methods instead..
        /// </summary>
        internal static string BH1000_Description {
            get {
                return ResourceManager.GetString("BH1000_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method {0} is used without Architect/CTO approval..
        /// </summary>
        internal static string BH1000_MessageFormat {
            get {
                return ResourceManager.GetString("BH1000_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Method WhereLike() or WhereNotLike() should not be used used..
        /// </summary>
        internal static string BH1000_Title {
            get {
                return ResourceManager.GetString("BH1000_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Replace &quot;{0}&quot; with &quot;{1}&quot;..
        /// </summary>
        internal static string BH1001_CodeFix {
            get {
                return ResourceManager.GetString("BH1001_CodeFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LogEvent called with event type &quot;{0}&quot;, use EventType.{1} instead..
        /// </summary>
        internal static string BH1001_Description {
            get {
                return ResourceManager.GetString("BH1001_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LogEvent called with event type &quot;{0}&quot;..
        /// </summary>
        internal static string BH1001_MessageFormat {
            get {
                return ResourceManager.GetString("BH1001_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LogEvent should not be called with hardcoded event type..
        /// </summary>
        internal static string BH1001_Title {
            get {
                return ResourceManager.GetString("BH1001_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Replace &quot;Request.UserHostAddress&quot; with &quot;RequestContext.UserHostAddress&quot;..
        /// </summary>
        internal static string BH1002_CodeFix {
            get {
                return ResourceManager.GetString("BH1002_CodeFix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Request.UserHostAddress property is used. Use RequestContext.UserHostAddress instead..
        /// </summary>
        internal static string BH1002_Description {
            get {
                return ResourceManager.GetString("BH1002_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Property Request.UserHostAddress is being accessed..
        /// </summary>
        internal static string BH1002_MessageFormat {
            get {
                return ResourceManager.GetString("BH1002_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Request.UserHostAddress should not be used..
        /// </summary>
        internal static string BH1002_Title {
            get {
                return ResourceManager.GetString("BH1002_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;Session[]&quot; should not be used. Use &quot;SessionHelper.GetValue()&quot; instead..
        /// </summary>
        internal static string HttpSessionElementAccess_Description {
            get {
                return ResourceManager.GetString("HttpSessionElementAccess_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;{0}&quot; should not be used. Use &quot;SessionHelper.GetValue()&quot; instead..
        /// </summary>
        internal static string HttpSessionElementAccess_MessageFormat {
            get {
                return ResourceManager.GetString("HttpSessionElementAccess_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;Session[]&quot; should not be used. Use &quot;SessionHelper.GetValue()&quot; instead..
        /// </summary>
        internal static string HttpSessionElementAccess_Title {
            get {
                return ResourceManager.GetString("HttpSessionElementAccess_Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;Session.SessionID&quot; should not be used. Use &quot;SessionHelper.GetSessionID()&quot; instead..
        /// </summary>
        internal static string HttpSessionSessionId_Description {
            get {
                return ResourceManager.GetString("HttpSessionSessionId_Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;{0}&quot; should not be used. Use &quot;SessionHelper.GetSessionID()&quot; instead..
        /// </summary>
        internal static string HttpSessionSessionId_MessageFormat {
            get {
                return ResourceManager.GetString("HttpSessionSessionId_MessageFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &quot;Session.SessionID&quot; should not be used. Use &quot;SessionHelper.GetSessionID()&quot; instead..
        /// </summary>
        internal static string HttpSessionSessionId_Title {
            get {
                return ResourceManager.GetString("HttpSessionSessionId_Title", resourceCulture);
            }
        }
    }
}
