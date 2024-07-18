namespace RueI.Parsing;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using NorthwoodLib.Pools;

using RueI.Elements.Enums;
using RueI.Extensions;
using RueI.Parsing.Enums;
using RueI.Parsing.Records;
using RueI.Parsing.Tags.ConcreteTags;

/// <summary>
/// Helps parse the content of elements. This class cannot be inherited.
/// </summary>
/// <remarks>
/// The <see cref="Parser"/> is a sealed, immutable class that provides APIs for parsing (extracting the information of) hints so that
/// multiple can be displayed at once, along with the ability to add new <see cref="RichTextTag"/>s. In order to create new <see cref="Parser"/>s,
/// you must use the <see cref="ParserBuilder"/> class.
/// </remarks>
/// <include file='docs.xml' path='docs/members[@name="parser"]/Parser/*'/>
/// <seealso cref="ParserBuilder"/>
public sealed class Parser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="tags">The list of tags to initialize with.</param>
    /// <param name="backups">The list of parsers to use as a backup.</param>
    internal Parser(IEnumerable<RichTextTag> tags, IEnumerable<Parser> backups)
    {
        IEnumerable<ValueTuple<string, RichTextTag>> tuplePairs = tags.SelectMany(x => x.Names.Select(y => (y, x)));
        Tags = (Lookup<string, RichTextTag>)tuplePairs.ToLookup(x => x.Item1, x => x.Item2);
        TagBackups = new(backups.ToList());
    }

    /// <summary>
    /// Gets the default <see cref="Parser"/>.
    /// </summary>
    public static Parser DefaultParser { get; } = new ParserBuilder().AddFromAssembly(typeof(Parser).Assembly).Build();

    /// <summary>
    /// Gets the tags that will be searched for when parsing.
    /// </summary>
    /// <remarks>
    /// Multiple tags can share the same name.
    /// </remarks>
    public Lookup<string, RichTextTag> Tags { get; }

    /// <summary>
    /// Gets a list of <see cref="Parser"/>s which this <see cref="Parser"/> will include the tags for.
    /// </summary>
    public ReadOnlyCollection<Parser> TagBackups { get; }

    /// <summary>
    /// Adds a character to a parser context.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    /// <param name="ch">The character to add.</param>
    /// <param name="append">Whether or not the character should be appended to the <see cref="ParserContext"/>'s <see cref="ParserContext.ResultBuilder"/>.</param>
    public static void AddCharacter(ParserContext context, char ch, bool append = true)
    {
        float size = CalculateCharacterLength(context, ch);

        // TODO: support for surrogate pairs and CJK
        if (ch == ' ' || ch == '​') // zero width space
        {
            context.SpaceBuffer += size;

            if (!context.NoBreak)
            {
                context.CurrentLineWidth += context.WidthSinceSpace;
                context.WidthSinceSpace = 0;
            }
        }
        else
        {
            context.CurrentLineWidth += context.SpaceBuffer;
            context.SpaceBuffer = 0;

            if (context.CurrentLineWidth + context.WidthSinceSpace > context.FunctionalWidth)
            {
                CreateLineBreak(context);
            }

            context.WidthSinceSpace += size;
            context.LineHasAnyChars = true;
            if (context.Size > context.BiggestCharSize)
            {
                context.BiggestCharSize = context.Size;
            }
        }

        if (append)
        {
            context.ResultBuilder.Append(ch);
        }
    }

    /// <summary>
    /// Calculates the length of a <see cref="char"/> with a context.
    /// </summary>
    /// <param name="context">The context to parse the char under.</param>
    /// <param name="ch">The char to calculate the length for.</param>
    /// <returns>A float indicating the total length of the char.</returns>
    public static float CalculateCharacterLength(TextInfo context, char ch)
    {
        if (context.IsMonospace)
        {
            return context.Monospacing + context.CurrentCSpace;
        }

        char functionalCase = context.CurrentCase switch
        {
            CaseStyle.Smallcaps or CaseStyle.Uppercase => char.ToUpper(ch),
            CaseStyle.Lowercase => char.ToLower(ch),
            _ => ch
        };

        if (CharacterLengths.Lengths.TryGetValue(functionalCase, out float chSize))
        {
            float multiplier = context.Size / Constants.DEFAULTSIZE;
            if (context.CurrentCase == CaseStyle.Smallcaps && char.IsLower(ch))
            {
                multiplier *= 0.8f;
            }

            if (context.IsSuperscript)
            {
                multiplier *= 0.5f;
            }

            // for some reason, tmp allows the text to be in both subscript and superscript
            if (context.IsSubscript)
            {
                multiplier *= 0.5f;
            }

            return chSize * multiplier;
        }
        else
        {
            // TODO: handle warnings
            return default;
        }
    }

    /// <summary>
    /// Generates the effects of a linebreak for a parser.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    /// <param name="isOverflow">Whether or not the line break was caused by an overflow.</param>
    public static void CreateLineBreak(ParserContext context, bool isOverflow = false)
    {
        if (context.WidthSinceSpace > context.FunctionalWidth)
        {
            context.CurrentLineWidth = 0;
        }
        else
        {
            context.CurrentLineWidth = context.WidthSinceSpace;
        }

        context.BiggestCharSize = 0;
        context.LineHasAnyChars = false;
        context.NewOffset += context.CurrentLineHeight; // TODO: support margins

        if (!isOverflow)
        {
            context.CurrentLineWidth += context.LineIndent;
            context.WidthSinceSpace = 0f;
        }
    }

    /// <summary>
    /// Attempts to parse the tag attributes of a string.
    /// </summary>
    /// <param name="content">The content to parse.</param>
    /// <param name="attributes">The pairs of attributes.</param>
    /// <returns>true if the content is valid, otherwise false.</returns>,
    public static bool GetTagAttributes(string content, out Dictionary<string, string> attributes)
    {
        IEnumerable<string> result = content.Split('"')
                        .Select((element, index) => index % 2 == 0
                           ? element.Split(' ')
                           : new string[] { element })
                        .SelectMany(element => element);

        Dictionary<string, string> attributePairs = new(3);
        attributes = attributePairs;

        foreach (string possiblePair in result)
        {
            if (possiblePair == string.Empty)
            {
                return false;
            }

            string[] results = possiblePair.Split('=');

            if (results.Length != 2)
            {
                return false;
            }

            attributePairs.Add(results[0], results[1]);
        }

        return true;
    }

    /// <summary>
    /// Parses a rich text string.
    /// </summary>
    /// <param name="text">The string to parse.</param>
    /// <param name="options">The options of the element.</param>
    /// <returns>A <see cref="ParsedData"/> containing information about the string.</returns>
    public ParsedData Parse(string text, ElementOptions options = ElementOptions.Default)
    {
        ParserState currentState = ParserState.CollectingTags;

        StringBuilder tagBuffer = StringBuilderPool.Shared.Rent(Constants.MAXTAGNAMESIZE);
        int tagBufferSize = 0;

        RichTextTag? currentTag = null;
        char? delimiter = null;

        StringBuilder paramBuffer = StringBuilderPool.Shared.Rent(30);

        using ParserContext context = new();

        void FailTagMatch() // not a tag, unload buffer
        {
            AddCharacter(context, '<');

            AvoidMatch(context);
            foreach (char ch in tagBuffer.ToString())
            {
                AddCharacter(context, ch);
            }

            if (delimiter != null)
            {
                AddCharacter(context, delimiter.Value);
                delimiter = null;
            }

            foreach (char ch in paramBuffer.ToString())
            {
                AddCharacter(context, ch);
            }

            tagBuffer.Clear();
            paramBuffer.Clear();

            currentTag = null;
            currentState = ParserState.CollectingTags;
            tagBufferSize = 0;
        }

        char[] chars = text.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char ch = chars[i];

            if (ch == '<') // indicates start of tag
            {
                if (currentState != ParserState.CollectingTags)
                {
                    FailTagMatch();
                }

                currentState = ParserState.DescendingTag;
                continue;
            }
            else if (ch == '\n')
            {
                context.ResultBuilder.Append('\n');
                CreateLineBreak(context);
                if (currentState != ParserState.CollectingTags)
                {
                    FailTagMatch();
                }

                continue;
            }
            else if (currentState == ParserState.DescendingTag)
            {
                if ((ch > '\u0060' && ch < '\u007B') || ch == '-' || ch == '/') // detects if a character is a-z, -, or /
                {
                    if (tagBufferSize > Constants.MAXTAGNAMESIZE)
                    {
                        FailTagMatch();
                    }

                    tagBuffer.Append(ch);
                    continue;
                }
                else if (ch == '>')
                {
                    if (TryGetBestMatch(tagBuffer.ToString(), TagStyle.NoParams, out RichTextTag? tag))
                    {
                        if (context.ShouldParse || tag is CloseNoparseTag)
                        {
                            tag!.HandleTag(context, string.Empty);

                            tagBuffer.Clear();
                            paramBuffer.Clear();

                            currentTag = null;
                            currentState = ParserState.CollectingTags;
                            tagBufferSize = 0;
                            continue;
                        }
                        else
                        {
                            FailTagMatch();
                        }
                    }
                    else
                    {
                        FailTagMatch();
                    }
                }
                else if (ch == ' ' || ch == '=')
                {
                    if (context.ShouldParse)
                    {
                        TagStyle style = ch switch
                        {
                            ' ' => TagStyle.Attributes,
                            '=' => TagStyle.ValueParam,
                            _ => throw new ArgumentOutOfRangeException(nameof(ch)),
                        };

                        if (TryGetBestMatch(tagBuffer.ToString(), style, out RichTextTag? tag))
                        {
                            currentTag = tag;
                            delimiter = ch;

                            currentState = ParserState.CollectingParams;
                            continue;
                        }
                        else
                        {
                            FailTagMatch();
                        }
                    }
                    else
                    {
                        FailTagMatch();
                    }
                }
                else
                {
                    FailTagMatch();
                }
            }
            else if (currentState == ParserState.CollectingParams)
            {
                if (ch == '>')
                {
                    if (currentTag!.HandleTag(context, paramBuffer.ToString()))
                    {
                        tagBuffer.Clear();
                        paramBuffer.Clear();

                        currentTag = null;
                        delimiter = null;
                        currentState = ParserState.CollectingTags;
                        tagBufferSize = 0;

                        continue;
                    }
                    else
                    {
                        FailTagMatch();
                    }
                }

                paramBuffer.Append(ch);
                continue; // do NOT add as a character
            }

            if (ch == '\\')
            {
                int original = i;
                int length = chars.Length;
                for (; i < length && chars[i + 1] == '\\'; i++)
                {
                    context.ResultBuilder.Append(chars[i]);
                }

                int matcher = i - original;
                int times = matcher - (int)Math.Floor(matcher / 3d);

                // detect if an escape sequence is escaped by backslashes
                if ((i - original) % 3 == 0)
                {
                    if (context.ShouldParse || options.HasFlagFast(ElementOptions.NoparseParsesEscape))
                    {
                        switch (chars[i])
                        {
                            case 'n':
                                CreateLineBreak(context);
                                i++;
                                break;
                            case 'r':
                                context.CurrentLineWidth = 0;
                                i++;
                                break;
                            case 'u':
                                context.ResultBuilder.Append('\\'); // TODO: add support for unicode literals
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        context.ResultBuilder.Append('\\');
                        i++;
                    }
                }

                for (int newIndex = 0; newIndex < times; newIndex++)
                {
                    AddCharacter(context, chars[i + newIndex], false);
                }
            }

            AddCharacter(context, ch);
        } // foreach

        context.ApplyClosingTags();

        StringBuilderPool.Shared.Return(tagBuffer);
        StringBuilderPool.Shared.Return(paramBuffer);
        return new ParsedData(context.ResultBuilder.ToString(), context.NewOffset);
    }

    /// <summary>
    /// Calculates the size offset that should applied to the parser for a given line.
    /// </summary>
    /// <param name="biggestChar">The size of the biggest char within the line.</param>
    /// <returns>An offset that should be added to the parser.</returns>
    private static float CalculateSizeOffset(float biggestChar) => (1 - (biggestChar / Constants.DEFAULTSIZE)) * 8.485f; // (((biggestChar / Constants.DEFAULTSIZE * 0.2f) + 0.8f) * Constants.DEFAULTHEIGHT) - Constants.DEFAULTHEIGHT;

    private static bool IsValidTagChar(char ch) => (ch > '\u0060' && ch < '\u007B') || ch == '-' || ch == '/';

    /// <summary>
    /// Avoids the client TMP matching a tag.
    /// </summary>
    /// <param name="context">The context of the parser.</param>
    private static void AvoidMatch(ParserContext context) => context.ResultBuilder.Append(context.IsBold ? "<b>" : "</b>");

    /// <summary>
    /// Tries to get a <see cref="RichTextTag"/> for the given name and <see cref="TagStyle"/>.
    /// </summary>
    /// <param name="name">The name of the tag.</param>
    /// <param name="style">The style of the tag.</param>
    /// <param name="tag">The returned tag, if it exists.</param>
    /// <returns>A value indicating whether or not a tag was found.</returns>
    private bool TryGetBestMatch(string name, TagStyle style, out RichTextTag? tag)
    {
        // lookups return an empty ienumerable if theres no value with the key
        // so this is perfectly safe :3
        tag = Tags[name].FirstOrDefault(x => x.TagStyle == style);
        if (tag == null)
        {
            foreach (Parser parser in TagBackups)
            {
                tag = Tags[name].FirstOrDefault(x => x.TagStyle == style);

                if (tag != null)
                {
                    break;
                }
            }
        }

        return tag != null;
    }

    private RichTextTag? GetBestMatch(string name, TagStyle style)
    {
        foreach (Parser parser in TagBackups)
        {
            RichTextTag? tag = Tags[name].FirstOrDefault(x => x.TagStyle == style);
            if (tag != null)
            {
                return tag;
            }
        }

        return Tags[name].FirstOrDefault(x => x.TagStyle == style) ?? GetTagBackups(name, style);
    }

    private RichTextTag? GetTagBackups(string name, TagStyle style)
    {
        foreach (Parser parser in TagBackups)
        {
            RichTextTag? tag = Tags[name].FirstOrDefault(x => x.TagStyle == style);
            if (tag != null)
            {
                return tag;
            }
        }

        return null;
    }
}
