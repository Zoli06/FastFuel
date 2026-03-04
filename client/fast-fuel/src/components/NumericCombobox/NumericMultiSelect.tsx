import { MultiSelect, type MultiSelectProps } from '@mantine/core';
import type { NumericComboboxItem, NumericComboboxItemGroup } from './types';

export type NumericMultiSelectProps = Omit<
  MultiSelectProps,
  'data' | 'value' | 'onChange' | 'defaultValue'
> & {
  data?:
    | Array<number | NumericComboboxItem | NumericComboboxItemGroup>
    | ReadonlyArray<number | NumericComboboxItem | NumericComboboxItemGroup>;
  value?: number[] | null;
  defaultValue?: number[] | null;
  onChange?: (value: number[]) => void;
};

export const NumericMultiSelect = ({
  data,
  value,
  onChange,
  defaultValue,
  ...rest
}: NumericMultiSelectProps) => {
  const selectData =
    data?.map((item) => {
      if (typeof item === 'number') {
        return { value: item.toString(), label: item.toString() };
      } else if ('value' in item) {
        return { value: item.value.toString(), label: item.label };
      } else {
        return {
          group: item.group,
          items: item.items.map((groupItem) => {
            if (typeof groupItem === 'number') {
              return { value: groupItem.toString(), label: groupItem.toString() };
            }
            return { value: groupItem.value.toString(), label: groupItem.label };
          }),
        };
      }
    }) || [];

  const valueStrs = value != null ? value.map((v) => v.toString()) : undefined;
  const defaultValueStrs = defaultValue != null ? defaultValue.map((v) => v.toString()) : undefined;

  const handleChange = (values: string[]) => {
    if (onChange) {
      onChange(values.map((v) => parseFloat(v)));
    }
  };

  return (
    <MultiSelect
      data={selectData}
      value={valueStrs}
      defaultValue={defaultValueStrs}
      onChange={handleChange}
      {...rest}
    />
  );
};
