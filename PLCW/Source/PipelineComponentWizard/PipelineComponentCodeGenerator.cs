using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Component.Utilities;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace MartijnHoogendoorn.BizTalk.Wizards.PipeLineComponentWizard
{
	/// <summary>
	/// PipelineComponentCodeGenerator is responsible for generating the code 
	/// for the pipeline component itself
	/// </summary>
	public static class PipelineComponentCodeGenerator
	{
        /// <summary>
        /// contains the options we use to generate the sourcecode
        /// </summary>
		private static CodeGeneratorOptions cgo;

	    /// <summary>
        /// static constructor, sets general options for code generation.
        /// </summary>
		static PipelineComponentCodeGenerator() 
		{
            cgo = new CodeGeneratorOptions();

            // removed, this is the default value according to documentation
            // cgo.BlankLinesBetweenMembers = true;

			cgo.BracingStyle = "C";

            // we don't want CodeDOM to rearrange our defined members to do
            // property-first aligment. this is to protect region directives we define
            cgo.VerbatimOrder = true;
		}

		#region retrieveCodeGenerator, generates an instance of the requested ICodeGenerator class
        /// <summary>
        /// gets an <see cref="I:ICodeGenerator"/> instance for the specified language using the specified
        /// <see cref="C:StreamWriter"/> as it's output buffer
        /// </summary>
        /// <param name="sw">the <see cref="C:StreamWriter"/> instance to generate code into</param>
        /// <param name="language">the <see cref="C:implementationLanguages"/> value, defining which programming
        /// language the code should be generated in</param>
        /// <returns>the requested <see cref="C:ICodeGenerator"/> instance, if supported</returns>
		private static ICodeGenerator retrieveCodeGenerator(StreamWriter sw, implementationLanguages language)
		{
            // determine which language was requested
			switch(language)
			{
				case implementationLanguages.CSharp:
					return new CSharpCodeProvider().CreateGenerator(sw);
				case implementationLanguages.VBNet:
					return new VBCodeProvider().CreateGenerator(sw);
				default:
                    throw new NotImplementedException(string.Format("The requested language support ({0}) has not been implemented.", Enum.GetName(typeof(implementationLanguages), language)));
			}
		}
		#endregion

		#region generatePipelineComponent, responsible for using CodeDOM to actually create the used class 
		/// <summary>
		/// generates the main class this pipeline component revolves around, uses CodeCOM to generate
		/// code and store it in "fileName"
		/// </summary>
		/// <param name="fileName">the output filename to use</param>
		/// <param name="clsNameSpace">the class' namespace</param>
		/// <param name="clsClassName">the class' name</param>
		/// <param name="designerProperties">any designer properties defined</param>
		/// <param name="componentCategory">the used component category</param>
		internal static void generatePipelineComponent(
			string fileName,
			string clsNameSpace,
			string clsClassName,
			bool implementsIProbeMessage,
			Hashtable designerProperties,
			componentTypes componentCategory,
			implementationLanguages language)
		{
			#region variable definition
			// the component GUID, used to represent the component uniquely
			Guid componentGuid = Guid.NewGuid();

			// the classes normally used within our component
			string[] usingClauses = new string[] {
													 "System",
													 "System.IO",
													 "System.Text",
													 "System.Drawing",
													 "System.Resources",
													 "System.Reflection",
													 "System.Diagnostics",
													 "System.Collections",
													 "System.ComponentModel",
													 "Microsoft.BizTalk.Message.Interop",
													 "Microsoft.BizTalk.Component.Interop",
													 "Microsoft.BizTalk.Component",
													 "Microsoft.BizTalk.Messaging",
                                                     "Microsoft.BizTalk.Streaming"
			};

			string[] baseTypes = new string[] {
												  "IBaseComponent",
												  "IPersistPropertyBag",
												  "IComponentUI",
			};
			#endregion

            // create the output file
			using(Stream codeFile = File.Open(fileName, FileMode.Create))
			{
                // enable writing to the selected output stream
				using(StreamWriter sw = new StreamWriter(codeFile))
				{
					// codeGenerator will contain the generator for the choosen language
					ICodeGenerator codeGenerator = retrieveCodeGenerator(sw, language);

					// create our namespace, in which our class will reside
					CodeNamespace cnsCodeNamespace = new CodeNamespace(clsNameSpace);

					CodeThisReferenceExpression _thisObject = new CodeThisReferenceExpression();

					#region using clauses
                    // iterate the entire using clause list
					foreach(string usingClause in usingClauses)
					{
                        // and add Import / using clauses for it
						cnsCodeNamespace.Imports.Add(new CodeNamespaceImport(usingClause));
					}

                    // SchemaList is defined inside Microsoft.BizTalk.Component.Utilities
					if(DesignerVariableType.SchemaListUsed)
					{
                        // so add it to the Import / using clauses
						cnsCodeNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.BizTalk.Component.Utilities"));
					}
					#endregion

					// create our class, this variable will entail the entire definition until
                    // we write it out
					CodeTypeDeclaration clsDecleration = new CodeTypeDeclaration();

                    // start by setting the name of the class
					clsDecleration.Name = clsClassName;

					#region class attributes

					// add our attributes
					clsDecleration.CustomAttributes.Add(
						new CodeAttributeDeclaration(
						"ComponentCategory", new CodeAttributeArgument(new CodeTypeReferenceExpression("CategoryTypes.CATID_PipelineComponent"))));

					// add our Guid
					clsDecleration.CustomAttributes.Add(
						new CodeAttributeDeclaration(
						"System.Runtime.InteropServices.Guid", new CodeAttributeArgument(new CodePrimitiveExpression(componentGuid.ToString()))));

					// add the component category
					clsDecleration.CustomAttributes.Add(
						new CodeAttributeDeclaration(
						"ComponentCategory", new CodeAttributeArgument(new CodeTypeReferenceExpression("CategoryTypes.CATID_" + componentCategory.ToString()))));

					#endregion

					#region resource manager

					// add our ResourceManager instance
					CodeMemberField cmf = new CodeMemberField(typeof(ResourceManager), "resourceManager");

                    // initialize the resourceManager by calling it's default constructor,
                    // passing Assembly.GetExecutingAssembly
					cmf.InitExpression = 
						new CodeObjectCreateExpression(
						typeof(ResourceManager), 
						new CodeExpression[] { 
								new CodeSnippetExpression("\"" + clsNameSpace + "." + clsClassName + "\""), 
								new CodeMethodInvokeExpression(
									new CodeVariableReferenceExpression("Assembly"),
									"GetExecutingAssembly") });

                    // set the accessor visibility
					cmf.Attributes = MemberAttributes.Private;

                    // finally, add the statement to the class
                    clsDecleration.Members.Add(cmf);
					#endregion

					#region designer properties

                    // process all properties of the class which will be exposed inside the designer
					CodeMemberProperty clsProperty = null;

                    // iterate all defined properties
					foreach(DictionaryEntry entry in designerProperties)
					{
                        // try and lookup the type as the variable
						Type designerPropertyType = DesignerVariableType.getType(entry.Value as string);

                        // add a member variable using the suggested name
						cmf = new CodeMemberField(designerPropertyType, "_" + entry.Key);

                        // set the visibility scope
						cmf.Attributes = MemberAttributes.Private;

                        // SchemaList has special needs because it's not a native variable type,
                        // it needs an instance
						if(designerPropertyType == typeof(SchemaList))
						{
							cmf.InitExpression = new CodeObjectCreateExpression(
								typeof(SchemaList), new CodeExpression[] { });
						}
                        // SchemaWithNone also has special needs because it's not a native variable type,
                        // it too needs an instance and a default parameter (empty selection string)
                        else if (designerPropertyType == typeof(SchemaWithNone))
						{
							cmf.InitExpression = new CodeObjectCreateExpression(
								typeof(SchemaWithNone), new CodeExpression[] { new CodePrimitiveExpression(String.Empty) });
						}

                        // add the member variable to the class definition
						clsDecleration.Members.Add(cmf);

                        // instantiate a new CodeMemberProperty, which defines a getter/setter combination
                        clsProperty = new CodeMemberProperty();
                        // set the name to reflect the designer property currently being iterated
						clsProperty.Name = entry.Key as string;
                        // set it's visibility to public to allow the VS.NET designer to reflect upon it
						clsProperty.Attributes = MemberAttributes.Public;
                        // set it's "return" type to reflect the Type we looked up earlier
						clsProperty.Type = new CodeTypeReference(designerPropertyType);
                        // indicate the Property has a Getter
						clsProperty.HasGet = true;
                        // add a simple Getter implementation to the getter (get { return <variableName>; ))
						clsProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("_" + entry.Key as string)));
                        // indicate the Property also has a Setter
						clsProperty.HasSet = true;
                        if (designerPropertyType == typeof(SchemaList))
                        {
                            ArrayList setStatementList = new ArrayList();

                            string varName = "_" + entry.Key as string;

                            // this._property = value;
                            setStatementList.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(varName), new CodePropertySetValueReferenceExpression()));

                            /*
                             * 
                             * add comment only, as this cannot correctly and neatly be
                             * done in CodeDOM
                             *  
                             * */
                            setStatementList.AddRange(
                                new CodeCommentStatementCollection(
                                    new CodeCommentStatement[] {
										new CodeCommentStatement("As CodeDOM is limited in it's implementation"),
                                        new CodeCommentStatement("(see http://blogs.msdn.com/bclteam/archive/2005/03/16/396915.aspx)"),
										new CodeCommentStatement("I don't provide any code for storing the "),
										new CodeCommentStatement("values of the SchemaList. Sorry about that..."),
									    new CodeCommentStatement("Hint: store the name and namespace of the various SchemaList"),
										new CodeCommentStatement("objects and use those to fill the propertybag when storing the"),
										new CodeCommentStatement("SchemaList") }));
                        }
                        else
                        {
                            clsProperty.SetStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("_" + entry.Key), new CodePropertySetValueReferenceExpression()));
                        }

						clsProperty.Attributes = MemberAttributes.Public;
						clsProperty.Attributes |= MemberAttributes.Final;
						clsDecleration.Members.Add(clsProperty);
                    }
					#endregion

                    // used to define #region elements to group output code
                    CodeRegionDirective crDirective = null;
                    // used to define #endregion directive
                    CodeRegionDirective crEndDirective = new CodeRegionDirective(CodeRegionMode.End, null);

                    #region implement IBaseComponent
                    crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IBaseComponent members");

                    #region IBaseComponent.Name
                    // first, add IBaseComponent.Name
					clsProperty = new CodeMemberProperty();

                    // add the #region directive
                    clsProperty.StartDirectives.Add(crDirective);

                    clsProperty.Name = "Name";
					clsProperty.HasSet = false;
					clsProperty.HasGet = true;
					clsProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("resourceManager.GetString(\"COMPONENTNAME\", System.Globalization.CultureInfo.InvariantCulture)")));
					clsProperty.Type = new CodeTypeReference(typeof(System.String));
					clsProperty.CustomAttributes.Add(new CodeAttributeDeclaration("Browsable", new CodeAttributeArgument(new CodePrimitiveExpression(false))));
					clsProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsProperty.Comments.Add(new CodeCommentStatement("Name of the component", true));
					clsProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsProperty.Attributes = MemberAttributes.Public;
					clsProperty.Attributes |= MemberAttributes.Final;
					clsProperty.ImplementationTypes.Clear();
					clsProperty.ImplementationTypes.Add(typeof(IBaseComponent));
					clsDecleration.Members.Add(clsProperty);
                    #endregion

                    #region IBaseComponent.Version
                    // next, add IBaseComponent.Version
					clsProperty = new CodeMemberProperty();
					clsProperty.Name = "Version";
					clsProperty.HasSet = false;
					clsProperty.HasGet = true;
					clsProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("resourceManager.GetString(\"COMPONENTVERSION\", System.Globalization.CultureInfo.InvariantCulture)")));
					clsProperty.Type = new CodeTypeReference(typeof(System.String));
					clsProperty.CustomAttributes.Add(
						new CodeAttributeDeclaration(
						"Browsable", new CodeAttributeArgument(new CodePrimitiveExpression(false))));
					clsProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsProperty.Comments.Add(new CodeCommentStatement("Version of the component", true));
					clsProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsProperty.Attributes = MemberAttributes.Public;
					clsProperty.Attributes |= MemberAttributes.Final;
					clsProperty.ImplementationTypes.Clear();
					clsProperty.ImplementationTypes.Add(typeof(IBaseComponent));
					clsDecleration.Members.Add(clsProperty);
                    #endregion

                    #region IBaseComponent.Description
                    // next, add IBaseComponent.Description
					clsProperty = new CodeMemberProperty();
					clsProperty.Name = "Description";
					clsProperty.HasSet = false;
					clsProperty.HasGet = true;
					clsProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("resourceManager.GetString(\"COMPONENTDESCRIPTION\", System.Globalization.CultureInfo.InvariantCulture)")));
					clsProperty.Type = new CodeTypeReference(typeof(System.String));
					clsProperty.CustomAttributes.Add(
						new CodeAttributeDeclaration(
						"Browsable", new CodeAttributeArgument(new CodePrimitiveExpression(false))));
					clsProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsProperty.Comments.Add(new CodeCommentStatement("Description of the component", true));
					clsProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsProperty.Attributes = MemberAttributes.Public;
					clsProperty.Attributes |= MemberAttributes.Final;
					//clsProperty.PrivateImplementationType.BaseType = "IBaseComponent";

					// the code generator thinks we inherit this from the first interface we implement
					// due to a "bug" in the CodeDOM. clear it.
					clsProperty.ImplementationTypes.Clear();
					clsProperty.ImplementationTypes.Add(typeof(IBaseComponent));

                    // add the #endregion directive
                    clsProperty.EndDirectives.Add(crEndDirective);

					clsDecleration.Members.Add(clsProperty);
                    #endregion

                    #endregion

                    #region implement IPersistPropertyBag

                    crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IPersistPropertyBag members");

                    #region IPersistPropertyBag.GetClassID

					// next, add IPersistPropertyBag.GetClassID
					CodeMemberMethod clsMethod = new CodeMemberMethod();

                    // add the #region directive
                    clsMethod.StartDirectives.Add(crDirective);

					clsMethod.Name = "GetClassID";
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Guid), "classid"));
					clsMethod.Parameters[0].Direction = FieldDirection.Out;
					clsMethod.Statements.Add(new CodeAssignStatement(new CodeArgumentReferenceExpression("classid"), new CodeObjectCreateExpression(typeof(Guid), new CodeExpression[] { new CodeSnippetExpression("\"" + componentGuid.ToString() + "\"") })));
					clsMethod.ReturnType = new CodeTypeReference(typeof(void));
					clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("Gets class ID of component for usage from unmanaged code.", true));
					clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"classid\">", true));
					clsMethod.Comments.Add(new CodeCommentStatement("Class ID of the component", true));
					clsMethod.Comments.Add(new CodeCommentStatement("</param>", true));
					clsMethod.Attributes = MemberAttributes.Public;
					clsMethod.Attributes |= MemberAttributes.Final;
					clsMethod.ImplementationTypes.Add(typeof(IPersistPropertyBag));
					clsDecleration.Members.Add(clsMethod);
					#endregion

					#region IPersistPropertyBag.InitNew
					// next, implement IPersistPropertyBag.InitNew
					clsMethod = new CodeMemberMethod();
					clsMethod.Name = "InitNew";
					clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("not implemented", true));
					clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsMethod.Attributes = MemberAttributes.Public;
					clsMethod.Attributes |= MemberAttributes.Final;
					clsMethod.ImplementationTypes.Add(typeof(IPersistPropertyBag));
					clsDecleration.Members.Add(clsMethod);
					#endregion

					#region IPersistPropertyBag.Load
					// next, implement IPersistPropertyBag.Load
					clsMethod = new CodeMemberMethod();
					clsMethod.Name = "Load";
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPropertyBag), "pb"));
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "errlog"));

					if(designerProperties.Count> 0)
					{
						// object val = null;
						clsMethod.Statements.Add(new CodeVariableDeclarationStatement(typeof(object), "val", new CodePrimitiveExpression(null)));
						foreach(DictionaryEntry entry in designerProperties)
						{
							// val = this.ReadPropertyBag(pb, "property");
							clsMethod.Statements.Add(
								new CodeAssignStatement(
								new CodeVariableReferenceExpression("val"),
								new CodeMethodInvokeExpression(
								new CodeMethodReferenceExpression(
								_thisObject, "ReadPropertyBag"),
								new CodeExpression[] { new CodeArgumentReferenceExpression("pb"), new CodeSnippetExpression("\"" + entry.Key + "\"") })));

							// typeof(variable)
							Type designerPropertyType = DesignerVariableType.getType(entry.Value as string);
							CodeAssignStatement assignment = null;

							if(designerPropertyType == typeof(SchemaList))
							{
								clsMethod.Statements.Add(
									new CodeSnippetStatement(
										string.Format(
											"#error please implement IPersistPropertyBag.Load for property \"{0}\"",
											entry.Key)));
							} 
							else if (designerPropertyType == typeof(SchemaWithNone))
							{
								// this._property = new SchemaWithNone((string) val);
								assignment = 
									new CodeAssignStatement(
										new CodeFieldReferenceExpression(_thisObject, "_" + entry.Key),
										new CodeObjectCreateExpression(
											designerPropertyType,
											new CodeCastExpression(
												typeof(string),
												new CodeVariableReferenceExpression("val"))));

								// if(val != null)
								clsMethod.Statements.Add(
									new CodeConditionStatement(
									new CodeBinaryOperatorExpression(
									new CodeVariableReferenceExpression("val"), 
									CodeBinaryOperatorType.IdentityInequality,
									new CodePrimitiveExpression(null)),
									assignment));
							}
							else
							{
								// this._property = (type) val;
								assignment = 
									new CodeAssignStatement(
										new CodeFieldReferenceExpression(_thisObject, "_" + entry.Key),
										new CodeCastExpression(DesignerVariableType.getType(entry.Value as string), new CodeVariableReferenceExpression("val")));

								// if(val != null)
								clsMethod.Statements.Add(
									new CodeConditionStatement(
									new CodeBinaryOperatorExpression(
									new CodeVariableReferenceExpression("val"), 
									CodeBinaryOperatorType.IdentityInequality,
									new CodePrimitiveExpression(null)),
									assignment));
							}
						}
					}

					clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("Loads configuration properties for the component", true));
					clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pb\">Configuration property bag</param>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"errlog\">Error status</param>", true));
					clsMethod.ReturnType = new CodeTypeReference(typeof(void));
					clsMethod.Attributes = MemberAttributes.Public;
					clsMethod.ImplementationTypes.Add(typeof(IPersistPropertyBag));
					clsDecleration.Members.Add(clsMethod);
					#endregion

					#region IPersistPropertyBag.Save
					// next, implement IPersistPropertyBag.Save
					clsMethod = new CodeMemberMethod();
					clsMethod.Name = "Save";
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPropertyBag), "pb"));
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "fClearDirty"));
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool), "fSaveAllProperties"));

					clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("Saves the current component configuration into the property bag", true));
					clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pb\">Configuration property bag</param>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"fClearDirty\">not used</param>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"fSaveAllProperties\">not used</param>", true));
					clsMethod.ReturnType = new CodeTypeReference(typeof(void));
					clsMethod.Attributes = MemberAttributes.Public;
					clsMethod.ImplementationTypes.Add(typeof(IPersistPropertyBag));

					foreach(DictionaryEntry entry in designerProperties)
					{
						Type designerPropertyType = DesignerVariableType.getType(entry.Value as string);

						if(designerPropertyType == typeof(SchemaList))
						{
							clsMethod.Statements.Add(
								new CodeSnippetStatement(
									string.Format(
										"#error please implement IPersistPropertyBag.Save for property \"{0}\"",
										entry.Key)));
						} 
						else if (designerPropertyType == typeof(SchemaWithNone))
						{
							clsMethod.Statements.Add(
								new CodeMethodInvokeExpression(
								_thisObject, "WritePropertyBag", new CodeExpression[] { new CodeArgumentReferenceExpression("pb"), new CodePrimitiveExpression(entry.Key), new CodeFieldReferenceExpression(new CodeFieldReferenceExpression(_thisObject, entry.Key as string), "SchemaName") }));
						}
						else
						{
							clsMethod.Statements.Add(
								new CodeMethodInvokeExpression(
								_thisObject, "WritePropertyBag", new CodeExpression[] { new CodeArgumentReferenceExpression("pb"), new CodePrimitiveExpression(entry.Key), new CodeFieldReferenceExpression(_thisObject, entry.Key as string) }));
						}
					}

					clsDecleration.Members.Add(clsMethod);
					#endregion

					#region safe version of ReadPropertyBag
                    crDirective = new CodeRegionDirective(CodeRegionMode.Start, "utility functionality");

                    // next, implement a safe version of ReadPropertyBag
					clsMethod = new CodeMemberMethod();

                    // add the #region directive
                    clsMethod.StartDirectives.Add(crDirective);

					clsMethod.Name = "ReadPropertyBag";
					clsMethod.ReturnType = new CodeTypeReference(typeof(object));
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPropertyBag), "pb"));
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "propName"));

					clsMethod.Statements.Add(new CodeVariableDeclarationStatement(typeof(object), "val", new CodePrimitiveExpression(null)));

					// try running pb.Read()
					CodeMethodInvokeExpression pbRead = 
						new CodeMethodInvokeExpression(
						new CodeMethodReferenceExpression(
						new CodeArgumentReferenceExpression("pb"), "Read"), 
						new CodeExpression[] { 
												 new CodeArgumentReferenceExpression("propName"), 
												 new CodeDirectionExpression(FieldDirection.Out, 
												 new CodeVariableReferenceExpression("val")), 
												 new CodePrimitiveExpression(0) });

					CodeMethodReturnStatement retVal = new CodeMethodReturnStatement(new CodeVariableReferenceExpression("val"));
					// if the choosen language supports try/catch blocks
					if(codeGenerator.Supports(GeneratorSupport.TryCatchStatements))
					{
						CodeTryCatchFinallyStatement tryCatch = new CodeTryCatchFinallyStatement();
						tryCatch.TryStatements.Add(pbRead);
						CodeCatchClause catchArgumentException = new CodeCatchClause();
						catchArgumentException.CatchExceptionType = new CodeTypeReference(typeof(ArgumentException));
						catchArgumentException.Statements.Add(retVal);
						tryCatch.CatchClauses.Add(catchArgumentException);

						CodeCatchClause catchException = new CodeCatchClause("e", new CodeTypeReference(typeof(Exception)));

						// if no dice, throw an ApplicationException
						catchException.Statements.Add(
							new CodeThrowExceptionStatement(
							new CodeObjectCreateExpression(
							new CodeTypeReference(typeof(ApplicationException)),
							new CodeExpression[] { new CodeVariableReferenceExpression("e.Message") } )));

						// add the exception handling to the try/catch block
						tryCatch.CatchClauses.Add(catchException);

						clsMethod.Statements.Add(tryCatch);
					} 
					else 
					{
						// unsafe
						clsMethod.Statements.Add(pbRead);
					}

					clsMethod.Statements.Add(retVal);

					clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("Reads property value from property bag", true));
					clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pb\">Property bag</param>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"propName\">Name of property</param>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<returns>Value of the property</returns>", true));
					clsDecleration.Members.Add(clsMethod);
					#endregion

					#region safe version of WritePropertyBag
					// next, implement a safe version of WritePropertyBag
					clsMethod = new CodeMemberMethod();
					clsMethod.Name = "WritePropertyBag";
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPropertyBag), "pb"));
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "propName"));
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "val"));

					CodeMethodInvokeExpression pbWrite = 
						new CodeMethodInvokeExpression(
						new CodeMethodReferenceExpression(
						new CodeArgumentReferenceExpression("pb"), "Write"), 
						new CodeExpression[] { 
												 new CodeArgumentReferenceExpression("propName"), 
												 new CodeDirectionExpression(FieldDirection.Ref, 
												 new CodeVariableReferenceExpression("val")) });

					if(codeGenerator.Supports(GeneratorSupport.TryCatchStatements))
					{
						CodeTryCatchFinallyStatement tryCatch = new CodeTryCatchFinallyStatement();
						tryCatch.TryStatements.Add(pbWrite);
						CodeCatchClause catchException = new CodeCatchClause("e", new CodeTypeReference(typeof(Exception)));

						// if no dice, throw an ApplicationException
						catchException.Statements.Add(
							new CodeThrowExceptionStatement(
							new CodeObjectCreateExpression(
							new CodeTypeReference(typeof(ApplicationException)),
							new CodeExpression[] { new CodeVariableReferenceExpression("e.Message") } )));

						// add the exception handling to the try/catch block
						tryCatch.CatchClauses.Add(catchException);

						clsMethod.Statements.Add(tryCatch);
					}
					else 
					{
						// unsafe
						clsMethod.Statements.Add(pbWrite);
					}

					clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("Writes property values into a property bag.", true));
					clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pb\">Property bag.</param>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"propName\">Name of property.</param>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"val\">Value of property.</param>", true));

                    // add the #endregion directive for the "utility functionality" methods
                    clsMethod.EndDirectives.Add(crEndDirective);

                    // add the #endregion directive for the IPersistPropertyBag implementation
                    clsMethod.EndDirectives.Add(crEndDirective);

					clsDecleration.Members.Add(clsMethod);
					#endregion

					#endregion

					#region implement IComponentUI

                    crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IComponentUI members");
                    
                    #region IComponentUI.Icon

					// next, implement IComponentUI.Icon property
					clsProperty = new CodeMemberProperty();

                    // add the #region directive
                    clsProperty.StartDirectives.Add(crDirective);

					clsProperty.Name = "Icon";
					clsProperty.HasSet = false;
					clsProperty.HasGet = true;

					clsProperty.GetStatements.Add(
						new CodeMethodReturnStatement(
						new CodeMethodInvokeExpression(
						new CodeCastExpression(
						typeof(Bitmap), 
						new CodeMethodInvokeExpression(
						new CodeFieldReferenceExpression(_thisObject, "resourceManager"), 
						"GetObject", 
						new CodeExpression[] { 
												 new CodeSnippetExpression("\"COMPONENTICON\""), 
												 new CodeSnippetExpression("System.Globalization.CultureInfo.InvariantCulture") })),
						"GetHicon", new CodeExpression[] {})));

					clsProperty.Type = new CodeTypeReference("IntPtr");
					clsProperty.CustomAttributes.Add(
						new CodeAttributeDeclaration(
						"Browsable", new CodeAttributeArgument(new CodePrimitiveExpression(false))));
					clsProperty.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsProperty.Comments.Add(new CodeCommentStatement("Component icon to use in BizTalk Editor", true));
					clsProperty.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsProperty.Attributes = MemberAttributes.Public;
					clsProperty.Attributes |= MemberAttributes.Final;
					clsProperty.ImplementationTypes.Add(typeof(IComponentUI));

					clsDecleration.Members.Add(clsProperty);
					#endregion

					#region IComponentUI.Validate
					// finally, implement IComponentUI.Validate
					clsMethod = new CodeMemberMethod();
					clsMethod.Name = "Validate";
					clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "obj"));
					
					// add sample code for the IComponentUI.Validate method
					clsMethod.Statements.Add(new CodeCommentStatement("example implementation:"));
					clsMethod.Statements.Add(new CodeCommentStatement("ArrayList errorList = new ArrayList();"));
					clsMethod.Statements.Add(new CodeCommentStatement("errorList.Add(\"This is a compiler error\");"));
					clsMethod.Statements.Add(new CodeCommentStatement("return errorList.GetEnumerator();"));

					clsMethod.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(null)));
					
					clsMethod.ReturnType = new CodeTypeReference(typeof(System.Collections.IEnumerator));

					clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("The Validate method is called by the BizTalk Editor during the build ", true));
					clsMethod.Comments.Add(new CodeCommentStatement("of a BizTalk project.", true));
					clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"obj\">An Object containing the configuration properties.</param>", true));
					clsMethod.Comments.Add(new CodeCommentStatement("<returns>The IEnumerator enables the caller to enumerate through a collection of strings containing error messages. These error messages appear as compiler error messages. To report successful property validation, the method should return an empty enumerator.</returns>", true));

					clsMethod.Attributes = MemberAttributes.Public;
					clsMethod.Attributes |= MemberAttributes.Final;
					clsMethod.ImplementationTypes.Add(typeof(IComponentUI));

                    clsMethod.EndDirectives.Add(crEndDirective);

					clsDecleration.Members.Add(clsMethod);
					#endregion

					#endregion

					switch(componentCategory)
                    {
                        #region AssemblingSerializer (IAssemblerComponent)
                        case componentTypes.AssemblingSerializer:
							// add another base type
							clsDecleration.BaseTypes.Add(typeof(IAssemblerComponent));

							// add a member variable to store the incoming message
							cmf = new CodeMemberField(typeof(ArrayList), "_inmsgs");
							cmf.InitExpression = 
								new CodeObjectCreateExpression(
								typeof(ArrayList),
								new CodeExpression[] {}); // no parameters

							cmf.Attributes = MemberAttributes.Private;
							clsDecleration.Members.Add(cmf);

                            #region implement IAssemblerComponent

                            crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IAssemblerComponent members");

                            #region IAssemblerComponent.AddDocument
                            // implement the IAssemblerComponent.AddDocument member
							clsMethod = new CodeMemberMethod();

                            // add #region directive
                            clsMethod.StartDirectives.Add(crDirective);

							clsMethod.Name = "AddDocument";
							clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
							clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IBaseMessage), "inmsg"));
							clsMethod.ReturnType = new CodeTypeReference(typeof(void));
							clsMethod.Statements.Add(new CodeCommentStatement("store the message for later use"));
							clsMethod.Statements.Add(
								new CodeMethodInvokeExpression(
								new CodeMethodReferenceExpression(
								new CodeVariableReferenceExpression("_inmsgs"), "Add"),
								new CodeExpression[] { new CodeArgumentReferenceExpression("inmsg") } ));

							clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("Adds the document inmsg to the list of messages that will be", true));
							clsMethod.Comments.Add(new CodeCommentStatement("included in the interchange.", true));
							clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">the current pipeline context</param>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"inmsg\">the message to be added</param>", true));

							clsMethod.Attributes = MemberAttributes.Public;
							clsMethod.Attributes |= MemberAttributes.Final;
							clsMethod.ImplementationTypes.Add(typeof(IAssemblerComponent));
							clsDecleration.Members.Add(clsMethod);
							#endregion

							#region  IAssemblerComponent.Assemble
							// implement the IAssemblerComponent.Assemble member
							clsMethod = new CodeMemberMethod();
							clsMethod.Name = "Assemble";
							clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
							clsMethod.ReturnType = new CodeTypeReference(typeof(IBaseMessage));
							clsMethod.Statements.Add(new CodeCommentStatement("TODO: implement assembling logic"));

							clsMethod.Statements.Add(new CodeCommentStatement("BizTalk 2004 documentation: \"In this release of BizTalk Server 2004,"));
							clsMethod.Statements.Add(new CodeCommentStatement("assembling functionality is not used, so BizTalk Server always passes"));
							clsMethod.Statements.Add(new CodeCommentStatement("one document to the component input.\""));
							clsMethod.Statements.Add(
								new CodeMethodReturnStatement(
								new CodeCastExpression(
								typeof(IBaseMessage), 
								new CodeArrayIndexerExpression(
								new CodeVariableReferenceExpression("_inmsgs"),
								new CodePrimitiveExpression(0)))));
						
							//new CodeSnippetExpression("return _inmsgs[0] as IBaseMessage"));

							clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("Builds the interchange from the messages that were added by the previous method. ", true));
							clsMethod.Comments.Add(new CodeCommentStatement("Returns a pointer to the assembled message.", true));
							clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">the current pipeline context</param>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<returns>the assembled message instance</returns>", true));

							clsMethod.Attributes = MemberAttributes.Public;
							clsMethod.Attributes |= MemberAttributes.Final;
							clsMethod.ImplementationTypes.Add(typeof(IAssemblerComponent));

                            // add #endregion
                            clsMethod.EndDirectives.Add(crEndDirective);

							clsDecleration.Members.Add(clsMethod);
							#endregion

                            #endregion

                            break;
                        #endregion

                        #region DisassemblingParser (IDisassemblerComponent, IProbeMessage)
                        case componentTypes.DisassemblingParser:
							clsDecleration.BaseTypes.Add(typeof(IDisassemblerComponent));

							// add a member variable to store the incoming message
							cmf = new CodeMemberField(typeof(Queue), "_msgs");
							cmf.InitExpression = new CodeObjectCreateExpression(typeof(Queue), new CodeExpression[] {});
							cmf.Attributes = MemberAttributes.Private;
							cmf.Comments.Add(new CodeCommentStatement("<summary>", true));
							cmf.Comments.Add(new CodeCommentStatement("this variable will contain any message generated by the Disassemble method", true));
							cmf.Comments.Add(new CodeCommentStatement("</summary>", true));
							clsDecleration.Members.Add(cmf);

                            #region implement IDisassemblerComponent

                            crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IDisassemblerComponent members");

                            #region IDisassemblerComponent.GetNext
                            // implement the IDisassemblerComponent.GetNext member
							clsMethod = new CodeMemberMethod();

                            // add #region directive
                            clsMethod.StartDirectives.Add(crDirective);

							clsMethod.Name = "GetNext";
							clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
							clsMethod.ReturnType = new CodeTypeReference(typeof(IBaseMessage));

							clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("called by the messaging engine until returned null, after disassemble has been called", true));
							clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">the pipeline context</param>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<returns>an IBaseMessage instance representing the message created</returns>", true));

							clsMethod.Statements.Add(new CodeCommentStatement("get the next message from the Queue and return it"));

							// IBaseMessage msg = null;
							clsMethod.Statements.Add(
								new CodeVariableDeclarationStatement(
									typeof(IBaseMessage), "msg", new CodePrimitiveExpression(null)));

							// if(_msgs.Count > 0)
							// {
							//		msg = (IBaseMessage) _msgs.Dequeue();
							// }
							clsMethod.Statements.Add(
								new CodeConditionStatement(
									new CodeBinaryOperatorExpression(
										new CodePropertyReferenceExpression(
											new CodeVariableReferenceExpression("_msgs"), "Count"),
										CodeBinaryOperatorType.GreaterThan,
										new CodePrimitiveExpression(0)),
									new CodeAssignStatement(
										new CodeVariableReferenceExpression("msg"),
										new CodeCastExpression(
											typeof(IBaseMessage),
									new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("_msgs"), "Dequeue", new CodeExpression[] {})))));

							// return msg;
							clsMethod.Statements.Add(
								new CodeMethodReturnStatement(
									new CodeVariableReferenceExpression(
										"msg")));

							clsMethod.Attributes = MemberAttributes.Public;
							clsMethod.Attributes |= MemberAttributes.Final;
							clsMethod.ImplementationTypes.Add(typeof(IDisassemblerComponent));
							clsDecleration.Members.Add(clsMethod);
							#endregion

							#region IDisassemblerComponent.Disassemble
							// implement the IDisassemblerComponent.Disassemble member
							clsMethod = new CodeMemberMethod();
							clsMethod.Name = "Disassemble";
							clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
							clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IBaseMessage), "inmsg"));
							clsMethod.ReturnType = new CodeTypeReference(typeof(void));
					
							clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("called by the messaging engine when a new message arrives", true));
							clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">the pipeline context</param>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"inmsg\">the actual message</param>", true));

							clsMethod.Statements.Add(new CodeCommentStatement(""));
							clsMethod.Statements.Add(new CodeCommentStatement("TODO: implement message retrieval logic"));
							clsMethod.Statements.Add(new CodeCommentStatement(""));

							// _msgs.Enqueue(inmsg)
							clsMethod.Statements.Add(
								new CodeMethodInvokeExpression(
									new CodeVariableReferenceExpression("_msgs"), "Enqueue", new CodeArgumentReferenceExpression("inmsg")));
									
							clsMethod.Attributes = MemberAttributes.Public;
							clsMethod.Attributes |= MemberAttributes.Final;
							clsMethod.ImplementationTypes.Add(typeof(IDisassemblerComponent));

                            // add #endregion directive
                            clsMethod.EndDirectives.Add(crEndDirective);

							clsDecleration.Members.Add(clsMethod);
							#endregion

                            #endregion

                            // does the user wants a stub for IProbeMessage?
							if(implementsIProbeMessage)
                            {
                                #region IProbeMessage implementation

                                crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IProbeMessage members");

                                #region IProbeMessage.Probe
                                clsDecleration.BaseTypes.Add(typeof(IProbeMessage));

								clsMethod = new CodeMemberMethod();

                                // add #region directive
                                clsMethod.StartDirectives.Add(crDirective);

								clsMethod.Name = "Probe";
								clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
								clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IBaseMessage), "inmsg"));
								clsMethod.ReturnType = new CodeTypeReference(typeof(bool));

								clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
								clsMethod.Comments.Add(new CodeCommentStatement("called by the messaging engine when a new message arrives", true));
								clsMethod.Comments.Add(new CodeCommentStatement("checks if the incoming message is in a recognizable format", true));
								clsMethod.Comments.Add(new CodeCommentStatement("if the message is in a recognizable format, only this component", true));
								clsMethod.Comments.Add(new CodeCommentStatement("within this stage will be execute (FirstMatch equals true)", true));
								clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
								clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">the pipeline context</param>", true));
								clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"inmsg\">the actual message</param>", true));

								clsMethod.Statements.Add(new CodeCommentStatement(""));
								clsMethod.Statements.Add(new CodeCommentStatement("TODO: check whether you're interested in the given message"));
								clsMethod.Statements.Add(new CodeCommentStatement(""));

								// return true;
								clsMethod.Statements.Add(
									new CodeMethodReturnStatement(
									new CodePrimitiveExpression(true)));

								clsMethod.Attributes = MemberAttributes.Public;
								clsMethod.Attributes |= MemberAttributes.Final;
								clsMethod.ImplementationTypes.Add(typeof(IProbeMessage));

                                // add #endregion directive
                                clsMethod.EndDirectives.Add(crEndDirective);

								clsDecleration.Members.Add(clsMethod);
								
                                #endregion

                                #endregion
                            }

							break;
                        #endregion

                        default:
							clsDecleration.BaseTypes.Add(typeof(IComponent));

                            #region IComponent implementation

                            crDirective = new CodeRegionDirective(CodeRegionMode.Start, "IComponent members");

                            #region IComponent.Execute
                            // implement IComponent.Execute member
							clsMethod = new CodeMemberMethod();

                            // add #region directive
                            clsMethod.StartDirectives.Add(crDirective);

							clsMethod.Name = "Execute";
							clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IPipelineContext), "pc"));
							clsMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IBaseMessage), "inmsg"));
							clsMethod.ReturnType = new CodeTypeReference(typeof(IBaseMessage));
					
							clsMethod.Comments.Add(new CodeCommentStatement("<summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("Implements IComponent.Execute method.", true));
							clsMethod.Comments.Add(new CodeCommentStatement("</summary>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"pc\">Pipeline context</param>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<param name=\"inmsg\">Input message</param>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<returns>Original input message</returns>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("<remarks>", true));
							clsMethod.Comments.Add(new CodeCommentStatement("IComponent.Execute method is used to initiate", true));
							clsMethod.Comments.Add(new CodeCommentStatement("the processing of the message in this pipeline component.", true));
							clsMethod.Comments.Add(new CodeCommentStatement("</remarks>", true));

							clsMethod.Statements.Add(new CodeCommentStatement(""));
							clsMethod.Statements.Add(new CodeCommentStatement("TODO: implement component logic"));
							clsMethod.Statements.Add(new CodeCommentStatement(""));

							clsMethod.Statements.Add(new CodeCommentStatement("this way, it's a passthrough pipeline component"));
							clsMethod.Statements.Add(
								new CodeMethodReturnStatement(
								new CodeArgumentReferenceExpression("inmsg")));

							clsMethod.Attributes = MemberAttributes.Public;
							clsMethod.Attributes |= MemberAttributes.Final;
							clsMethod.ImplementationTypes.Add(typeof(IComponent));

                            // add #endregion directive
                            clsMethod.EndDirectives.Add(crEndDirective);

							clsDecleration.Members.Add(clsMethod);
							#endregion

                            #endregion

                            break;
					}

					// add the interfaces we've implemented
					foreach(string baseType in baseTypes)
					{
						clsDecleration.BaseTypes.Add(baseType);
					}

					// we're generating a public class
					clsDecleration.IsClass = true;
					clsDecleration.TypeAttributes = TypeAttributes.Public;

					// and add it to our namespace
					cnsCodeNamespace.Types.Add(clsDecleration);

					// tell the code generator to generate our sourcecode
					codeGenerator.GenerateCodeFromNamespace(cnsCodeNamespace, sw, cgo);
				}
			}

            // we're done, unless the user chose to implement the code in VB.NET...
            if (language == implementationLanguages.VBNet)
            {
                // remove unneeded Inherits ... code, move the ... to the Implements line
                // as we have no base class we want to use
                postFixVbCode(fileName);
            }
		}
		#endregion

        #region postFixVbCode - does a postfix to move the "Inherits <classname>" to the interfaces
        /// <summary>
        /// this method alters the generated sourcecode (VB.NET) in order to remove the Inerits &lt;classname&gt;
        /// and move the class to the Implements row (the VB.NET) <see cref="C:ICodeGenerator"/> copies the first
        /// defined BaseType to the Inherits line, while we need it to be on the Implements line, as we don't
        /// inherit from any classes
        /// </summary>
        /// <param name="fileName">the file to modify</param>
        private static void postFixVbCode(string fileName)
		{
            // this will hold the entire sourcecode file contents
            StringBuilder buffer = new StringBuilder();

            #region copy the content of the file into "buffer"
            // read the specified file
            using (StreamReader sr = new StreamReader(fileName))
            {
                // just a simple buffer
                string tmp = null;

                // read until we have no more input
                while ((tmp = sr.ReadLine()) != null)
                {
                    // and append an exact copy to our buffer instance
                    buffer.AppendLine(tmp);
                }
            }
            #endregion

            #region Replace the incorrect definition
            Regex replacer = new Regex(@"(?im)inherits\s+(?<inherits>.*)\r\n*[\t|\s]*(?<implements>implements\s+.*)\r|(?<implements>implements\s+.*)\r\n[\t|\s]*inherits\s+(?<inherits>.*)");

            MatchCollection matches = replacer.Matches(buffer.ToString());
            string interfaces = null, baseclasses = null;

            // get the implements / inherits groups
            foreach (Match match in matches)
            {
                if (interfaces == null && match.Groups["implements"].Value.Length > 0)
                {
                    interfaces = match.Groups["implements"].Value;
                }
                if (baseclasses == null && match.Groups["inherits"].Value.Length > 0)
                {
                    baseclasses = match.Groups["inherits"].Value;
                }
            }

            string replacement = null;

            foreach (Match match in matches)
            {
                if (match.Groups["implements"].Value.Length > 0)
                {
                    replacement = replacer.Replace(buffer.ToString(), string.Format("{0}, {1}", interfaces, baseclasses));
                }
            }
            #endregion
             
            // "replacement" now contains the correct inheritance
            // write out the correct definition
            using (StreamWriter sw = new StreamWriter(fileName))
			{
				sw.Write(replacement.ToString());
			}
		}
		#endregion
	}
}