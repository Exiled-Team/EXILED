using System.Collections.Generic;

namespace EXILED.Extensions
{
	public static class Config
	{
		public static Dictionary<string, string> GetStringDictionary(this YamlConfig config, string key, Dictionary<string, string> defaultValue)
		{
			var dictionary = config.GetStringDictionary(key);

			if (dictionary?.Count == 0) return defaultValue;

			return dictionary;
		}

		public static List<string> GetStringList(this YamlConfig config, string key, List<string> defaultValue)
		{
			var list = config.GetStringList(key);

			if (list?.Count == 0) return defaultValue;

			return list;
		}
	}
}
