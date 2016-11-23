namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Xml;

    public class PromotingMap
    {
        private static Hashtable propertyMapping = null;
        private static object syncRoot = new object();

        private static object ChangeTypeToBoolean(string value)
        {
            return XmlConvert.ToBoolean(value);
        }

        private static object ChangeTypeToByte(string value)
        {
            return XmlConvert.ToByte(value);
        }

        private static object ChangeTypeToDateTime(string value)
        {
            return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified).ToUniversalTime();
        }

        private static object ChangeTypeToDecimal(string value)
        {
            return XmlConvert.ToDecimal(value);
        }

        private static object ChangeTypeToDouble(string value)
        {
            return XmlConvert.ToDouble(value);
        }

        private static object ChangeTypeToInt16(string value)
        {
            return XmlConvert.ToInt16(value);
        }

        private static object ChangeTypeToInt32(string value)
        {
            return XmlConvert.ToInt32(value);
        }

        private static object ChangeTypeToSByte(string value)
        {
            return XmlConvert.ToSByte(value);
        }

        private static object ChangeTypeToSingle(string value)
        {
            return XmlConvert.ToSingle(value);
        }

        private static object ChangeTypeToString(string value)
        {
            return value;
        }

        private static object ChangeTypeToUInt16(string value)
        {
            return XmlConvert.ToUInt16(value);
        }

        private static object ChangeTypeToUInt32(string value)
        {
            return XmlConvert.ToUInt32(value);
        }

        private static void InitializeMapping()
        {
            if (propertyMapping == null)
            {
                lock (syncRoot)
                {
                    propertyMapping = new Hashtable();
                    propertyMapping["anyURI"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["boolean"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToBoolean);
                    propertyMapping["byte"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToSByte);
                    propertyMapping["date"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDateTime);
                    propertyMapping["dateTime"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDateTime);
                    propertyMapping["decimal"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDecimal);
                    propertyMapping["double"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDouble);
                    propertyMapping["ENTITY"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["float"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToSingle);
                    propertyMapping["gDay"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDateTime);
                    propertyMapping["gMonth"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDateTime);
                    propertyMapping["gMonthDay"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDateTime);
                    propertyMapping["gYear"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDateTime);
                    propertyMapping["gYearMonth"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDateTime);
                    propertyMapping["ID"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["IDREF"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["int"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToInt32);
                    propertyMapping["integer"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDecimal);
                    propertyMapping["language"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["Name"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["NCName"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["negativeInteger"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDecimal);
                    propertyMapping["NMTOKEN"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["nonNegativeInteger"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDecimal);
                    propertyMapping["nonPositiveInteger"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDecimal);
                    propertyMapping["normalizedString"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["NOTATION"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["positiveInteger"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDecimal);
                    propertyMapping["QName"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["short"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToInt16);
                    propertyMapping["string"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["time"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToDateTime);
                    propertyMapping["token"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToString);
                    propertyMapping["unsignedByte"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToByte);
                    propertyMapping["unsignedInt"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToUInt32);
                    propertyMapping["unsignedShort"] = new ChangeTypeDelegate(PromotingMap.ChangeTypeToUInt16);
                }
            }
        }

        public object MapValue(string value, string XSDType)
        {
            InitializeMapping();
            ChangeTypeDelegate delegate2 = (ChangeTypeDelegate) propertyMapping[XSDType];
            if (delegate2 == null)
            {
                throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "XSD data type {0} is not supported for property promotion", new object[] { XSDType }));
            }
            return delegate2(value);
        }
    }
}

