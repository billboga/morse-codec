using System.Collections.Generic;

namespace MorseCodec
{
    public interface IMorseCode
    {
        char CharacterSeparator { get; }
        char DitCharacter { get; }
        char DahCharacter { get; }
        IDictionary<char, string> CharacterMap { get; }

        string Decode(string message, bool ignoreInvalidCharacters);
        string Encode(string message, bool ignoreInvalidCharacters);
    }
}
