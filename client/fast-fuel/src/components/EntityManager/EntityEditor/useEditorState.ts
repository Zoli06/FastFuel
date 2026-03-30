import { useState, useCallback } from 'react';
import type { EditorMode } from './types.ts';

export type EditorState<Values> = {
  opened: boolean;
  mode: EditorMode;
  data: Values | null;
  openCreate: () => void;
  openEdit: (item: Values) => void;
  close: () => void;
};

export function useEditorState<Values = unknown>(): EditorState<Values> {
  const [opened, setOpened] = useState(false);
  const [mode, setMode] = useState<EditorMode>('create');
  const [data, setData] = useState<Values | null>(null);

  const openCreate = useCallback(() => {
    setData(null);
    setMode('create');
    setOpened(true);
  }, []);

  const openEdit = useCallback((item: Values) => {
    setData(item);
    setMode('edit');
    setOpened(true);
  }, []);

  const close = useCallback(() => {
    setOpened(false);
  }, []);

  return { opened, mode, data, openCreate, openEdit, close };
}
