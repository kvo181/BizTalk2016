using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace bizilante.SSO.Helper
{
	internal class VariantHelper
	{
		private static Hashtable typecode2variantTable;

		private static Hashtable variant2typecodeTable;

		static VariantHelper()
		{
			VariantHelper.typecode2variantTable = new Hashtable();
			VariantHelper.typecode2variantTable.Add(TypeCode.Empty, VarEnum.VT_EMPTY);
			VariantHelper.typecode2variantTable.Add(TypeCode.Object, VarEnum.VT_UNKNOWN);
			VariantHelper.typecode2variantTable.Add(TypeCode.DBNull, VarEnum.VT_NULL);
			VariantHelper.typecode2variantTable.Add(TypeCode.Boolean, VarEnum.VT_BOOL);
			VariantHelper.typecode2variantTable.Add(TypeCode.Char, VarEnum.VT_UI2);
			VariantHelper.typecode2variantTable.Add(TypeCode.SByte, VarEnum.VT_I1);
			VariantHelper.typecode2variantTable.Add(TypeCode.Byte, VarEnum.VT_UI1);
			VariantHelper.typecode2variantTable.Add(TypeCode.Int16, VarEnum.VT_I2);
			VariantHelper.typecode2variantTable.Add(TypeCode.UInt16, VarEnum.VT_UI2);
			VariantHelper.typecode2variantTable.Add(TypeCode.Int32, VarEnum.VT_I4);
			VariantHelper.typecode2variantTable.Add(TypeCode.UInt32, VarEnum.VT_UI4);
			VariantHelper.typecode2variantTable.Add(TypeCode.Int64, VarEnum.VT_I8);
			VariantHelper.typecode2variantTable.Add(TypeCode.UInt64, VarEnum.VT_UI8);
			VariantHelper.typecode2variantTable.Add(TypeCode.Single, VarEnum.VT_R4);
			VariantHelper.typecode2variantTable.Add(TypeCode.Double, VarEnum.VT_R8);
			VariantHelper.typecode2variantTable.Add(TypeCode.Decimal, VarEnum.VT_DECIMAL);
			VariantHelper.typecode2variantTable.Add(TypeCode.DateTime, VarEnum.VT_DATE);
			VariantHelper.typecode2variantTable.Add(TypeCode.String, VarEnum.VT_BSTR);
			VariantHelper.variant2typecodeTable = new Hashtable();
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_EMPTY, TypeCode.Empty);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_UNKNOWN, TypeCode.Object);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_NULL, TypeCode.DBNull);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_BOOL, TypeCode.Boolean);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_I1, TypeCode.SByte);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_UI1, TypeCode.Byte);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_I2, TypeCode.Int16);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_UI2, TypeCode.UInt16);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_I4, TypeCode.Int32);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_UI4, TypeCode.UInt32);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_I8, TypeCode.Int64);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_UI8, TypeCode.UInt64);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_R4, TypeCode.Single);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_R8, TypeCode.Double);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_DECIMAL, TypeCode.Decimal);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_DATE, TypeCode.DateTime);
			VariantHelper.variant2typecodeTable.Add(VarEnum.VT_BSTR, TypeCode.String);
		}

		internal static VarEnum Lookup(TypeCode typecode)
		{
			return (VarEnum)VariantHelper.typecode2variantTable[typecode];
		}

		internal static TypeCode Lookup(VarEnum varenum)
		{
			return (TypeCode)VariantHelper.variant2typecodeTable[varenum];
		}

		internal static object FromString(string stringval, VarEnum vt_type)
		{
			object result;
			if (vt_type == VarEnum.VT_BOOL)
			{
				result = Convert.ToBoolean(Convert.ToInt32(stringval, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
			}
			else if (vt_type == VarEnum.VT_NULL)
			{
				result = DBNull.Value;
			}
			else
			{
				result = Convert.ChangeType(stringval, VariantHelper.Lookup(vt_type), CultureInfo.InvariantCulture);
			}
			return result;
		}

		internal static string ToString(object obj, VarEnum vt_type)
		{
			string result;
			if (vt_type == VarEnum.VT_BOOL)
			{
				if ((bool)obj)
				{
					result = "-1";
				}
				else
				{
					result = "0";
				}
			}
			else if (vt_type == VarEnum.VT_NULL)
			{
				result = "";
			}
			else
			{
				result = Convert.ToString(obj, CultureInfo.InvariantCulture);
			}
			return result;
		}
	}
}
