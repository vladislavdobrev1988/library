using System.Linq;

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

        private static class ErrorMessage
        {
            public const string REQUIRED = "Password is required";
            public const string MIN_CHARACTER_COUNT_FORMAT = "Password must be at least {0} characters";
            public const string VALID_CHARACTERS_FORMAT = "Valid password characters are ONLY latin letters, digits or any of the following special characters: {0}";

            public const string MIN_LOWER_LETTER_COUNT_FORMAT = "Password must contain at least {0} lower latin letters";
            public const string MIN_UPPER_LETTER_COUNT_FORMAT = "Password must contain at least {0} upper latin letters";
            public const string MIN_DIGIT_COUNT_FORMAT = "Password must contain at least {0} digits";
            public const string MIN_SPECIAL_CHARACTER_COUNT_FORMAT = "Password must contain at least {0} of the following special characters: {1}";
        }

        public static string Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return ErrorMessage.REQUIRED;
            }

            if (password.Length < MIN_CHARACTER_COUNT)
            {
                return string.Format(ErrorMessage.MIN_CHARACTER_COUNT_FORMAT, MIN_CHARACTER_COUNT);
            }

            if (password.Any(ch => !IsVadlidCharacter(ch)))
            {
                return string.Format(ErrorMessage.VALID_CHARACTERS_FORMAT, ALLOWED_SPECIAL_CHARACTERS);
            }

            if (password.Count(IsLowerLatinLetter) < MIN_LOWER_LETTER_COUNT)
            {
                return string.Format(ErrorMessage.MIN_LOWER_LETTER_COUNT_FORMAT, MIN_LOWER_LETTER_COUNT);
            }

            if (password.Count(IsUpperLatinLetter) < MIN_UPPER_LETTER_COUNT)
            {
                return string.Format(ErrorMessage.MIN_UPPER_LETTER_COUNT_FORMAT, MIN_UPPER_LETTER_COUNT);
            }

            if (password.Count(char.IsDigit) < MIN_DIGIT_COUNT)
            {
                return string.Format(ErrorMessage.MIN_DIGIT_COUNT_FORMAT, MIN_DIGIT_COUNT);
            }

            if (password.Count(ch => ALLOWED_SPECIAL_CHARACTERS.Contains(ch)) < MIN_SPECIAL_CHARACTER_COUNT)
            {
                return string.Format(ErrorMessage.MIN_SPECIAL_CHARACTER_COUNT_FORMAT, MIN_SPECIAL_CHARACTER_COUNT, ALLOWED_SPECIAL_CHARACTERS);
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
