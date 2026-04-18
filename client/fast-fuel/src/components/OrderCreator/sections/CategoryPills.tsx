import { Group, UnstyledButton } from '@mantine/core';
import { CATEGORIES, type CategoryValue } from '../constants.ts';

type CategoryPillsProps = {
  activeCategory: CategoryValue;
  onCategoryChange: (value: CategoryValue) => void;
};

export const CategoryPills = ({ activeCategory, onCategoryChange }: CategoryPillsProps) => {
  return (
    <Group
      hiddenFrom="md"
      gap={8}
      style={{
        overflowX: 'auto',
        flexWrap: 'nowrap',
        paddingBottom: 4,
        scrollbarWidth: 'none',
      }}
    >
      {CATEGORIES.map((cat) => {
        const isActive = activeCategory === cat.value;
        return (
          <UnstyledButton
            key={cat.value}
            onClick={() => onCategoryChange(cat.value)}
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: 6,
              padding: '6px 14px',
              borderRadius: 20,
              background: isActive ? 'darkred' : 'var(--mantine-color-dark-6)',
              color: isActive ? '#fff' : 'var(--mantine-color-gray-4)',
              fontWeight: isActive ? 700 : 400,
              fontSize: 13,
              whiteSpace: 'nowrap',
              flexShrink: 0,
              transition: 'all 0.15s',
            }}
          >
            <Group gap={4}>
              {cat.icons.map((Icon, i) => (
                <Icon key={i} size={15} />
              ))}
            </Group>
            {cat.label}
          </UnstyledButton>
        );
      })}
    </Group>
  );
};
