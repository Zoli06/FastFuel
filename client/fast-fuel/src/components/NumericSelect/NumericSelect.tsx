import { Select, type ComboboxItemGroup, type SelectProps } from '@mantine/core';

interface ComboboxNumericItem {
  value: number;
  disabled?: boolean;
}

interface ComboboxItem extends ComboboxNumericItem {
  label: string;
}

export type NumericSelectProps = Omit<
  SelectProps,
  'data' | 'value' | 'onChange' | 'defaultValue'
> & {
  data?:
    | Array<number | ComboboxItem | ComboboxItemGroup>
    | ReadonlyArray<number | ComboboxItem | ComboboxItemGroup>;
  value?: number | null;
  defaultValue?: number | null;
  onChange?: (value: number | null, option: ComboboxItem) => void;
};

export const NumericSelect = ({
  data,
  value,
  onChange,
  defaultValue,
  ...rest
}: NumericSelectProps) => {
  // Holy AI
  // Now pray

  // Helper: normalize various input item shapes into Select item with string value
  const normalizeItem = (d: unknown) => {
    // number
    if (typeof d === 'number') {
      const v = String(d);
      return { value: v, label: v } as const;
    }

    // object-like
    if (d && typeof d === 'object') {
      const obj = d as { value?: unknown; label?: unknown; disabled?: unknown };

      // If value is number
      if (typeof obj.value === 'number') {
        const v = String(obj.value);
        const label = typeof obj.label === 'string' ? obj.label : v;
        const disabled = Boolean(obj.disabled);
        return { value: v, label, disabled } as const;
      }

      // If value is string (e.g., already a Select item), keep it
      if (typeof obj.value === 'string') {
        const v = obj.value;
        const label = typeof obj.label === 'string' ? obj.label : v;
        const disabled = Boolean(obj.disabled);
        return { value: v, label, disabled } as const;
      }

      // Fallback: stringify whole object
      const fallback = String(((obj.label ?? obj.value) as unknown) ?? String(d));
      return { value: fallback, label: fallback } as const;
    }

    // Fallback for unexpected types
    const v = String(d);
    return { value: v, label: v } as const;
  };

  // Type guard to detect groups
  const isGroup = (x: unknown): x is ComboboxItemGroup => {
    return (
      typeof x === 'object' &&
      x !== null &&
      'items' in (x as Record<string, unknown>) &&
      Array.isArray((x as Record<string, unknown>).items)
    );
  };

  const normalizeData = (
    items?:
      | Array<number | ComboboxItem | ComboboxItemGroup>
      | ReadonlyArray<number | ComboboxItem | ComboboxItemGroup>,
  ) => {
    if (!items) return undefined;

    return items.map((it) => {
      if (isGroup(it)) {
        const group = it as ComboboxItemGroup;
        const label = (group as { label?: string }).label;
        const inner = (group.items ?? []).map((innerIt: unknown) => normalizeItem(innerIt));
        return { label, items: inner } as unknown as ComboboxItemGroup;
      }

      return normalizeItem(it as unknown);
    }) as SelectProps['data'];
  };

  const selectData = normalizeData(data);

  const valueStr: string | null = value === null || value === undefined ? null : String(value);
  const defaultValueStr: string | undefined =
    defaultValue === null || defaultValue === undefined ? undefined : String(defaultValue);

  const handleChange: SelectProps['onChange'] = (val, option) => {
    if (!onChange) return;

    const parsedValue = val === null ? null : Number(val);

    // Build ComboboxItem to pass to caller
    let parsedOption: ComboboxItem;
    if (option && typeof option === 'object' && 'value' in option) {
      const opt = option as { value?: unknown; label?: unknown; disabled?: unknown };
      const optVal = opt.value;
      const numericVal = typeof optVal === 'number' ? optVal : Number(String(optVal ?? ''));
      const label = typeof opt.label === 'string' ? opt.label : String(opt.value ?? String(optVal));
      parsedOption = { value: numericVal, label, disabled: Boolean(opt.disabled) };
    } else {
      parsedOption = {
        value: parsedValue ?? NaN,
        label: val === null ? '' : String(val),
        disabled: false,
      };
    }

    onChange(parsedValue, parsedOption);
  };

  return (
    <Select
      data={selectData}
      value={valueStr}
      defaultValue={defaultValueStr}
      onChange={handleChange}
      {...rest}
    />
  );
};
