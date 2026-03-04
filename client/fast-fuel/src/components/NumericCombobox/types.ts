export interface NumericComboboxNumberItem {
  value: number;
  disabled?: boolean;
}

export interface NumericComboboxItem extends NumericComboboxNumberItem {
  label: string;
}

export interface NumericComboboxItemGroup<T = NumericComboboxItem | number> {
  group: string;
  items: T[];
}
