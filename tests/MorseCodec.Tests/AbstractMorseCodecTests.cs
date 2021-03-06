﻿using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;

namespace MorseCodec.Tests
{
    public class AbstractMorseCodecTests
    {
        public AbstractMorseCodecTests()
        {
            sut = new TestAbstractMorseCode();
        }

        private readonly TestAbstractMorseCode sut;

        public class Decode : AbstractMorseCodecTests
        {
            [Fact]
            public void Should_throw_exception_if_message_contains_invalid_character()
            {
                Assert.Throws<FormatException>(() => sut.Decode(
                    message: "abc",
                    ignoreInvalidCharacters: false));
            }

            [Theory,
            InlineData("... ..- .-.", "abc"),
            InlineData("...       ..-       .-.", "a b c"),
            InlineData("... ..- .-.       ... ..- .-.", "abc abc"),
            InlineData("...", "a"),
            InlineData("... ", "a"),
            InlineData("...  ", "a"),
            InlineData("...   ", "a"),
            InlineData("...    ", "a"),
            InlineData("...     ", "a"),
            InlineData("...      ", "a")]
            public void Should_property_decode_message(string message, string expectedDecodedMessage)
            {
                var @return = sut.Decode(message);

                Assert.Equal(expectedDecodedMessage, @return);
            }
        }

        public class Encode : AbstractMorseCodecTests
        {
            [Fact]
            public void Should_throw_exception_if_message_contains_invalid_character()
            {
                Assert.Throws<FormatException>(() => sut.Encode(
                    message: "abcd",
                    ignoreInvalidCharacters: false));
            }

            [Theory,
            InlineData("abc", "... ..- .-."),
            InlineData("a b c", "...       ..-       .-."),
            InlineData("abc abc", "... ..- .-.       ... ..- .-."),
            InlineData("<abc> <abc>", ".....-.-.       .....-.-."),
            InlineData("<abc> a b", ".....-.-.       ...       ..-")]
            public void Should_properly_encode_message(string message, string expectedEncodedMessage)
            {
                var @return = sut.Encode(message);

                Assert.Equal(expectedEncodedMessage, @return);
            }
        }

        private class TestAbstractMorseCode : AbstractMorseCodec
        {
            private IDictionary<string, string> characterMap = new Dictionary<string, string>()
            {
                { "a", "..." },
                { "b", "..-" },
                { "c", ".-." },
                { "<abc>", ".....-.-." }
            };

            public override char CharacterSeparator
            {
                get { return ' '; }
            }

            public override char DitCharacter
            {
                get { return '.'; }
            }

            public override char DahCharacter
            {
                get { return '-'; }
            }

            public override IDictionary<string, string> CharacterMap
            {
                get { return characterMap; }
            }
        }
    }
}
