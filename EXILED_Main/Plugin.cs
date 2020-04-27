using EXILED.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EXILED
{
	public abstract class Plugin
	{
		public static YamlConfig Config;
		public abstract string GetName { get; }
		public abstract string ConfigPrefix { get; }
		public abstract void OnEnable();
		public abstract void OnDisable();
		public abstract void OnReload();
		public abstract void ReloadConfig();
	}
}