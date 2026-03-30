import { Button, Group, NumberInput, Stack, Text } from '@mantine/core';

type QuantitySelectorProps = {
  value: number;
  onChange: (quantity: number) => void;
  label?: string;
};

const clampQuantity = (value: number | string) => Math.max(1, Number(value) || 1);

export const QuantitySelector = ({
  value,
  onChange,
  label = 'Quantity',
}: QuantitySelectorProps) => (
  <Stack gap={4}>
    <Text size="sm" fw={500}>
      {label}
    </Text>
    <Group justify="space-between" align="center" px="md">
      <Button
        variant="outline"
        color="darkred"
        size="lg"
        w={56}
        h={56}
        p={0}
        style={{ fontSize: 28, fontWeight: 700, borderRadius: 12 }}
        onClick={() => onChange(Math.max(1, value - 1))}
        disabled={value <= 1}
      >
        -
      </Button>
      <NumberInput
        value={value}
        onChange={(newValue) => onChange(clampQuantity(newValue))}
        min={1}
        style={{ width: 80 }}
        styles={{ input: { textAlign: 'center', fontWeight: 800, fontSize: 20 } }}
        hideControls
      />
      <Button
        variant="filled"
        color="darkred"
        size="lg"
        w={56}
        h={56}
        p={0}
        style={{ fontSize: 28, fontWeight: 700, borderRadius: 12 }}
        onClick={() => onChange(value + 1)}
      >
        +
      </Button>
    </Group>
  </Stack>
);
