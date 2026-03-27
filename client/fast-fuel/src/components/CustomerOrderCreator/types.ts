import type { components } from '../../types/api';

export type Menu = components['schemas']['MenuResponseDto'];
export type Food = components['schemas']['FoodResponseDto'];

export type UnifiedItem = (Menu & { type: 'menu' }) | (Food & { type: 'food' });

export type CartItem = {
  item: UnifiedItem;
  quantity: number;
  specialInstructions: string | null;
};

export type SortKey = 'name-asc' | 'name-desc' | 'price-asc' | 'price-desc';
