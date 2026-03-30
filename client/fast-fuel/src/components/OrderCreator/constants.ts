import {
  IconBottleFilled,
  IconBurger,
  IconLayoutGrid,
  IconToolsKitchen3,
} from '@tabler/icons-react';

export const CATEGORIES = [
  { label: 'All', value: 'all', icons: [IconLayoutGrid] },
  { label: 'Foods', value: 'food', icons: [IconToolsKitchen3] },
  { label: 'Menus', value: 'menu', icons: [IconBurger, IconBottleFilled] },
] as const;

export type CategoryValue = (typeof CATEGORIES)[number]['value'];
