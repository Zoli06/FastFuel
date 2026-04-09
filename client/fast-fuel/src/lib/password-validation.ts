export const PASSWORD_COMPLEXITY_MESSAGE =
  "Passwords must be at least 6 characters.; Passwords must have at least one non alphanumeric character.; Passwords must have at least one digit ('0'-'9').; Passwords must have at least one uppercase ('A'-'Z').";

export function validatePasswordComplexity(password: string | null | undefined): string | null {
  const value = password ?? '';
  const hasMinLength = value.length >= 6;
  const hasNonAlphanumeric = /[^A-Za-z0-9]/.test(value);
  const hasDigit = /\d/.test(value);
  const hasUppercase = /[A-Z]/.test(value);

  if (hasMinLength && hasNonAlphanumeric && hasDigit && hasUppercase) {
    return null;
  }

  return PASSWORD_COMPLEXITY_MESSAGE;
}

export function validatePasswordComplexityIfProvided(
  password: string | null | undefined,
): string | null {
  if (password === '' || password === null || password === undefined) {
    return null;
  }

  return validatePasswordComplexity(password);
}
