/* eslint-disable */
// @ts-nocheck
/**
 * generate-page-api-types.ts
 *
 * Usage:
 *   npx ts-node generate-page-api-types.ts \
 *     --openapi  https://example.com/openapi.json \
 *     --permissions https://example.com/page-permissions.json \
 *     --out ./src/api/page-api-types.generated.ts
 *
 * Generated API surface:
 *   const api = useApi("MyPage");
 *   api.useNoPerm().useQuery("get", "/api/Public")
 *   api.useNecessaryPerm("Permission:Foo:Read").useQuery("get", "/api/Foo")
 *   api.useRecommendedPerm("Permission:Foo:Read")?.useQuery("get", "/api/Foo")
 */

import * as fs from 'fs';
import * as path from 'path';

// ---------------------------------------------------------------------------
// CLI args
// ---------------------------------------------------------------------------
function arg(name: string): string {
  const idx = process.argv.indexOf(name);
  if (idx === -1 || !process.argv[idx + 1]) throw new Error(`Missing arg: ${name}`);
  return process.argv[idx + 1];
}

// ---------------------------------------------------------------------------
// Input types
// ---------------------------------------------------------------------------
interface PagePermissionEntry {
  page: string;
  necessaryPermissions: string[];
  recommendedPermissions: string[];
  requiresDefaultRole: string[];
}

interface OpenAPIOperation {
  operationId?: string;
  'x-required-permission'?: string;
}

interface OpenAPISchema {
  paths: Record<string, Record<string, OpenAPIOperation>>;
}

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

/** Quoted string literal, e.g.  foo  →  "foo" */
function lit(s: string) {
  return `"${s.replace(/"/g, '\\"')}"`;
}

/** Formats a union of string literals, or "never" if empty. */
function union(items: string[]): string {
  return items.length ? items.join(' | ') : 'never';
}

// ---------------------------------------------------------------------------
// Core extraction
// ---------------------------------------------------------------------------
interface PathOperation {
  apiPath: string; // e.g. "/api/Ingredient/{id}"
  method: string; // e.g. "get"
  requiredPermission: string | null; // null = no permission required
}

function extractOperations(schema: OpenAPISchema): PathOperation[] {
  const ops: PathOperation[] = [];

  for (const [apiPath, methods] of Object.entries(schema.paths)) {
    for (const [method, operation] of Object.entries(methods)) {
      if (!operation || typeof operation !== 'object') continue;

      const permission = operation['x-required-permission'] ?? null;
      if (!permission) {
        console.warn(
          `⚠  No x-required-permission: ${method.toUpperCase()} ${apiPath} — will be included in HooksWithNoPerm`,
        );
      }

      ops.push({ apiPath, method: method.toLowerCase(), requiredPermission: permission });
    }
  }

  return ops;
}

// ---------------------------------------------------------------------------
// Code generation
// ---------------------------------------------------------------------------
function generate(ops: PathOperation[], pages: PagePermissionEntry[]): string {
  // ── Derived lookups ───────────────────────────────────────────────────────

  // permission → list of { path, method } (order-stable, deduped by path+method)
  const permToEndpoints = new Map<string, Array<{ path: string; method: string }>>();
  // operations with no required permission
  const noPermEndpoints: Array<{ path: string; method: string }> = [];

  for (const { apiPath, method, requiredPermission } of ops) {
    if (!requiredPermission) {
      if (!noPermEndpoints.some((e) => e.path === apiPath && e.method === method)) {
        noPermEndpoints.push({ path: apiPath, method });
      }
      continue;
    }
    if (!permToEndpoints.has(requiredPermission)) {
      permToEndpoints.set(requiredPermission, []);
    }
    const list = permToEndpoints.get(requiredPermission)!;
    if (!list.some((e) => e.path === apiPath && e.method === method)) {
      list.push({ path: apiPath, method });
    }
  }

  // All permission strings that appear in any page's necessary or optional lists
  const allPagePermissions = new Set<string>();
  for (const { necessaryPermissions, recommendedPermissions } of pages) {
    for (const p of [...necessaryPermissions, ...recommendedPermissions]) {
      allPagePermissions.add(p);
    }
  }

  // Sanity check: page permissions not found in the OpenAPI schema
  for (const { page, necessaryPermissions, recommendedPermissions } of pages) {
    for (const perm of [...necessaryPermissions, ...recommendedPermissions]) {
      if (!permToEndpoints.has(perm)) {
        console.warn(`⚠  Page "${page}" lists "${perm}" but no operation requires it`);
      }
    }
  }

  // ── Emit ──────────────────────────────────────────────────────────────────
  const lines: string[] = [];

  lines.push(`\
// ============================================================
//  AUTO-GENERATED — do not edit by hand.
//  Re-run generate-page-api-types.ts to update.
// ============================================================
`);

  lines.push(`import createClient from 'openapi-react-query';`);
  lines.push(`import type { paths } from './api-schema.generated.ts';`);
  lines.push(``);

  // ── PageName ──────────────────────────────────────────────────────────────
  lines.push(`export type PageName =`);
  for (const { page } of pages) {
    lines.push(`  | ${lit(page)}`);
  }
  lines.push(`;`);
  lines.push(``);

  // ── PermissionName ────────────────────────────────────────────────────────
  // Union of every permission string referenced by any page.
  // Permissions that appear in the schema but are not used by any page are
  // intentionally excluded — they are not part of the typed surface.
  const sortedPermissions = [...allPagePermissions].sort();
  lines.push(`export type PermissionName =`);
  for (const perm of sortedPermissions) {
    lines.push(`  | ${lit(perm)}`);
  }
  lines.push(`;`);
  lines.push(``);

  // ── PageNecessaryPermissions ──────────────────────────────────────────────
  lines.push(`export type PageNecessaryPermissions = {`);
  for (const { page, necessaryPermissions } of pages) {
    const known = necessaryPermissions.filter((p) => permToEndpoints.has(p));
    lines.push(`  ${page}: ${union(known.map(lit))};`);
  }
  lines.push(`};`);
  lines.push(``);

  // ── PageRecommendedPermissions ────────────────────────────────────────────
  lines.push(`export type PageRecommendedPermissions = {`);
  for (const { page, recommendedPermissions } of pages) {
    const known = recommendedPermissions.filter((p) => permToEndpoints.has(p));
    lines.push(`  ${page}: ${union(known.map(lit))};`);
  }
  lines.push(`};`);
  lines.push(``);

  // ── PermissionEndpoints ───────────────────────────────────────────────────
  // Maps each permission to the exact { path, method } pairs it grants access
  // to. Only permissions referenced by at least one page are emitted.
  lines.push(`export type PermissionEndpoints = {`);
  for (const perm of sortedPermissions) {
    const endpoints = permToEndpoints.get(perm);
    if (!endpoints?.length) continue; // warned above

    if (endpoints.length === 1) {
      const { path: p, method: m } = endpoints[0];
      lines.push(`  ${lit(perm)}: { path: ${lit(p)}; method: ${lit(m)} };`);
    } else {
      lines.push(`  ${lit(perm)}:`);
      for (let i = 0; i < endpoints.length; i++) {
        const { path: p, method: m } = endpoints[i];
        const tail = i < endpoints.length - 1 ? '' : ';';
        lines.push(`    | { path: ${lit(p)}; method: ${lit(m)} }${tail}`);
      }
    }
  }
  lines.push(`};`);
  lines.push(``);

  // ── PickPaths (static utility type) ──────────────────────────────────────
  // Constructs a paths-compatible object type containing only the specific
  // method keys (plus "parameters") for each path key matched by Filter.
  // This enables method-level filtering, unlike a plain Pick<paths, pathKey>.
  lines.push(
    `type PickPaths<Paths extends paths, Filter extends { path: string; method: string }> = {`,
  );
  lines.push(`  [PathKey in Filter['path'] & keyof Paths]: Pick<`);
  lines.push(`    Paths[PathKey],`);
  lines.push(
    `    (Extract<Filter, { path: PathKey }>['method'] | 'parameters') & keyof Paths[PathKey]`,
  );
  lines.push(`  >;`);
  lines.push(`};`);
  lines.push(``);

  // ── HooksForPerm ──────────────────────────────────────────────────────────
  // A client scoped to exactly the endpoints a given permission unlocks.
  lines.push(`export type HooksForPerm<Perm extends PermissionName> = ReturnType<`);
  lines.push(`  typeof createClient<PickPaths<paths, PermissionEndpoints[Perm]>>`);
  lines.push(`>;`);
  lines.push(``);

  // ── NoPermEndpoints ───────────────────────────────────────────────────────
  // Union of all { path, method } pairs that require no permission.
  if (noPermEndpoints.length === 0) {
    lines.push(`type NoPermEndpoints = never;`);
  } else if (noPermEndpoints.length === 1) {
    const { path: p, method: m } = noPermEndpoints[0];
    lines.push(`type NoPermEndpoints = { path: ${lit(p)}; method: ${lit(m)} };`);
  } else {
    lines.push(`type NoPermEndpoints =`);
    for (let i = 0; i < noPermEndpoints.length; i++) {
      const { path: p, method: m } = noPermEndpoints[i];
      const tail = i < noPermEndpoints.length - 1 ? '' : ';';
      lines.push(`  | { path: ${lit(p)}; method: ${lit(m)} }${tail}`);
    }
  }
  lines.push(``);

  // ── HooksWithNoPerm ───────────────────────────────────────────────────────
  lines.push(`export type HooksWithNoPerm = ReturnType<`);
  lines.push(`  typeof createClient<PickPaths<paths, NoPermEndpoints>>`);
  lines.push(`>;`);
  lines.push(``);

  // ── UseApiWithoutSpecifiedPage ────────────────────────────────────────────
  lines.push(`export type UseApiWithoutSpecifiedPage = {`);
  lines.push(`  useNoPerm(): HooksWithNoPerm;`);
  lines.push(`  invalidateApiCache(): void;`);
  lines.push(`};`);
  lines.push(``);

  // ── UseApiWithSpecifiedPage ───────────────────────────────────────────────
  lines.push(`type UseApiWithSpecifiedPage<Page extends PageName> = {`);
  lines.push(`  useNecessaryPerm<Perm extends PageNecessaryPermissions[Page]>(`);
  lines.push(`    permission: Perm,`);
  lines.push(`  ): HooksForPerm<Perm>;`);
  lines.push(`  useRecommendedPerm<Perm extends PageRecommendedPermissions[Page]>(`);
  lines.push(`    permission: Perm,`);
  lines.push(`  ): HooksForPerm<Perm> | null;`);
  lines.push(`};`);
  lines.push(``);

  // ── UseApi ────────────────────────────────────────────────────────────────
  lines.push(
    `export type UseApi<Page extends PageName | null> = UseApiWithoutSpecifiedPage & (Page extends PageName ? UseApiWithSpecifiedPage<Page> : never);`,
  );
  lines.push(``);

  return lines.join('\n');
}

// ---------------------------------------------------------------------------
// Entry point
// ---------------------------------------------------------------------------
async function fetchJson<T>(url: string): Promise<T> {
  const res = await fetch(url);
  if (!res.ok) throw new Error(`Failed to fetch ${url}: ${res.status} ${res.statusText}`);
  return res.json() as Promise<T>;
}

async function main() {
  const openapiUrl = arg('--openapi');
  const permissionsUrl = arg('--permissions');
  const outPath = arg('--out');

  const [schema, pages] = await Promise.all([
    fetchJson<OpenAPISchema>(openapiUrl),
    fetchJson<PagePermissionEntry[]>(permissionsUrl),
  ]);

  const ops = extractOperations(schema);
  const uniquePerms = new Set(ops.map((o) => o.requiredPermission)).size;
  console.log(`✔  Parsed ${ops.length} operations (${uniquePerms} unique permissions)`);
  console.log(`✔  Parsed ${pages.length} pages`);

  const code = generate(ops, pages);
  fs.mkdirSync(path.dirname(outPath), { recursive: true });
  fs.writeFileSync(outPath, code, 'utf8');
  console.log(`✔  Written to ${outPath}`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
