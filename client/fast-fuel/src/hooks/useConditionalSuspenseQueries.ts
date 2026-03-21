import {
  useQueries,
  useQueryClient,
  type UseQueryOptions,
  type UseQueryResult,
} from '@tanstack/react-query';

type ResultForOptions<T> = T extends false
  ? UseQueryResult<undefined>
  : T extends { queryFn: (...args: never[]) => infer TResult }
    ? UseQueryResult<Awaited<TResult>>
    : UseQueryResult;

export const useConditionalSuspenseQueries = <const T extends readonly (object | false)[]>(
  queries: T,
): { [K in keyof T]: ResultForOptions<T[K]> } => {
  const queryClient = useQueryClient();

  const results = useQueries({
    queries: queries.map((q, i) =>
      q === false ? { queryKey: ['__skipped__', i], queryFn: () => null, enabled: false } : q,
    ) as (UseQueryOptions & { queryKey: readonly unknown[] })[],
  });

  for (let i = 0; i < queries.length; i++) {
    const q = queries[i];
    if (q !== false && results[i].isPending) {
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
