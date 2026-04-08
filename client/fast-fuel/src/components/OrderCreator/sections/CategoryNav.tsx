import { Box, Group, Stack, UnstyledButton } from '@mantine/core';
import { CATEGORIES, type CategoryValue } from '../constants.ts';

type CategoryNavProps = {
  activeCategory: CategoryValue;
  onCategoryChange: (value: CategoryValue) => void;
};

export const CategoryNav = ({ activeCategory, onCategoryChange }: CategoryNavProps) => {
  return (
    <>
      <Stack
        visibleFrom="sm"
        gap={4}
        p="sm"
        style={{
          width: 100,
          minHeight: '100vh',
          background: 'var(--mantine-color-dark-8, #1a1a1a)',
          borderRight: '2px solid beige',
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

      <Box hiddenFrom="sm">
        <Group
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
                  background: isActive ? 'beige' : 'var(--mantine-color-dark-6)',
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
      </Box>
    </>
  );
};
