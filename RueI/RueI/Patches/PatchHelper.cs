namespace RueI.Patches;

using System.Reflection;

/// <summary>
/// Provides helpful functions for working with patches.
/// </summary>
public class PatchHelper
{
    /// <summary>
    /// Finds a matching method using the provided <see cref="Delegate"/> and name in a type.
    /// </summary>
    /// <typeparam name="T">The type of the delegate.</typeparam>
    /// <param name="type">The type of the class to search.</param>
    /// <param name="name">The name of the method.</param>
    /// <returns>The <see cref="MethodInfo"/> of the found method, or null if it was not found.</returns>
    public static MethodInfo? DelegeteMatch<T>(Type type, string name)
        where T : Delegate
    {
        MethodInfo del = typeof(T).GetMethod("Invoke");

        if (del == null)
        {
            return null;
        }

        foreach (MethodInfo method in type.GetMethods())
        {
            if (
                method.Name == name &&
                method.ReturnType == del.ReturnType &&
                method.GetParameters().Select(x => x.ParameterType).SequenceEqual(del.GetParameters().Select(x => x.ParameterType)) &&
                method.GetGenericArguments().SequenceEqual(del.GetGenericArguments()))
            {
                return method;
            }
        }

        return null;
    }
}
