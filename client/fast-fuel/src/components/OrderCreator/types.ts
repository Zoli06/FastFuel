import type { components } from '../../types/api-schema.generated.ts';

export type Menu = components['schemas']['MenuResponseDto'];
export type Food = components['schemas']['FoodResponseDto'];

export type UnifiedItem = (Menu & { type: 'menu' }) | (Food & { type: 'food' });

export type CartItem = {
  item: UnifiedItem;
  quantity: number;
  specialInstructions: string | null;
};

export type CartEntry = CartItem & { cartKey: string };

export type CheckoutStep = 'idle' | 'confirm' | 'payment' | 'thankyou';

export type PaymentMethod = 'card' | 'cash';

export type SortKey = 'name-asc' | 'name-desc' | 'price-asc' | 'price-desc';
