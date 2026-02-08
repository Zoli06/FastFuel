import type { components } from '../types/api';

const Food = ({ food }: { food: components['schemas']['FoodResponseDto'] }) => {
  return (
    <div>
      <h2>{food.name}</h2>
      <p>{food.description}</p>
    </div>
  );
};

export default Food;
