const PASSWORD_MIN_LENGTH_MESSAGE = 'Passwords must be at least 6 characters.';
const PASSWORD_NON_ALPHANUMERIC_MESSAGE =
  'Passwords must have at least one non alphanumeric character.';
const PASSWORD_DIGIT_MESSAGE = "Passwords must have at least one digit ('0'-'9').";
const PASSWORD_UPPERCASE_MESSAGE = "Passwords must have at least one uppercase ('A'-'Z').";
const PASSWORD_LOWERCASE_MESSAGE = "Passwords must have at least one lowercase ('a'-'z').";

export function validatePasswordComplexity(password: string | null | undefined): string {
  const value = password ?? '';
  const hasMinLength = value.length >= 6;
  const hasNonAlphanumeric = /[^A-Za-z0-9]/.test(value);
  const hasDigit = /\d/.test(value);
  const hasUppercase = /[A-Z]/.test(value);
  const hasLowercase = /[a-z]/.test(value);

  if (!hasMinLength) return PASSWORD_MIN_LENGTH_MESSAGE;
  if (!hasNonAlphanumeric) return PASSWORD_NON_ALPHANUMERIC_MESSAGE;
  if (!hasDigit) return PASSWORD_DIGIT_MESSAGE;
  if (!hasUppercase) return PASSWORD_UPPERCASE_MESSAGE;
  if (!hasLowercase) return PASSWORD_LOWERCASE_MESSAGE;

  return '';
}

export function validatePasswordComplexityIfProvided(password: string | null | undefined): string {
  if (password === '' || password === null || password === undefined) {
    return '';
  }

  return validatePasswordComplexity(password);
}
