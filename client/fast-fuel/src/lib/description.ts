const DEFAULT_MAX_DESCRIPTION_LENGTH = 100;

export const getDisplayedDescription = (
  description: string | null | undefined,
  maxLength = DEFAULT_MAX_DESCRIPTION_LENGTH,
) => {
  if (!description) return 'No description provided';
  return description.length > maxLength ? `${description.substring(0, maxLength)}...` : description;
};
