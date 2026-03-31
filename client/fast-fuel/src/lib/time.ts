export const getDuration = (start: Date, end: Date) => {
  const diffMs = end.getTime() - start.getTime();
  const totalMinutes = Math.round(diffMs / 60000);
  return {
    hours: Math.floor(totalMinutes / 60),
    minutes: totalMinutes % 60,
  };
};

export const parseAsUtcDate = (value: string) => {
  const normalized = normalizeDateTime(value);
  const hasTimeZone = /(?:Z|[+-]\d{2}:\d{2})$/i.test(normalized);
  return new Date(hasTimeZone ? normalized : `${normalized}Z`);
};

export const normalizeDateTime = (value: string) => {
  const trimmed = value.trim();
  return trimmed.includes('T') ? trimmed : trimmed.replace(' ', 'T');
};
