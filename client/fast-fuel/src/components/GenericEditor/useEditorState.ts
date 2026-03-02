import { useState, useCallback } from 'react';

export type EditorMode = 'create' | 'edit';

export type EditorState<TData> = {
  opened: boolean;
  mode: EditorMode;
  data: TData | null;
  openCreate: () => void;
  openEdit: (item: TData) => void;
  close: () => void;
};

export function useEditorState<TData = unknown>(): EditorState<TData> {
  const [opened, setOpened] = useState(false);
  const [mode, setMode] = useState<EditorMode>('create');
  const [data, setData] = useState<TData | null>(null);

  const openCreate = useCallback(() => {
    setData(null);
    setMode('create');
    setOpened(true);
  }, []);

  const openEdit = useCallback((item: TData) => {
    setData(item);
    setMode('edit');
    setOpened(true);
  }, []);

  const close = useCallback(() => {
    setOpened(false);
  }, []);

  return { opened, mode, data, openCreate, openEdit, close };
}
