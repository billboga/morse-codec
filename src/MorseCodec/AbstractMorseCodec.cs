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

        public abstract IDictionary<string, string> CharacterMap { get; }

        public virtual string Decode(
            string message,
            bool ignoreInvalidCharacters = true)
        {
            if (!ignoreInvalidCharacters)
            {
                if (message
                    .Select(x =>
                        x == CharacterSeparator ||
                        x == DitCharacter ||
                        x == DahCharacter)
                    .Where(x => x == false)
                    .Count() > 0)
                {
                    throw new FormatException("message contains invalid characters");
                }
            }

            var formattedMessage = message
                .Replace(new string(CharacterSeparator, 7), " <space> ");

            var decodedMessage = "";

            foreach (var block in formattedMessage.Split(CharacterSeparator))
            {
                var decodedCharacter = CharacterMap.FirstOrDefault(x => x.Value == block);

                if (block == "<space>")
                {
                    decodedMessage += CharacterSeparator;
                }
                else if (!decodedCharacter.Equals(default(KeyValuePair<char, string>)))
                {
                    decodedMessage += decodedCharacter.Key;
                }
            }

            return decodedMessage.Trim();
        }

        public virtual string Encode(
            string message,
            bool ignoreInvalidCharacters = true)
        {
            if (!ignoreInvalidCharacters)
            {
                if (message
                    .Select(x => CharacterMap.ContainsKey(x.ToString()))
                    .Where(x => x == false)
                    .Count() > 0)
                {
                    throw new FormatException("message contains invalid characters");
                }
            }

            var encodedMessage = "";
            var isProsign = false;
            var prosign = string.Empty;

            foreach (var character in message)
            {
                if (character == CharacterSeparator)
                {
                    var iterationCount = encodedMessage.Last() == CharacterSeparator
                        ? 6
                        : 7;

                    encodedMessage += new string(CharacterSeparator, iterationCount);
                }
                else if (character == '<')
                {
                    isProsign = true;
                    prosign = character.ToString();

                    continue;
                }
                else
                {
                    KeyValuePair<string, string> encodedCharacter;

                    if (isProsign && character != '>')
                    {
                        prosign += character.ToString();

                        continue;
                    }
                    else if (isProsign && character == '>')
                    {
                        prosign += character.ToString();

                        encodedCharacter = CharacterMap.FirstOrDefault(x => x.Key == prosign);

                        isProsign = false;
                    }
                    else
                    {
                        encodedCharacter = CharacterMap.FirstOrDefault(x => x.Key == character.ToString());
                    }

                    if (!encodedCharacter.Equals(default(KeyValuePair<char, string>)))
                    {
                        encodedMessage += encodedCharacter.Value + CharacterSeparator;
                    }
                }
            }

            return encodedMessage.Trim();
        }

        public void PlaybackMessage(
            string message,
            Action onEvent,
            Action offEvent,
            int ditDuration = 50)
        {
            var formattedMessage = message
                .Replace(new string(CharacterSeparator, 7), " <space> ");
            var eventList = new Queue<Action>();

            foreach (var block in formattedMessage.Split(CharacterSeparator))
            {
                if (block == "<space>")
                {
                    eventList.Enqueue(null);
                    eventList.Enqueue(null);
                    eventList.Enqueue(null);
                    eventList.Enqueue(null);
                }
                else
                {
                    foreach (var character in block)
                    {
                        eventList.Enqueue(onEvent);

                        if (character == DahCharacter)
                        {
                            eventList.Enqueue(null);
                            eventList.Enqueue(null);
                        }

                        eventList.Enqueue(offEvent);
                    }

                    eventList.Enqueue(null);
                    eventList.Enqueue(null);
                }
            }

            var timer = new System.Timers.Timer(ditDuration);

            timer.Elapsed += (source, eventArguments) =>
            {
                if (eventList.Count() == 0)
                {
                    timer.Stop();
                }
                else
                {
                    var currentEvent = eventList.Dequeue();

                    if (currentEvent != null)
                    {
                        currentEvent();
                    }
                }
            };

            timer.Start();
        }
    }
}
