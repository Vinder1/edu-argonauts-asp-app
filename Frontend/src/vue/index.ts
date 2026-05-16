import { createApp } from 'vue';
import { createPinia } from 'pinia';
import App from './App.vue';

export interface VueCallbacks {
    onReload: () => void;
    focusCameraOnSpaceship: () => void;
    onToggleAnimation: () => void;
    onBrightnessChange: (value: number) => void;
    onScaleChange: (value: number) => void;
}

export function mountVueApp(callbacks: VueCallbacks) {
    const pinia = createPinia();
    const app = createApp(App, { callbacks });
    app.use(pinia);
    return { app, pinia };
}
