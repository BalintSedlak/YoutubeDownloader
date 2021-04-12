using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace YoutubeDownloader.Models
{
    public static class EnumExtension
    {
        public static string GetDescription<T>(this Enum en) where T : Enum
        {
            FieldInfo fieldInfo = en.GetType().GetField(en.ToString());

            DescriptionAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return en.ToString();
        }

        public static IEnumerable<string> GetAllDescription<T>(this Enum en) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Select((e) => e.GetDescription<T>()).ToList();
        }

        /// <summary>
        /// Based on: https://stackoverflow.com/questions/4367723/get-enum-from-description-attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetValueFromDescription<T>(this Enum en, string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }
    }
}
