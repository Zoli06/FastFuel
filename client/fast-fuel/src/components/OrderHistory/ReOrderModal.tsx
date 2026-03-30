import { Badge, Box, Button, Divider, Group, Modal, ScrollArea, Stack, Text } from '@mantine/core';
import { IconCheck, IconRefresh } from '@tabler/icons-react';
import { useState } from 'react';
import { apiClient } from '../../lib/api-client.ts';

export interface ReorderItem {
  id: number;
  type: 'food' | 'menu';
  name: string;
  price: number;
  quantity: number;
  specialInstructions?: string;
}

export interface ReorderModalProps {
  opened: boolean;
  onClose: () => void;
  orderNumber: number;
  restaurantId: number;
  restaurantName: string;
  items: ReorderItem[];
}

type Step = 'review' | 'success';

export const ReorderModal = ({
  opened,
  onClose,
  orderNumber,
  restaurantId,
  restaurantName,
  items,
}: ReorderModalProps) => {
  const [step, setStep] = useState<Step>('review');
  const [newOrderNumber, setNewOrderNumber] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const { mutateAsync: createOrder, isPending } = apiClient.useMutation('post', '/api/Order');

  const total = items.reduce((sum, i) => sum + i.price * i.quantity, 0);

  const handleConfirm = async () => {
    setError(null);
    try {
      const created = await createOrder({
        body: {
          restaurantId,
          // Match the exact shape OrderCreator uses — null instead of undefined
          foods: items
            .filter((i) => i.type === 'food')
            .map((i) => ({
              foodId: i.id,
              quantity: i.quantity,
              specialInstructions: i.specialInstructions ?? null,
            })),
          menus: items
            .filter((i) => i.type === 'menu')
            .map((i) => ({
              menuId: i.id,
              quantity: i.quantity,
              specialInstructions: i.specialInstructions ?? null,
            })),
        },
      });
      setNewOrderNumber(created.orderNumber);
      setStep('success');
    } catch (err) {
      setError('Something went wrong placing your order. Please try again.');
      console.error('Reorder failed:', err);
    }
  };

  const handleClose = () => {
    setStep('review');
    setNewOrderNumber(null);
    setError(null);
    onClose();
  };

  return (
    <Modal
      opened={opened}
      onClose={handleClose}
      title={
        step === 'review' ? (
          <Group gap="xs">
            <IconRefresh size={20} color="var(--mantine-color-orange-5)" />
            <Text fw={700} size="md">
              Reorder{' '}
              <Text span c="orange.4">
                #{orderNumber}
              </Text>
            </Text>
          </Group>
        ) : null
      }
      centered
      size="sm"
      radius="md"
      withCloseButton={step === 'review'}
      closeOnClickOutside={step === 'review'}
      closeOnEscape={step === 'review'}
    >
      {step === 'review' ? (
        <Stack gap="sm">
          <Text size="sm" c="dimmed">
            From:{' '}
            <Text span fw={600} c="orange">
              {restaurantName}
            </Text>
          </Text>

          <Divider />

          <ScrollArea h={Math.min(items.length * 64, 260)} scrollbarSize={4}>
            <Stack gap={6}>
              {items.map((item, i) => (
                <Group key={i} justify="space-between" wrap="nowrap" gap="xs">
                  <Stack gap={2} style={{ flex: 1, minWidth: 0 }}>
                    <Group gap={6}>
                      <Text size="sm" fw={600} lineClamp={1}>
                        {item.name}
                      </Text>
                      <Badge size="xs" color="orange" variant="outline">
                        ×{item.quantity}
                      </Badge>
                      <Badge
                        size="xs"
                        color={item.type === 'menu' ? 'grape' : 'blue'}
                        variant="light"
                      >
                        {item.type}
                      </Badge>
                    </Group>
                    {item.specialInstructions && (
                      <Text size="xs" c="dimmed" fs="italic" lineClamp={1}>
                        {item.specialInstructions}
                      </Text>
                    )}
                  </Stack>
                  <Text size="sm" fw={700} c="orange.4" style={{ flexShrink: 0 }}>
                    ${(item.price * item.quantity).toFixed(2)}
                  </Text>
                </Group>
              ))}
            </Stack>
          </ScrollArea>

          <Divider />

          <Group justify="space-between">
            <Text fw={700} size="sm">
              Estimated total
            </Text>
            <Text fw={800} size="md" c="orange.5">
              ${total.toFixed(2)}
            </Text>
          </Group>

          <Text size="xs" c="dimmed">
            Prices may have changed since your original order.
          </Text>

          {error && (
            <Text size="sm" c="red">
              {error}
            </Text>
          )}

          <Group justify="flex-end" gap="sm" mt="xs">
            <Button variant="subtle" color="gray" onClick={handleClose} disabled={isPending}>
              Cancel
            </Button>
            <Button
              color="orange"
              leftSection={<IconRefresh size={16} />}
              onClick={handleConfirm}
              loading={isPending}
            >
              Reorder
            </Button>
          </Group>
        </Stack>
      ) : (
        <Stack gap="sm" align="center" py="md">
          <Box
            style={{
              width: 56,
              height: 56,
              borderRadius: '50%',
              background: 'var(--mantine-color-green-8)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
            }}
          >
            <IconCheck size={28} color="white" />
          </Box>
          <Text fw={700} size="lg" ta="center">
            Order placed!
          </Text>
          <Text fw={800} size="xl" c="orange">
            #{newOrderNumber}
          </Text>
          <Text size="sm" c="dimmed" ta="center">
            Your reorder from{' '}
            <Text span fw={600} c="orange">
              {restaurantName}
            </Text>{' '}
            has been submitted.
          </Text>
          <Button fullWidth color="orange" mt="sm" onClick={handleClose}>
            Done
          </Button>
        </Stack>
      )}
    </Modal>
  );
};
