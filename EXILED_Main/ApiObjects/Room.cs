using System.Collections.Generic;
using System.Linq;
using EXILED.Extensions;
using UnityEngine;

namespace EXILED.ApiObjects
{
	public class Room
	{
		private ZoneType zone = ZoneType.Unspecified;
		public string Name { get; set; }
		public Transform Transform { get; set; }
		public Vector3 Position { get; set; }

		public ZoneType Zone
		{
			get
			{
				if (zone != ZoneType.Unspecified)
					return zone;

				zone = ZoneType.Unspecified;

				if (Position.y == -1997f)
					zone = ZoneType.Unspecified;
				else if (Position.y >= 0f && Position.y < 500f)
					zone = ZoneType.LightContainment;
				else if (Position.y < -100 && Position.y > -1000f)
					zone = ZoneType.HeavyContainment;
				else if (Name.Contains("ENT") || Name.Contains("INTERCOM"))
					zone = ZoneType.Entrance;
				else if (Position.y >= 5)
					zone = ZoneType.Surface;

				return zone;
			}
		}
	}
}