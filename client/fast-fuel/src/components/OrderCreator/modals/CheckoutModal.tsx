import { Button, Divider, Group, Modal, Stack, Text, TextInput } from '@mantine/core';
import { useForm } from '@mantine/form';
import type { CartEntry, CheckoutStep } from '../types.ts';
import { useEffect } from 'react';

type CheckoutModalProps = {
  checkoutStep: CheckoutStep;
  selectedRestaurantName: string | undefined;
  cart: CartEntry[];
  totalPrice: number;
  placedOrderNumber: number | null;
  isPending: boolean;
  onCloseCheckout: () => void;
  onConfirmOrder: () => void;
  onBackToConfirm: () => void;
  onPay: () => void;
};

export const CheckoutModal = ({
  checkoutStep,
  selectedRestaurantName,
  cart,
  totalPrice,
  placedOrderNumber,
  isPending,
  onCloseCheckout,
  onConfirmOrder,
  onBackToConfirm,
  onPay,
}: CheckoutModalProps) => {
  // Card form state and validation
  const cardForm = useForm({
    initialValues: {
      cardNumber: '',
      expiry: '',
      cvv: '',
    },
    validate: {
      cardNumber: (value) => {
        const digits = value.replace(/\D/g, '');
        if (digits.length !== 16) return 'Card number must be 16 digits';
        return null;
      },
      expiry: (value) => {
        if (!/^\d{2}\/\d{2}$/.test(value)) return 'Expiry must be MM/YY';
        const [mm, yy] = value.split('/').map(Number);
        if (mm < 1 || mm > 12) return 'Invalid month';
        // Expiry check: current date
        const now = new Date();
        const currentYear = now.getFullYear() % 100;
        const currentMonth = now.getMonth() + 1;
        if (yy < currentYear || (yy === currentYear && mm < currentMonth)) return 'Card expired';
        return null;
      },
      cvv: (value) => {
        if (!/^\d{3,4}$/.test(value)) return 'CVV must be 3 or 4 digits';
        return null;
      },
    },
  });

  // Format card number as #### #### #### ####
  function formatCardNumber(value: string) {
    return value
      .replace(/\D/g, '')
      .replace(/(.{4})/g, '$1 ')
      .trim();
  }

  // Reset form on modal close or thank you
  useEffect(() => {
    if (checkoutStep !== 'payment') {
      cardForm.reset();
    }

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [checkoutStep]);

  return (
    <>
      <Modal
        opened={checkoutStep === 'confirm'}
        onClose={onCloseCheckout}
        title="Confirm your order"
        centered
        size="sm"
      >
        <Stack gap="sm">
          <Text size="sm" c="dimmed">
            Ordering from:{' '}
            <Text span fw={700} c="orange">
              {selectedRestaurantName}
            </Text>
          </Text>
          <Divider />
          <Stack gap={6}>
            {cart.map((c) => (
              <Group key={c.cartKey} justify="space-between" align="flex-start" wrap="nowrap">
                <Stack gap={2} style={{ flex: 1, minWidth: 0 }}>
                  <Text size="sm">
                    {c.item.name}{' '}
                    <Text span c="dimmed">
                      x{c.quantity}
                    </Text>
                  </Text>
                  {c.specialInstructions && (
                    <Text size="xs" c="dimmed" fs="italic">
                      {c.specialInstructions}
                    </Text>
                  )}
                </Stack>
                <Group gap={6} align="center" style={{ flexShrink: 0 }}>
                  <Text size="sm" fw={600} c="darkred">
                    ${(c.item.price * c.quantity).toFixed(2)}
                  </Text>
                </Group>
              </Group>
            ))}
          </Stack>
          <Divider />
          <Group justify="space-between">
            <Text fw={700}>Total</Text>
            <Text fw={800} c="darkred" size="lg">
              ${totalPrice.toFixed(2)}
            </Text>
          </Group>
          <Group justify="flex-end" gap="sm" mt="xs">
            <Button variant="subtle" color="gray" onClick={onCloseCheckout}>
              Back
            </Button>
            <Button color="darkred" onClick={onConfirmOrder} disabled={cart.length === 0}>
              Confirm & Pay
            </Button>
          </Group>
        </Stack>
      </Modal>

      <Modal
        opened={checkoutStep === 'payment'}
        onClose={onCloseCheckout}
        title="Payment"
        centered
        size="sm"
        closeOnClickOutside={false}
        closeOnEscape={false}
        withCloseButton={false}
      >
        <form
          onSubmit={cardForm.onSubmit(() => {
            onPay();
          })}
        >
          <Stack gap="md">
            <Text size="sm" c="dimmed">
              Total to pay:
            </Text>
            <Text fw={800} size="xl" c="darkred" ta="center">
              ${totalPrice.toFixed(2)}
            </Text>
            <Divider label="Card details" labelPosition="center" />
            <TextInput
              label="Card number"
              maxLength={19}
              placeholder="1234 5678 9012 3456"
              {...cardForm.getInputProps('cardNumber')}
              value={cardForm.values.cardNumber}
              onChange={(e) => {
                cardForm.setFieldValue('cardNumber', formatCardNumber(e.currentTarget.value));
              }}
              error={cardForm.errors.cardNumber}
              inputMode="numeric"
              autoComplete="cc-number"
            />
            <Group grow>
              <TextInput
                label="Expiry date"
                maxLength={5}
                placeholder="MM/YY"
                {...cardForm.getInputProps('expiry')}
                value={cardForm.values.expiry}
                onChange={(e) => {
                  // Auto-insert slash
                  let v = e.currentTarget.value.replace(/[^\d]/g, '');
                  if (v.length > 2) v = v.slice(0, 2) + '/' + v.slice(2, 4);
                  cardForm.setFieldValue('expiry', v.slice(0, 5));
                }}
                error={cardForm.errors.expiry}
                inputMode="numeric"
                autoComplete="cc-exp"
              />
              <TextInput
                label="CVV"
                maxLength={4}
                placeholder="123"
                {...cardForm.getInputProps('cvv')}
                value={cardForm.values.cvv}
                onChange={(e) => {
                  cardForm.setFieldValue(
                    'cvv',
                    e.currentTarget.value.replace(/\D/g, '').slice(0, 4),
                  );
                }}
                error={cardForm.errors.cvv}
                inputMode="numeric"
                autoComplete="cc-csc"
              />
            </Group>
            <Button
              fullWidth
              color="darkred"
              loading={isPending}
              type="submit"
              disabled={!cardForm.isValid() || isPending}
            >
              Pay ${totalPrice.toFixed(2)}
            </Button>
            <Button variant="subtle" color="gray" onClick={onBackToConfirm}>
              Back
            </Button>
          </Stack>
        </form>
      </Modal>

      <Modal
        opened={checkoutStep === 'thankyou'}
        onClose={onCloseCheckout}
        title="Order placed!"
        centered
        size="sm"
        withCloseButton={false}
        closeOnClickOutside={false}
        closeOnEscape={false}
      >
        <Stack gap="sm" align="center">
          <Text fw={700} size="xl" c="orange">
            Order #{placedOrderNumber}
          </Text>
          <Text size="xs" c="dimmed" ta="center">
            Sit back and relax - your food is on its way!
          </Text>
          <Button fullWidth color="darkred" mt="sm" onClick={onCloseCheckout}>
            OK
          </Button>
        </Stack>
      </Modal>
    </>
  );
};
