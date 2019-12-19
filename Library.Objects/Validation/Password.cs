using System.Linq;
using Library.Objects.Helpers.Constants;

namespace Library.Objects.Validation
{
    public static class Password
    {
        private const byte MIN_CHARACTER_COUNT = 8;
        private const byte MIN_LOWER_LETTER_COUNT = 1;
        private const byte MIN_UPPER_LETTER_COUNT = 1;
        private const byte MIN_DIGIT_COUNT = 1;
        private const byte MIN_SPECIAL_CHARACTER_COUNT = 1;

        private const string ALLOWED_SPECIAL_CHARACTERS = "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

        public static class ErrorMessage
        {
            public readonly static string MinCharacterCount = string.Format("Password must be at least {0} characters", MIN_CHARACTER_COUNT);
            public readonly static string ValidCharacters = string.Format("Valid password characters are ONLY latin letters, digits or any of the following special characters: {0}", ALLOWED_SPECIAL_CHARACTERS);

            public readonly static string MinLowerLetterCount = string.Format("Password must contain at least {0} lower latin letters", MIN_LOWER_LETTER_COUNT);
            public readonly static string MinUpperLetterCount = string.Format("Password must contain at least {0} upper latin letters", MIN_UPPER_LETTER_COUNT);
            public readonly static string MinDigitCount = string.Format("Password must contain at least {0} digits", MIN_DIGIT_COUNT);
            public readonly static string MinSpecialCharacterCount = string.Format("Password must contain at least {0} of the following special characters: {1}", MIN_SPECIAL_CHARACTER_COUNT, ALLOWED_SPECIAL_CHARACTERS);
        }

        public static string Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return CommonErrorMessage.PASSWORD_REQUIRED;
            }

            if (password.Length < MIN_CHARACTER_COUNT)
            {
                return ErrorMessage.MinCharacterCount;
            }

            if (password.Any(ch => !IsVadlidCharacter(ch)))
            {
                return ErrorMessage.ValidCharacters;
            }

            if (password.Count(IsLowerLatinLetter) < MIN_LOWER_LETTER_COUNT)
            {
                return ErrorMessage.MinLowerLetterCount;
            }

            if (password.Count(IsUpperLatinLetter) < MIN_UPPER_LETTER_COUNT)
            {
                return ErrorMessage.MinUpperLetterCount;
            }

            if (password.Count(char.IsDigit) < MIN_DIGIT_COUNT)
            {
                return ErrorMessage.MinDigitCount;
            }

            if (password.Count(ch => ALLOWED_SPECIAL_CHARACTERS.Contains(ch)) < MIN_SPECIAL_CHARACTER_COUNT)
            {
                return ErrorMessage.MinSpecialCharacterCount;
            }

            return null;
        }

        private static bool IsVadlidCharacter(char ch)
        {
            return
                IsLowerLatinLetter(ch) ||
                IsUpperLatinLetter(ch) ||
                char.IsDigit(ch) ||
                ALLOWED_SPECIAL_CHARACTERS.Contains(ch);
        }

        private static bool IsLowerLatinLetter(char ch)
        {
            return ch >= 'a' && ch <= 'z';
        }
        private static bool IsUpperLatinLetter(char ch)
        {
            return ch >= 'A' && ch <= 'Z';
        }
    }
}
