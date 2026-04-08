import type { OrderStatus, OrderSummary, OrderSummaryItem, SortKey } from './types.ts';

export const STATUS_META: Record<OrderStatus, { label: string; color: string }> = {
  Pending: { label: 'Pending', color: '#c92a2a' },
  InProgress: { label: 'In Progress', color: '#BFB48F' },
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
  foods: { foodId: number; quantity: number; specialInstructions?: string | null }[],
  menus: { menuId: number; quantity: number; specialInstructions?: string | null }[],
  foodMap: Map<number, { id: number; name: string; price: number }>,
  menuMap: Map<number, { id: number; name: string; price: number }>,
): OrderSummaryItem[] {
  const items: OrderSummaryItem[] = [];

  for (const f of foods ?? []) {
    const found = foodMap.get(f.foodId);
    items.push({
      itemId: f.foodId,
      type: 'food',
      name: found?.name ?? `Food #${f.foodId}`,
      quantity: f.quantity,
      price: found?.price ?? 0,
      note: f.specialInstructions ?? undefined,
    });
  }

  for (const m of menus ?? []) {
    const found = menuMap.get(m.menuId);
    items.push({
      itemId: m.menuId,
      type: 'menu',
      name: found?.name ? `${found.name} (Menu)` : `Menu #${m.menuId}`,
      quantity: m.quantity,
      price: found?.price ?? 0,
      note: m.specialInstructions ?? undefined,
    });
  }

  return items;
}
