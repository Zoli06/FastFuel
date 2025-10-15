// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-ignore
import { DateTime, LocalTime, URL, UnsignedInt } from "graphql-scalars/typings/typeDefs";
export type Maybe<T> = T | null;
export type InputMaybe<T> = Maybe<T>;
export type Exact<T extends { [key: string]: unknown }> = { [K in keyof T]: T[K] };
export type MakeOptional<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]?: Maybe<T[SubKey]> };
export type MakeMaybe<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]: Maybe<T[SubKey]> };
export type MakeEmpty<T extends { [key: string]: unknown }, K extends keyof T> = { [_ in K]?: never };
export type Incremental<T> = T | { [P in keyof T]?: P extends ' $fragmentName' | '__typename' ? T[P] : never };
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
  ID: { input: string; output: string; }
  String: { input: string; output: string; }
  Boolean: { input: boolean; output: boolean; }
  Int: { input: number; output: number; }
  Float: { input: number; output: number; }
  DateTime: { input: DateTime; output: DateTime; }
  LocalTime: { input: LocalTime; output: LocalTime; }
  URL: { input: URL; output: URL; }
  UnsignedInt: { input: UnsignedInt; output: UnsignedInt; }
};

export type Allergy = {
  __typename: 'Allergy';
  id: Scalars['ID']['output'];
  ingredients: Array<Ingredient>;
  message: Maybe<Scalars['String']['output']>;
  name: Scalars['String']['output'];
};

export type DayOfWeek =
  | 'FRIDAY'
  | 'MONDAY'
  | 'SATURDAY'
  | 'SUNDAY'
  | 'THURSDAY'
  | 'TUESDAY'
  | 'WEDNESDAY';

export type Food = {
  __typename: 'Food';
  description: Maybe<Scalars['String']['output']>;
  id: Scalars['ID']['output'];
  imageUrl: Maybe<Scalars['URL']['output']>;
  ingredients: Array<Ingredient>;
  menus: Array<Menu>;
  name: Scalars['String']['output'];
  price: Scalars['UnsignedInt']['output'];
};

export type FoodIngredient = {
  __typename: 'FoodIngredient';
  food: Food;
  id: Scalars['ID']['output'];
  ingredient: Ingredient;
  quantity: Scalars['UnsignedInt']['output'];
};

export type Ingredient = {
  __typename: 'Ingredient';
  allergies: Array<Allergy>;
  foods: Array<Food>;
  id: Scalars['ID']['output'];
  imageUrl: Maybe<Scalars['URL']['output']>;
  name: Scalars['String']['output'];
  stationCategory: StationCategory;
};

export type Menu = {
  __typename: 'Menu';
  description: Maybe<Scalars['String']['output']>;
  foods: Array<Food>;
  id: Scalars['ID']['output'];
  imageUrl: Maybe<Scalars['URL']['output']>;
  isSpecialDeal: Scalars['Boolean']['output'];
  name: Scalars['String']['output'];
  price: Scalars['UnsignedInt']['output'];
};

export type MenuFood = {
  __typename: 'MenuFood';
  food: Food;
  id: Scalars['ID']['output'];
  menu: Menu;
  quantity: Scalars['UnsignedInt']['output'];
};

export type OpeningHour = {
  __typename: 'OpeningHour';
  closeTime: Scalars['LocalTime']['output'];
  dayOfWeek: DayOfWeek;
  id: Scalars['ID']['output'];
  openTime: Scalars['LocalTime']['output'];
  restaurant: Restaurant;
};

export type Order = {
  __typename: 'Order';
  completedAt: Maybe<Scalars['DateTime']['output']>;
  createdAt: Scalars['DateTime']['output'];
  id: Scalars['ID']['output'];
  orderNumber: Scalars['UnsignedInt']['output'];
  restaurant: Restaurant;
  status: OrderStatus;
};

export type OrderFood = {
  __typename: 'OrderFood';
  food: Food;
  id: Scalars['ID']['output'];
  order: Order;
  quantity: Scalars['UnsignedInt']['output'];
  specialInstructions: Maybe<Scalars['String']['output']>;
};

export type OrderMenu = {
  __typename: 'OrderMenu';
  id: Scalars['ID']['output'];
  menu: Menu;
  order: Order;
  quantity: Scalars['UnsignedInt']['output'];
  specialInstructions: Maybe<Scalars['String']['output']>;
};

export type OrderStatus =
  | 'CANCELLED'
  | 'COMPLETED'
  | 'IN_PROGRESS'
  | 'PENDING';

export type Query = {
  __typename: 'Query';
  allergies: Array<Allergy>;
  allergyById: Allergy;
  foodById: Food;
  foods: Array<Food>;
  ingredientById: Ingredient;
  ingredients: Array<Ingredient>;
  menuById: Menu;
  menus: Array<Menu>;
  openingHoursByRestaurantId: OpeningHour;
  orderById: Order;
  orders: Array<Order>;
  restaurantById: Restaurant;
  restaurants: Array<Restaurant>;
  stationById: Station;
  stationCategories: Array<StationCategory>;
  stationCategoryById: StationCategory;
  stations: Array<Station>;
};


export type QueryAllergyByIdArgs = {
  id: Scalars['ID']['input'];
};


export type QueryFoodByIdArgs = {
  id: Scalars['ID']['input'];
};


export type QueryIngredientByIdArgs = {
  id: Scalars['ID']['input'];
};


export type QueryMenuByIdArgs = {
  id: Scalars['ID']['input'];
};


export type QueryOpeningHoursByRestaurantIdArgs = {
  restaurantId: Scalars['ID']['input'];
};


export type QueryOrderByIdArgs = {
  id: Scalars['ID']['input'];
};


export type QueryRestaurantByIdArgs = {
  id: Scalars['ID']['input'];
};


export type QueryStationByIdArgs = {
  id: Scalars['ID']['input'];
};


export type QueryStationCategoryByIdArgs = {
  id: Scalars['ID']['input'];
};

export type Restaurant = {
  __typename: 'Restaurant';
  address: Scalars['String']['output'];
  description: Maybe<Scalars['String']['output']>;
  id: Scalars['ID']['output'];
  latitude: Scalars['Float']['output'];
  longitude: Scalars['Float']['output'];
  name: Scalars['String']['output'];
  phone: Maybe<Scalars['String']['output']>;
};

export type Station = {
  __typename: 'Station';
  id: Scalars['ID']['output'];
  inOperation: Scalars['Boolean']['output'];
  name: Scalars['String']['output'];
  restaurant: Restaurant;
  stationCategory: StationCategory;
};

export type StationCategory = {
  __typename: 'StationCategory';
  id: Scalars['ID']['output'];
  ingredients: Array<Ingredient>;
  name: Scalars['String']['output'];
  stations: Array<Station>;
};

export type FoodsQueryVariables = Exact<{ [key: string]: never; }>;


export type FoodsQuery = { foods: Array<{ __typename: 'Food', id: string, name: string, description: string | null, imageUrl: URL | null }> };
