import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tseslint from 'typescript-eslint'
import { defineConfig, globalIgnores } from 'eslint/config'

export default defineConfig([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      reactHooks.configs['recommended-latest'],
      reactRefresh.configs.vite,
    ],
    languageOptions: {
      ecmaVersion: 2020,
      globals: globals.browser,
    },
    rules: {
      // Disallow `import React from 'react'` (default import)
      'no-restricted-imports': ['error', {
        paths: [
          {
            name: 'react',
            importNames: ['default'],
            message: 'Default React import is unnecessary with the new JSX transform — use named imports for hooks instead.'
          }
        ]
      }],

      // Disallow `import * as React from 'react'` (namespace import)
      'no-restricted-syntax': ['error',
        {
          selector: "ImportNamespaceSpecifier[source.value='react']",
          message: 'Namespace React import is unnecessary — use named imports for hooks instead.'
        }
      ]
    }
  },
])
