import {
  Anchor,
  Badge,
  Box,
  Button,
  Container,
  Divider,
  Group,
  Image,
  Paper,
  Stack,
  Text,
  Title,
} from '@mantine/core';
import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { useConditionalSuspenseQueries } from '../../hooks/useConditionalSuspenseQueries.ts';
import { getDisplayedDescription } from '../../lib/description.ts';
import { useApi } from '../../lib/api.ts';
import welcomeBg from './WelcomeBg_transitioned.webp';

const dayOfWeekOrder = [
  'Monday',
  'Tuesday',
  'Wednesday',
  'Thursday',
  'Friday',
  'Saturday',
  'Sunday',
] as const;

type DayOfWeek = (typeof dayOfWeekOrder)[number];

interface OpeningHour {
  dayOfWeek: DayOfWeek;
  openTime: string;
  closeTime: string;
}

function getTodayName(): DayOfWeek {
  return dayOfWeekOrder[new Date().getDay() === 0 ? 6 : new Date().getDay() - 1];
}

function isOpenNow(hours: OpeningHour[]): boolean {
  const today = getTodayName();
  const todayHours = hours.find((h) => h.dayOfWeek === today);
  if (!todayHours) return false;
  const now = new Date();
  const [openH, openM] = todayHours.openTime.split(':').map(Number);
  const [closeH, closeM] = todayHours.closeTime.split(':').map(Number);
  const mins = now.getHours() * 60 + now.getMinutes();
  return mins >= openH * 60 + openM && mins < closeH * 60 + closeM;
}

export const Welcome = () => {
  const { useNoPerm } = useApi(null);
  const [{ data: restaurants = [] }] = useConditionalSuspenseQueries([
    useNoPerm().queryOptions('get', '/api/Restaurant'),
  ]);

  const [selectedRestaurantId, setSelectedRestaurantId] = useState<number | null>(null);

  useEffect(() => {
    if (restaurants.length === 0) {
      setSelectedRestaurantId(null);
      return;
    }

    const selectedStillExists = restaurants.some((r) => r.id === selectedRestaurantId);
    if (!selectedStillExists) {
      setSelectedRestaurantId(restaurants[0].id);
    }
  }, [restaurants, selectedRestaurantId]);

  const selectedRestaurant = restaurants.find((r) => r.id === selectedRestaurantId);

  const orderedOpeningHours = useMemo(() => {
    if (!selectedRestaurant) return [];
    const dayToIndex = new Map(dayOfWeekOrder.map((day, i) => [day, i]));
    return [...selectedRestaurant.openingHours].sort(
      (a, b) => (dayToIndex.get(a.dayOfWeek) ?? 0) - (dayToIndex.get(b.dayOfWeek) ?? 0),
    );
  }, [selectedRestaurant]);

  const today = getTodayName();
  const selectedIsOpen = selectedRestaurant ? isOpenNow(selectedRestaurant.openingHours) : false;
  const mapsUrl = selectedRestaurant
    ? `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(selectedRestaurant.address)}`
    : null;

  return (
    <Container size="lg" py={80}>
      <Paper
        pos="relative"
        radius="xl"
        p={{ base: 'xl', sm: '3rem' }}
        mih={420}
        style={{
          overflow: 'hidden',
          background:
            'linear-gradient(145deg, rgba(255, 245, 230, 0.98) 0%, rgba(255, 225, 180, 0.96) 52%, rgba(255, 204, 153, 0.94) 100%)',
          border: '1px solid rgba(140, 72, 22, 0.16)',
          boxShadow: '0 24px 60px rgba(140, 72, 22, 0.12)',
        }}
      >
        {/* ── Hero ── */}
        <Box
          style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))',
            gap: '2rem',
            alignItems: 'center',
          }}
        >
          <Stack gap="lg" justify="center" maw={520}>
            <Text fw={700} tt="uppercase" c="orange.8" size="sm">
              Welcome to Fast Fuel
            </Text>
            <Title order={1} fz={{ base: '2.5rem', sm: '3.5rem' }} lh={1.05}>
              Fast orders, clear workflows, and more all in one.
            </Title>
            <Button
              component={Link}
              to="/login"
              size="lg"
              radius="xl"
              w={{ base: '100%', sm: 'fit-content' }}
              color="dark"
            >
              Login
            </Button>
          </Stack>

          <Image
            src={welcomeBg}
            alt="Fast Fuel welcome background"
            radius="lg"
            fit="cover"
            h={{ base: 220, sm: 320 }}
            style={{
              pointerEvents: 'none',
              userSelect: 'none',
              boxShadow: '0 18px 40px rgba(120, 68, 24, 0.18)',
            }}
          />
        </Box>

        <Divider my="xl" color="orange.3" />

        {/* ── Restaurant browser ── */}
        <Box
          style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))',
            gap: '1.25rem',
            alignItems: 'start',
          }}
        >
          {/* List */}
          <Stack gap="sm">
            <Title order={3}>Restaurants</Title>
            {restaurants.length === 0 ? (
              <Text c="dimmed">No restaurants available right now.</Text>
            ) : (
              restaurants.map((restaurant) => {
                const isSelected = restaurant.id === selectedRestaurantId;
                const open = isOpenNow(restaurant.openingHours ?? []);
                return (
                  <Paper
                    key={restaurant.id}
                    withBorder
                    radius="md"
                    p="sm"
                    onClick={() => setSelectedRestaurantId(restaurant.id)}
                    style={{
                      cursor: 'pointer',
                      transition: 'box-shadow 150ms ease, border-color 150ms ease',
                      borderColor: isSelected ? 'var(--mantine-color-orange-6)' : undefined,
                      boxShadow: isSelected ? '0 0 0 1px var(--mantine-color-orange-6)' : undefined,
                    }}
                  >
                    <Group justify="space-between" align="flex-start" wrap="nowrap">
                      <Text fw={600} size="sm" lineClamp={1} style={{ flex: 1 }}>
                        {restaurant.name}
                      </Text>
                      {(restaurant.openingHours?.length ?? 0) > 0 && (
                        <Badge
                          size="xs"
                          variant="light"
                          color={open ? 'green' : 'gray'}
                          style={{ flexShrink: 0 }}
                        >
                          {open ? 'Open' : 'Closed'}
                        </Badge>
                      )}
                    </Group>
                    <Text size="xs" c="dimmed" lineClamp={1} mt={2}>
                      {restaurant.address}
                    </Text>
                  </Paper>
                );
              })
            )}
          </Stack>

          {/* Detail */}
          <Paper withBorder radius="md" p="md">
            {!selectedRestaurant ? (
              <Stack align="center" py="xl" gap="xs">
                <Text size="lg">👈</Text>
                <Text c="dimmed" ta="center" size="sm">
                  Pick a restaurant on the left to see details.
                </Text>
              </Stack>
            ) : (
              <Stack gap="md">
                {/* Header */}
                <Box>
                  <Group justify="space-between" align="flex-start">
                    <Title order={3}>{selectedRestaurant.name}</Title>
                    {(selectedRestaurant.openingHours?.length ?? 0) > 0 && (
                      <Badge variant="light" color={selectedIsOpen ? 'green' : 'gray'}>
                        {selectedIsOpen ? 'Open now' : 'Closed'}
                      </Badge>
                    )}
                  </Group>
                  {selectedRestaurant.description && (
                    <Text size="sm" c="dimmed" mt="xs">
                      {getDisplayedDescription(selectedRestaurant.description, 300)}
                    </Text>
                  )}
                </Box>

                {/* Contact */}
                <Stack gap={4}>
                  <Text
                    size="xs"
                    tt="uppercase"
                    fw={600}
                    c="dimmed"
                    style={{ letterSpacing: '0.05em' }}
                  >
                    Contact
                  </Text>
                  <Group justify="space-between" gap="sm">
                    <Text size="sm" c="dimmed">
                      Phone
                    </Text>
                    {selectedRestaurant.phone ? (
                      <Anchor href={`tel:${selectedRestaurant.phone}`} size="sm">
                        {selectedRestaurant.phone}
                      </Anchor>
                    ) : (
                      <Text size="sm" c="dimmed">
                        Not provided
                      </Text>
                    )}
                  </Group>
                </Stack>

                {/* Location */}
                <Stack gap={4}>
                  <Text
                    size="xs"
                    tt="uppercase"
                    fw={600}
                    c="dimmed"
                    style={{ letterSpacing: '0.05em' }}
                  >
                    Location
                  </Text>
                  <Group justify="space-between" gap="sm" align="flex-start">
                    <Text size="sm" c="dimmed">
                      Address
                    </Text>
                    <Anchor
                      href={mapsUrl!}
                      target="_blank"
                      rel="noopener noreferrer"
                      size="sm"
                      ta="right"
                      style={{ flex: 1 }}
                    >
                      {selectedRestaurant.address}
                    </Anchor>
                  </Group>
                </Stack>

                {/* Opening hours */}
                <Stack gap={6}>
                  <Text
                    size="xs"
                    tt="uppercase"
                    fw={600}
                    c="dimmed"
                    style={{ letterSpacing: '0.05em' }}
                  >
                    Opening hours
                  </Text>
                  {orderedOpeningHours.length === 0 ? (
                    <Text size="sm" c="dimmed">
                      No opening hours provided.
                    </Text>
                  ) : (
                    orderedOpeningHours.map((hour) => {
                      const isToday = hour.dayOfWeek === today;
                      return (
                        <Group
                          key={hour.dayOfWeek}
                          justify="space-between"
                          gap="sm"
                          style={{
                            padding: '3px 6px',
                            borderRadius: 6,
                            background: isToday ? 'var(--mantine-color-orange-0)' : undefined,
                          }}
                        >
                          <Text
                            size="sm"
                            fw={isToday ? 600 : 400}
                            c={isToday ? 'orange.7' : undefined}
                          >
                            {hour.dayOfWeek}
                            {isToday && (
                              <Text component="span" size="xs" c="dimmed" ml={4}>
                                (today)
                              </Text>
                            )}
                          </Text>
                          <Text
                            size="sm"
                            fw={isToday ? 600 : 400}
                            c={isToday ? 'orange.7' : undefined}
                          >
                            {hour.openTime.slice(0, 5)} – {hour.closeTime.slice(0, 5)}
                          </Text>
                        </Group>
                      );
                    })
                  )}
                </Stack>
              </Stack>
            )}
          </Paper>
        </Box>
      </Paper>
    </Container>
  );
};
