import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
// vite.config.js / vite.config.ts
import { viteStaticCopy } from 'vite-plugin-static-copy'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(),
    viteStaticCopy({
      targets: [
        {
          src: 'Build/*',
          dest: 'Build'
        }
      ]
    })

  ],
  base: './',
  build: {
    assetsDir: '.',
    outDir: 'dist',
  },
})
