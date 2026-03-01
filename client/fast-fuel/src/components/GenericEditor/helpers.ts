import type { FieldOrFieldset, FormValues } from './types.ts';

export function collectNullableKeys<T extends FormValues>(
  fields: FieldOrFieldset<T>[],
): Set<string> {
  const keys = new Set<string>();
  for (const f of fields) {
    if (f.type === 'fieldset') {
      for (const sub of f.fields) {
        if ('nullable' in sub && sub.nullable && 'key' in sub && sub.key) keys.add(sub.key);
      }
    } else if ('nullable' in f && f.nullable && 'key' in f && f.key) {
      keys.add(f.key);
    }
  }
  return keys;
}

export function convertNullables<T extends FormValues>(values: T, nullableKeys: Set<string>): T {
  const result = { ...values };
  for (const key of nullableKeys) {
    if (key in result && !result[key]) {
      (result as Record<string, unknown>)[key] = null;
    }
  }
  return result;
}
