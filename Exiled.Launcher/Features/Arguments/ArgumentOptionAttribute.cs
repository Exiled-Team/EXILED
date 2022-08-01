// -----------------------------------------------------------------------
// <copyright file="ArgumentOptionAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Launcher.Features.Arguments;

public class ArgumentOptionAttribute : Attribute
{
    public string ShortName;
    public string Name;
    public string HelpText;

    public ArgumentOptionAttribute(string shortName, string name, string helpText)
    {
        ShortName = shortName;
        Name = name;
        HelpText = helpText;
    }
}
