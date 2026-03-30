import { useState } from 'react';
import { Button, Divider, Group, Modal, Stack, Textarea } from '@mantine/core';
import type { CartEntry } from '../types.ts';

type EditCartEntryModalProps = {
  entry: CartEntry;
  onSave: (specialInstructions: string | null) => void;
  onCancel: () => void;
};

export const EditCartEntryModal = ({ entry, onSave, onCancel }: EditCartEntryModalProps) => {
  const [instructions, setInstructions] = useState(entry.specialInstructions ?? '');

  return (
    <Modal opened onClose={onCancel} title={`Edit - ${entry.item.name}`} centered size="sm">
      <Stack gap="md">
        <Textarea
          label="Special instructions"
          placeholder="e.g. no onions, extra sauce..."
          value={instructions}
          onChange={(e) => setInstructions(e.currentTarget.value)}
          minRows={2}
          autosize
        />

        <Divider />

        <Group justify="space-between" align="center">
          <Group gap="sm">
            <Button variant="subtle" color="gray" onClick={onCancel}>
              Cancel
            </Button>
            <Button color="darkred" onClick={() => onSave(instructions || null)}>
              Save
            </Button>
          </Group>
        </Group>
      </Stack>
    </Modal>
  );
};
