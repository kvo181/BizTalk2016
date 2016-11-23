﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.BizTalk.CAT.BestPractices.Framework.Properties {
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
    internal class ExceptionMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.BizTalk.CAT.BestPractices.Framework.Properties.ExceptionMessages", typeof(ExceptionMessages).Assembly);
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
        ///   Looks up a localized string similar to The specified argument {0} cannot be initialised with its default value..
        /// </summary>
        internal static string ArgumentCannotBeDefault {
            get {
                return ResourceManager.GetString("ArgumentCannotBeDefault", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The body part is not available for this message..
        /// </summary>
        internal static string BodyPartNotFound {
            get {
                return ResourceManager.GetString("BodyPartNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The execution of the configuration policy {0} has failed. Please inspect the exception details for more information..
        /// </summary>
        internal static string ConfigPolicyExecutionFailed {
            get {
                return ResourceManager.GetString("ConfigPolicyExecutionFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The current application settings do not contain a valid name of the configuration policy. Please add a configPolicy key into the appSettings section of the application configuration file..
        /// </summary>
        internal static string ConfigPolicyKeyNotFound {
            get {
                return ResourceManager.GetString("ConfigPolicyKeyNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value of the argument {0} provided for the enumeration {1} is invalid..
        /// </summary>
        internal static string InvalidEnumValue {
            get {
                return ResourceManager.GetString("InvalidEnumValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} component does not configure its description using the Description property. Please make sure that this property is set..
        /// </summary>
        internal static string MissingComponentDescription {
            get {
                return ResourceManager.GetString("MissingComponentDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} component does not have a valid GUID. Please make sure that the corresponding class is decorated with a GuidAttribute attribute..
        /// </summary>
        internal static string MissingComponentGuid {
            get {
                return ResourceManager.GetString("MissingComponentGuid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} component does not configure its name using the Name property. Please make sure that this property is set..
        /// </summary>
        internal static string MissingComponentName {
            get {
                return ResourceManager.GetString("MissingComponentName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The list of parameters is too long. The maximum allowed number of parameters is {0}..
        /// </summary>
        internal static string ParameterListTooLong {
            get {
                return ResourceManager.GetString("ParameterListTooLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to acquire a PipelineContext instance. The context has not as yet been initialized. Please make sure that you access the PipelineContext instance after it is fully initialized..
        /// </summary>
        internal static string PipelineContextNotInitialized {
            get {
                return ResourceManager.GetString("PipelineContextNotInitialized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified string argument {0} must not be empty..
        /// </summary>
        internal static string StringCannotBeEmpty {
            get {
                return ResourceManager.GetString("StringCannotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified type {0} is not compatible with {1}..
        /// </summary>
        internal static string TypeNotCompatible {
            get {
                return ResourceManager.GetString("TypeNotCompatible", resourceCulture);
            }
        }
    }
}
