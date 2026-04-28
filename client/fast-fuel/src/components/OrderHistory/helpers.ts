import type { OrderStatus, OrderSummary, OrderSummaryItem, SortKey } from './types.ts';

export const STATUS_META: Record<OrderStatus, { label: string; color: string }> = {
  Pending: { label: 'Pending', color: '#c92a2a' },
  InProgress: { label: 'In Progress', color: 'blue' },
  Ready: { label: 'Ready', color: 'teal' },
  Completed: { label: 'Completed', color: 'green' },
  Cancelled: { label: 'Cancelled', color: 'red' },
};

export const ALL_STATUSES = Object.keys(STATUS_META) as OrderStatus[];

export const SORT_OPTIONS: { value: SortKey; label: string }[] = [
  { value: 'date-desc', label: 'Newest first' },
  { value: 'date-asc', label: 'Oldest first' },
  { value: 'price-desc', label: 'Price: high→low' },
  { value: 'price-asc', label: 'Price: low→high' },
  { value: 'name-asc', label: 'Restaurant A→Z' },
  { value: 'name-desc', label: 'Restaurant Z→A' },
];

export function formatDate(iso: string) {
  return new Date(iso).toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

export function sortOrders(orders: OrderSummary[], key: SortKey): OrderSummary[] {
  return [...orders].sort((a, b) => {
    switch (key) {
      case 'date-desc':
        return new Date(b.placedAt).getTime() - new Date(a.placedAt).getTime();
      case 'date-asc':
        return new Date(a.placedAt).getTime() - new Date(b.placedAt).getTime();
      case 'price-desc':
        return b.totalPrice - a.totalPrice;
      case 'price-asc':
        return a.totalPrice - b.totalPrice;
      case 'name-asc':
        return a.restaurantName.localeCompare(b.restaurantName);
      case 'name-desc':
        return b.restaurantName.localeCompare(a.restaurantName);
      default:
        return 0;
    }
  });
}

export function resolveItems(
  foods: {
    foodId?: number | null;
    quantity: number;
    specialInstructions?: string | null;
    originalFoodPrice: number;
    originalFoodName: string;
  }[],
  menus: {
    menuId?: number | null;
    quantity: number;
    specialInstructions?: string | null;
    originalMenuPrice: number;
    originalMenuName: string;
  }[],
): OrderSummaryItem[] {
  const items: OrderSummaryItem[] = [];

  for (const f of foods ?? []) {
    items.push({
      itemId: f.foodId,
      type: 'food',
      name: f.originalFoodName,
      quantity: f.quantity,
      price: f.originalFoodPrice,
      note: f.specialInstructions ?? undefined,
    });
  }

  for (const m of menus ?? []) {
    items.push({
      itemId: m.menuId,
      type: 'menu',
      name: m.originalMenuName,
      quantity: m.quantity,
      price: m.originalMenuPrice,
      note: m.specialInstructions ?? undefined,
    });
  }

  return items;
}
