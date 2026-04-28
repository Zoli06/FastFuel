import { Group, Stack, UnstyledButton } from '@mantine/core';
import { CATEGORIES, type CategoryValue } from '../constants.ts';

type CategoryNavProps = {
  activeCategory: CategoryValue;
  onCategoryChange: (value: CategoryValue) => void;
};

export const CategoryNav = ({ activeCategory, onCategoryChange }: CategoryNavProps) => {
  return (
    <Stack
      visibleFrom="md"
      gap={4}
      p="sm"
      style={{
        width: 100,
        minHeight: '100vh',
        background: 'var(--mantine-color-dark-8, #1a1a1a)',
        position: 'sticky',
        top: 0,
        flexShrink: 0,
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
              flexDirection: 'column',
              alignItems: 'center',
              gap: 4,
              padding: '10px 6px',
              borderRadius: 8,
              background: isActive ? 'darkred' : 'transparent',
              color: isActive ? '#fff' : 'var(--mantine-color-gray-4)',
              fontWeight: isActive ? 700 : 400,
              fontSize: 11,
              transition: 'all 0.15s',
              textAlign: 'center',
            }}
          >
            <Group gap={2} justify="center">
              {cat.icons.map((Icon, i) => (
                <Icon key={i} size={18} />
              ))}
            </Group>
            {cat.label}
          </UnstyledButton>
        );
      })}
    </Stack>
  );
};
