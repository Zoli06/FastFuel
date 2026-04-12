const IMAGE_URL_ERROR = 'Image URL must be a valid http(s) URL';

export const validateOptionalImageUrl = (value: unknown): string | null => {
  if (value == null) return null;
  if (typeof value !== 'string') return IMAGE_URL_ERROR;

  const trimmed = value.trim();
  if (!trimmed) return null;

  try {
    const parsed = new URL(trimmed);
    return parsed.protocol === 'http:' || parsed.protocol === 'https:' ? null : IMAGE_URL_ERROR;
  } catch {
    return IMAGE_URL_ERROR;
  }
};
