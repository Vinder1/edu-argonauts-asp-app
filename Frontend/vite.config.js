import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

export default defineConfig({
  plugins: [vue()],
  build: {
    outDir: '../Argonauts.Web/wwwroot',
    emptyOutDir: true,
    assetsDir: 'assets'
  },
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5085',
        changeOrigin: true,
        secure: false
      },
      '/events': {
        target: 'http://localhost:5085',
        changeOrigin: true,
        secure: false,
        ws: true
      },
    }
  }
});
