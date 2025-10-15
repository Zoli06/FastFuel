import {CodegenConfig} from "@graphql-codegen/cli";
import {loadEnv} from 'vite';

const env = loadEnv(
    "development",
    process.cwd()
);

const config: CodegenConfig = {
    overwrite: true,
    schema: env.VITE_GRAPHQL_HTTP_URL,
    // This assumes that all your source files are in a top-level `src/` directory - you might need to adjust this to your file structure
    documents: ["src/**/*.{ts,tsx}"],
    // Don't exit with non-zero status when there are no documents
    ignoreNoDocuments: true,
    generates: {
        // Use a path that works the best for the structure of your application
        "./src/types/__generated__/graphql.ts": {
            plugins: ["typescript", "typescript-operations", {
                add: {
                    content: "// eslint-disable-next-line @typescript-eslint/ban-ts-comment\n" +
                        "// @ts-ignore\n" +
                        "import { DateTime, LocalTime, URL, UnsignedInt } from \"graphql-scalars/typings/typeDefs\";"
                }
            }
            ],
            config: {
                avoidOptionals: {
                    // Use `null` for nullable fields instead of optionals
                    field: true,
                    // Allow nullable input fields to remain unspecified
                    inputValue: false,
                },
                enumsAsTypes: true,
                // Use `unknown` instead of `any` for unconfigured scalars
                defaultScalarType: "unknown",
                // Apollo Client always includes `__typename` fields
                nonOptionalTypename: true,
                scalars: {
                    DateTime: "DateTime",
                    LocalTime: "LocalTime",
                    UnsignedInt: "UnsignedInt",
                    URL: "URL",
                    UUID: "UUID",
                },
                // Apollo Client doesn't add the `__typename` field to root types so
                // don't generate a type for the `__typename` for root operation types.
                skipTypeNameForRoot: true,
                strictScalars: true,
            },
        },
    },
};

export default config;