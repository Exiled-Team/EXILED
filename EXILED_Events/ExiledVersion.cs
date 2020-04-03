using System;

namespace EXILED
{
	public class ExiledVersion
	{
		public int Major;
		public int Minor;
		public int Patch;

		public override string ToString() => $"{Major}.{Minor}.{Patch}";

		public override int GetHashCode()
		{
			int hashCode = -639545495;

			hashCode = hashCode * -1521134295 + Major.GetHashCode();
			hashCode = hashCode * -1521134295 + Minor.GetHashCode();
			hashCode = hashCode * -1521134295 + Patch.GetHashCode();

			return hashCode;
		}

		public override bool Equals(object obj) => this == obj as ExiledVersion;

		public static bool operator ==(ExiledVersion left, ExiledVersion right) => Version.Parse(left.ToString()) == Version.Parse(right.ToString());

		public static bool operator !=(ExiledVersion left, ExiledVersion right) => !(left == right);

		public static bool operator >(ExiledVersion left, ExiledVersion right) => Version.Parse(left.ToString()) > Version.Parse(right.ToString());

		public static bool operator <(ExiledVersion left, ExiledVersion right) => Version.Parse(left.ToString()) < Version.Parse(right.ToString());

		public static bool operator >=(ExiledVersion left, ExiledVersion right) => Version.Parse(left.ToString()) >= Version.Parse(right.ToString());

		public static bool operator <=(ExiledVersion left, ExiledVersion right) => Version.Parse(left.ToString()) <= Version.Parse(right.ToString());
	}
}