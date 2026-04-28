export type OrderStatus = 'Pending' | 'InProgress' | 'Ready' | 'Completed' | 'Cancelled';
export type SortKey =
  | 'date-desc'
  | 'date-asc'
  | 'price-desc'
  | 'price-asc'
  | 'name-asc'
  | 'name-desc';

export interface OrderSummary {
  id: number;
  orderNumber: number;
  restaurantId: number;
  restaurantName: string;
  status: OrderStatus;
  totalPrice: number;
  placedAt: string;
  items: OrderSummaryItem[];
}

export interface OrderSummaryItem {
  name: string;
  quantity: number;
  price: number;
  note?: string;
  type: 'food' | 'menu';
  itemId?: number | null;
}
