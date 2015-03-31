using System;
using System.Collections.Generic;
using System.Linq;

namespace MorseCodec
{
    public abstract class AbstractMorseCodec : IMorseCode
    {
        public abstract char CharacterSeparator { get; }

        public abstract char DitCharacter { get; }

        public abstract char DahCharacter { get; }

        public abstract IDictionary<char, string> CharacterMap { get; }

        public virtual string Decode(string message)
        {
            throw new NotImplementedException();
        }

        public virtual string Encode(
            string message,
            bool ignoreInvalidCharacters = true)
        {
            if (!ignoreInvalidCharacters)
            {
                if (message
                    .Select(x => CharacterMap.ContainsKey(x))
                    .Where(x => x == false)
                    .Count() > 0)
                {
                    throw new FormatException("message contains invalid characters");
                }
            }

            var decodedMessage = "";

            foreach (var character in message)
            {
                if (character == CharacterSeparator)
                {
                    var iterationCount = decodedMessage.Last() == CharacterSeparator
                        ? 5
                        : 6;

                    decodedMessage += new string(CharacterSeparator, iterationCount);
                }
                else
                {
                    var encodedCharacter = CharacterMap.FirstOrDefault(x => x.Key == character);

                    if (!encodedCharacter.Equals(default(KeyValuePair<char, string>)))
                    {
                        decodedMessage += encodedCharacter.Value + CharacterSeparator;
                    }
                }
            }

            return decodedMessage.Trim();
        }
    }
}
