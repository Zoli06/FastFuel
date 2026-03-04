import { type ComboboxItem, Select, type SelectProps } from '@mantine/core';
import type { NumericComboboxItem, NumericComboboxItemGroup } from './types';

export type NumericSelectProps = Omit<
  SelectProps,
  'data' | 'value' | 'onChange' | 'defaultValue'
> & {
  data?:
    | Array<number | NumericComboboxItem | NumericComboboxItemGroup>
    | ReadonlyArray<number | NumericComboboxItem | NumericComboboxItemGroup>;
  value?: number | null;
  defaultValue?: number | null;
  onChange?: (value: number | null, option: NumericComboboxItem) => void;
};

export const NumericSelect = ({
  data,
  value,
  onChange,
  defaultValue,
  ...rest
}: NumericSelectProps) => {
  const selectData =
    data?.map((item) => {
      if (typeof item === 'number') {
        return { value: item.toString(), label: item.toString() };
      } else if ('value' in item) {
        return { value: item.value.toString(), label: item.label };
      } else {
        return { value: item.group, label: item.group };
      }
    }) || [];

  const valueStr = value !== undefined && value !== null ? value.toString() : '';
  const defaultValueStr =
    defaultValue !== undefined && defaultValue !== null ? defaultValue.toString() : '';

  const handleChange = (value: string | null, option: ComboboxItem) => {
    if (onChange) {
      const numericValue = value !== null ? parseFloat(value) : null;
      const numericOption = { ...option, value: parseFloat(option.value) };
      onChange(numericValue, numericOption);
    }
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
