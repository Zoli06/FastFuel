import {
  useQueries,
  useQueryClient,
  type UseQueryOptions,
  type UseQueryResult,
} from '@tanstack/react-query';

type ResultForOptions<T> = T extends undefined
  ? UseQueryResult<undefined>
  : T extends { queryFn: (...args: never[]) => infer TResult }
    ? UseQueryResult<Awaited<TResult>>
    : UseQueryResult;

export const useConditionalSuspenseQueries = <const T extends readonly (object | undefined)[]>(
  queries: T,
): { [K in keyof T]: ResultForOptions<T[K]> } => {
  const queryClient = useQueryClient();

  const results = useQueries({
    queries: queries.map((q, i) =>
      q === undefined ? { queryKey: ['__skipped__', i], enabled: false } : q,
    ) as (UseQueryOptions & { queryKey: readonly unknown[] })[],
  });

  for (let i = 0; i < queries.length; i++) {
    const q = queries[i];
    if (q !== undefined && results[i].isPending) {
      const cachedQuery = queryClient
        .getQueryCache()
        .find({ queryKey: (q as { queryKey: readonly unknown[] }).queryKey });
      if (cachedQuery?.promise) {
        throw cachedQuery.promise;
      }
    }
  }

  return results as { [K in keyof T]: ResultForOptions<T[K]> };
};
