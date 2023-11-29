namespace WremiaTrade.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.ComponentModel.DataAnnotations;

    using WremiaTrade.Utilities.Attributes;
    using WremiaTrade.Utilities.Enums;
    using WremiaTrade.Utilities.Models;
    using System.ComponentModel;

    public static class EnumHelper
    {
        /// <summary>
        /// Get enum from value
        /// </summary>
        public static T GetEnum<T>(object value)
        {
            if (typeof(T).IsEnumDefined(value))
            {
                return (T)value;
            }

            throw new ArgumentException("Unsupported Enum Type.");
        }

        /// <summary>
        /// Checks and converts the string name of enum value to enum.
        /// Ex: Currency currency = ParseEnum<Currency>("TRY")
        /// </summary>
        public static bool TryParseEnum<T>(string value, out T result)
        {
            try
            {
                result = (T)Enum.Parse(typeof(T), value, true);

                return true;
            }
            catch
            {
                result = default;

                return false;
            }
        }

        /// <summary>
        /// Converts the string name of enum value to enum.
        /// Ex: Currency currency = ParseEnum<Currency>("TRY")
        /// </summary>
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Returns the string specified on the enum value by using the [Display] attribute.
        /// </summary>
        public static string GetDisplayName(this Enum enumValue)
        {
            MemberInfo member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            var attribute = member?.GetCustomAttribute<DisplayAttribute>();

            if (member == null || attribute == null)
            {
                return enumValue.ToString();
            }

            return attribute.Name;
        }

        /// <summary>
        /// Returns the ShortName specified on the enum value by using the [Display] attribute.
        /// </summary>
        public static string GetDisplayShortName(this Enum enumValue)
        {
            MemberInfo member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            var attribute = member?.GetCustomAttribute<DisplayAttribute>();

            if (member == null || attribute == null)
            {
                return enumValue.ToString();
            }

            return attribute.ShortName;
        }

        /// <summary>
        /// Returns the Description specified on the enum value by using the [Display] attribute.
        /// </summary>
        public static string GetDisplayDescription(this Enum enumValue)
        {
            MemberInfo member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            var attribute = member?.GetCustomAttribute<DisplayAttribute>();

            if (member == null || attribute == null)
            {
                return enumValue.ToString();
            }

            return attribute.Description;
        }

        /// <summary>
        /// Get enum from name
        /// </summary>
        public static T GetEnumFromDisplayName<T>(string name)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, attributeType: typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.Name == name)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == name)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Unsupported Enum Type.");
        }

        /// <summary>
        /// Generates generic enum list by attribute
        /// See papara.api example /commercial/jobtypes
        /// See also Papara.Model.JobType and it's custom attribute
        /// </summary>
        public static IEnumerable<OrderableStringModel<T>> GetEnumListByAttribute<T, TEnum, TAttribute>(string filterName)
            where TEnum : Enum
            where T : struct
            where TAttribute : Attribute
        {
            return typeof(TEnum).GetFields(BindingFlags.Public |
                                             BindingFlags.Static)
                .Where(field => field.IsDefined(typeof(TAttribute), false))
                .Select(field => (TEnum)field.GetValue(null))
                .Select(enumItem =>
                    new OrderableStringModel<T>()
                    {
                        Id = (T)Convert.ChangeType(enumItem, typeof(T)),
                        DisplayName = enumItem.GetDisplayName(),
                        Order = enumItem.GetOrderValue()
                    }).ToList();
        }

        /// <summary>
        /// Filters enum by filter attribute
        /// See AuditLogOperationNames
        /// See 
        /// </summary>
        public static IEnumerable<OrderableStringModel<T>> GetFilteredEnumList<T, TEnum>(string filterName)
            where TEnum : Enum
            where T : struct
        {
            var response = new List<OrderableStringModel<T>>();

            var fieldInfoList = typeof(TEnum).GetFields(BindingFlags.Public |
                                                        BindingFlags.Static)
                .Where(field => field.IsDefined(typeof(EnumFilterAttribute), false));

            foreach (var fieldInfo in fieldInfoList)
            {
                var enumItem = (TEnum)fieldInfo.GetValue(null);

                var filterAttribute = fieldInfo
                    .GetCustomAttributes(typeof(EnumFilterAttribute), false)
                    .FirstOrDefault() as EnumFilterAttribute;

                if (filterAttribute.FilterName == filterName)
                {
                    response.Add(new OrderableStringModel<T>()
                    {
                        Id = (T)Convert.ChangeType(enumItem, typeof(T)),
                        DisplayName = enumItem.GetDisplayName(),
                        Order = enumItem.GetOrderValue()
                    });
                }
            }

            return response;
        }

        /// <summary>
        /// Generates enum list
        /// </summary>
        public static IEnumerable<TEnum> GetEnumList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        /// <summary>
        /// Returns the value specified on the enum value by using the [Order] attribute.
        /// </summary>
        public static int GetOrderValue(this Enum enumValue)
        {
            MemberInfo member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            var attribute = member?.GetCustomAttribute<OrderAttribute>();

            if (member == null || attribute == null)
            {
                return 0;
            }

            return attribute.Order;
        }

        /// <summary>
        /// Calculate binary value with enum value
        /// </summary>
        public static int ToPow(this Enum enumValue)
        {
            var typeCode = (double)enumValue.GetHashCode();

            return ((int)Math.Pow(2, typeCode));
        }

        /// <summary>
        /// Get Attribute Values from Enum.
        /// </summary>
        public static T GetAttribute<T>(Enum enumValue) where T : Attribute
        {
            T attribute;

            MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                attribute = (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();

                return attribute;
            }

            throw new ArgumentNullException("Attribute can not be null.");
        }

        /// <summary>
        /// Get Enum Description.
        /// </summary>
        public static string GetEnumDescription<T>(this T value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.
                GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            throw new ArgumentNullException("Description attribute can not be null.");
        }

        /// <summary>
        /// Returns the value specified on the enum value by using the [CustomMessage] attribute.
        /// </summary>
        public static string GetCustomMessage(this Enum enumValue)
        {
            MemberInfo member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            var attribute = member?.GetCustomAttribute<CustomDisplayErrorAttribute>();

            if (member == null || attribute == null)
            {
                return enumValue.ToString(); ;
            }

            return attribute.ErrorMessage;
        }

        /// <summary>
        /// Returns the value specified on the enum value by using the [CustomErrorCode] attribute.
        /// </summary>
        public static int GetCustomErrorCode(this Enum enumValue)
        {
            MemberInfo member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            var attribute = member?.GetCustomAttribute<CustomDisplayErrorAttribute>();

            if (member == null || attribute == null)
            {
                return 0;
            }

            return attribute.ErrorCode;
        }

        /// <summary>
        /// Checks the enum value requires additional description by using the [DescriptionRequired] attribute.
        /// </summary>
        public static bool DescriptionRequired(this Enum enumValue)
        {
            MemberInfo member = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            var attribute = member?.GetCustomAttribute<DescriptionRequiredAttribute>();

            return member != null && attribute != null;
        }
    }
}