#region License
// Copyright (c) Newtonsoft. All Rights Reserved.
// License: https://raw.github.com/JamesNK/Newtonsoft.Json.Schema/master/LICENSE.md
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Newtonsoft.Json.Schema.Infrastructure
{
    internal static class AttributeHelpers
    {
        private static ReflectionObject? _dataTypeReflectionObject;
        private static ReflectionObject? _regexReflectionObject;
        private static ReflectionObject? _maxLengthReflectionObject;
        private static ReflectionObject? _minLengthReflectionObject;
        private static ReflectionObject? _enumTypeReflectionObject;
        private static ReflectionObject? _stringLengthReflectionObject;
        private static ReflectionObject? _rangeReflectionObject;
        private static ReflectionObject? _displayReflectionObject;
        private static ReflectionObject? _displayNameReflectionObject;
        private static ReflectionObject? _descriptionReflectionObject;

        private const string DisplayNameAttributeName = "System.ComponentModel.DisplayNameAttribute";
        private const string DescriptionAttributeName = "System.ComponentModel.DescriptionAttribute";
        private const string DisplayAttributeName = "System.ComponentModel.DataAnnotations.DisplayAttribute";
        private const string RequiredAttributeName = "System.ComponentModel.DataAnnotations.RequiredAttribute";
        private const string MinLengthAttributeName = "System.ComponentModel.DataAnnotations.MinLengthAttribute";
        private const string MaxLengthAttributeName = "System.ComponentModel.DataAnnotations.MaxLengthAttribute";
        private const string DataTypeAttributeName = "System.ComponentModel.DataAnnotations.DataTypeAttribute";
        private const string RegularExpressionAttributeName = "System.ComponentModel.DataAnnotations.RegularExpressionAttribute";
        private const string RangeAttributeName = "System.ComponentModel.DataAnnotations.RangeAttribute";
        private const string UrlAttributeName = "System.ComponentModel.DataAnnotations.UrlAttribute";
        private const string PhoneAttributeName = "System.ComponentModel.DataAnnotations.PhoneAttribute";
        private const string EmailAddressAttributeName = "System.ComponentModel.DataAnnotations.EmailAddressAttribute";
        private const string StringLengthAttributeName = "System.ComponentModel.DataAnnotations.StringLengthAttribute";
        private const string EnumDataTypeAttributeName = "System.ComponentModel.DataAnnotations.EnumDataTypeAttribute";

        private static bool GetDisplay(Type type, JsonProperty? memberProperty, out string? name, out string? description)
        {
            if (TryGetAttributeByNameFromTypeOrProperty(type, memberProperty, DisplayAttributeName, out Attribute? displayAttribute, out Type? matchingType))
            {
                if (_displayReflectionObject == null)
                {
                    _displayReflectionObject = ReflectionObject.Create(matchingType, "GetName", "GetDescription");
                }
                name = (string?)_displayReflectionObject.GetValue(displayAttribute, "GetName");
                description = (string?)_displayReflectionObject.GetValue(displayAttribute, "GetDescription");
                return true;
            }

            name = null;
            description = null;
            return false;
        }

        public static bool GetDisplayName(Type type, JsonProperty? memberProperty, out string? displayName)
        {
            if (GetDisplay(type, memberProperty, out displayName, out _) && !string.IsNullOrEmpty(displayName))
            {
                return true;
            }

            if (TryGetAttributeByNameFromTypeOrProperty(type, memberProperty, DisplayNameAttributeName, out Attribute? displayNameAttribute, out Type? matchingType))
            {
                if (_displayNameReflectionObject == null)
                {
                    _displayNameReflectionObject = ReflectionObject.Create(matchingType, "DisplayName");
                }
                displayName = (string?)_displayNameReflectionObject.GetValue(displayNameAttribute, "DisplayName");
                return true;
            }

            displayName = null;
            return false;
        }

        public static bool GetDescription(Type type, JsonProperty? memberProperty, out string? description)
        {
            if (GetDisplay(type, memberProperty, out _, out description) && !string.IsNullOrEmpty(description))
            {
                return true;
            }

            if (TryGetAttributeByNameFromTypeOrProperty(type, memberProperty, DescriptionAttributeName, out Attribute? descriptionAttribute, out Type? matchingType))
            {
                if (_descriptionReflectionObject == null)
                {
                    _descriptionReflectionObject = ReflectionObject.Create(matchingType, "Description");
                }
                description = (string?)_descriptionReflectionObject.GetValue(descriptionAttribute, "Description");
                return true;
            }

            description = null;
            return false;
        }

        public static bool GetRange(JsonProperty? property, out double minimum, out double maximum)
        {
            if (property != null)
            {
                if (TryGetAttributeByName(property, RangeAttributeName, out Attribute? rangeAttribute, out Type? matchingType))
                {
                    if (_rangeReflectionObject == null)
                    {
                        _rangeReflectionObject = ReflectionObject.Create(matchingType, "Minimum", "Maximum");
                    }
                    minimum = Convert.ToDouble(_rangeReflectionObject.GetValue(rangeAttribute, "Minimum"), CultureInfo.InvariantCulture);
                    maximum = Convert.ToDouble(_rangeReflectionObject.GetValue(rangeAttribute, "Maximum"), CultureInfo.InvariantCulture);
                    return true;
                }
            }

            minimum = 0;
            maximum = 0;
            return false;
        }

        public static bool GetStringLength(JsonProperty? property, out int minimumLength, out int maximumLength)
        {
            if (property != null)
            {
                if (TryGetAttributeByName(property, StringLengthAttributeName, out Attribute? attribute, out Type? matchingType))
                {
                    if (_stringLengthReflectionObject == null)
                    {
                        _stringLengthReflectionObject = ReflectionObject.Create(
                            matchingType,
#if !NET35
                            "MinimumLength",
#endif
                            "MaximumLength");
                    }

#if !NET35
                    minimumLength = (int)_stringLengthReflectionObject.GetValue(attribute, "MinimumLength")!;
#else
                    minimumLength = 0;
#endif
                    maximumLength = (int)_stringLengthReflectionObject.GetValue(attribute, "MaximumLength")!;
                    return true;
                }
            }

            minimumLength = 0;
            maximumLength = 0;
            return false;
        }

        public static Type? GetEnumDataType(JsonProperty? property)
        {
            if (property != null)
            {
                if (TryGetAttributeByName(property, EnumDataTypeAttributeName, out Attribute? attribute, out Type? matchingType))
                {
                    if (_enumTypeReflectionObject == null)
                    {
                        _enumTypeReflectionObject = ReflectionObject.Create(matchingType, "EnumType");
                    }
                    return (Type?)_enumTypeReflectionObject.GetValue(attribute, "EnumType");
                }
            }

            return null;
        }

        public static int? GetMinLength(JsonProperty? property)
        {
            if (property != null)
            {
                if (TryGetAttributeByName(property, MinLengthAttributeName, out Attribute? minLengthAttribute, out Type? matchingType))
                {
                    if (_minLengthReflectionObject == null)
                    {
                        _minLengthReflectionObject = ReflectionObject.Create(matchingType, "Length");
                    }
                    return (int)_minLengthReflectionObject.GetValue(minLengthAttribute, "Length")!;
                }
            }

            return null;
        }

        public static int? GetMaxLength(JsonProperty? property)
        {
            if (property != null)
            {
                if (TryGetAttributeByName(property, MaxLengthAttributeName, out Attribute? maxLengthAttribute, out Type? matchingType))
                {
                    if (_maxLengthReflectionObject == null)
                    {
                        _maxLengthReflectionObject = ReflectionObject.Create(matchingType, "Length");
                    }
                    return (int)_maxLengthReflectionObject.GetValue(maxLengthAttribute, "Length")!;
                }
            }

            return null;
        }

        public static bool GetRequired(JsonProperty? property)
        {
            if (property != null)
            {
                return TryGetAttributeByName(property, RequiredAttributeName, out _, out _);
            }

            return false;
        }

        public static string? GetPattern(JsonProperty? property)
        {
            if (property != null)
            {
                if (TryGetAttributeByName(property, RegularExpressionAttributeName, out Attribute? regexAttribute, out Type? matchingType))
                {
                    if (_regexReflectionObject == null)
                    {
                        _regexReflectionObject = ReflectionObject.Create(matchingType, "Pattern");
                    }
                    return (string?)_regexReflectionObject.GetValue(regexAttribute, "Pattern");
                }
            }

            return null;
        }

        public static string? GetFormat(JsonProperty? property)
        {
            if (property != null)
            {
                if (TryGetAttributeByName(property, UrlAttributeName, out _, out _))
                {
                    return Constants.Formats.Uri;
                }

                if (TryGetAttributeByName(property, PhoneAttributeName, out _, out _))
                {
                    return Constants.Formats.Phone;
                }

                if (TryGetAttributeByName(property, EmailAddressAttributeName, out _, out _))
                {
                    return Constants.Formats.Email;
                }

                if (TryGetAttributeByName(property, DataTypeAttributeName, out Attribute? dataTypeAttribute, out Type? matchingType))
                {
                    if (_dataTypeReflectionObject == null)
                    {
                        _dataTypeReflectionObject = ReflectionObject.Create(matchingType, "DataType");
                    }
                    string s = _dataTypeReflectionObject.GetValue(dataTypeAttribute, "DataType")!.ToString();
                    switch (s)
                    {
                        case "Url":
                            return Constants.Formats.Uri;
                        case "Date":
                            return Constants.Formats.Date;
                        case "Time":
                            return Constants.Formats.Time;
                        case "DateTime":
                            return Constants.Formats.DateTime;
                        case "EmailAddress":
                            return Constants.Formats.Email;
                        case "PhoneNumber":
                            return Constants.Formats.Phone;
                    }
                }
            }

            return null;
        }

        private static bool TryGetAttributeByName(JsonProperty property, string name, [NotNullWhen(true)] out Attribute? attribute, [NotNullWhen(true)] out Type? matchingType)
        {
            return TryGetAttributeByName(property.AttributeProvider, name, out attribute, out matchingType);
        }

        private static bool TryGetAttributeByName(IAttributeProvider? attributeProvider, string name, [NotNullWhen(true)] out Attribute? attribute, [NotNullWhen(true)] out Type? matchingType)
        {
            if (attributeProvider != null)
            {
                IList<Attribute> attributes = attributeProvider.GetAttributes(true);
                foreach (Attribute a in attributes)
                {
                    if (IsMatchingAttribute(a.GetType(), name, out matchingType))
                    {
                        attribute = a;
                        return true;
                    }
                }
            }

            attribute = null;
            matchingType = null;
            return false;
        }

        private static bool IsMatchingAttribute(Type attributeType, string name, [NotNullWhen(true)] out Type? matchingType)
        {
            // check that attribute or its base class matches the name
            // e.g. attribute might inherit from DescriptionAttribute
            Type currentType = attributeType;
            do
            {
                if (string.Equals(currentType.FullName, name, StringComparison.Ordinal))
                {
                    matchingType = currentType;
                    return true;
                }
            } while ((currentType = currentType.BaseType()) != null);

            matchingType = null;
            return false;
        }

        private static bool TryGetAttributeByNameFromTypeOrProperty(Type type, JsonProperty? memberProperty, string name, [NotNullWhen(true)] out Attribute? attribute, [NotNullWhen(true)] out Type? matchingType)
        {
            // check for property attribute first
            if (memberProperty != null && TryGetAttributeByName(memberProperty.AttributeProvider, name, out attribute, out matchingType))
            {
                return true;
            }

            // fall back to type attribute
            return TryGetAttributeByName(new ReflectionAttributeProvider(type), name, out attribute, out matchingType);
        }
    }
}