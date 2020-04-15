using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Broadcaster.Interfaces.Enums;

namespace Broadcaster.Interfaces.Helpers
{
    public static class EnumExtensions
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, DictionaryItem>> s_cacheDictionary = new ConcurrentDictionary<Type, Dictionary<string, DictionaryItem>>();
        public static string GetDescription(this Enum value)
        {
            return ((DescriptionAttribute[])value.GetType().GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false))[0].Description;
        }

        public static T GetEnumByDescription<T>(string description) where T : struct, IConvertible
        {
            var currentType = typeof(T);
            if (!currentType.IsEnum) throw new ArgumentException("T must be an enumerated type");
            DictionaryItem localItem;
            if (s_cacheDictionary.ContainsKey(currentType))
            {
                Dictionary<string, DictionaryItem> items;
                s_cacheDictionary.TryGetValue(currentType, out items);
                items.TryGetValue(description, out localItem);
            }
            else
            {
                var dictionaryOfDescription = Enum.GetValues(typeof(T)).Cast<T>().Select(en =>
                {
                    var localenum = en as Enum;
                    return new DictionaryItem { Enum = localenum, Description = localenum.GetDescription() };
                }).ToDictionary(arg => arg.Description);

                dictionaryOfDescription.TryGetValue(description, out localItem);
                s_cacheDictionary.TryAdd(currentType, dictionaryOfDescription);
            }
            if (localItem == null) throw new NotSupportedException($"can't find enum with description: {description} in {typeof(T)}");
            return (T)(dynamic)localItem.Enum;
        }

        private class DictionaryItem
        {
            public Enum Enum { get; set; }
            public string Description { get; set; }
        }
    }
}
