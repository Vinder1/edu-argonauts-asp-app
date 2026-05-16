<script setup lang="ts">
import { useGameStore } from '../stores/gameStore';

const game = useGameStore();
</script>

<template>
    <div class="stats-panel">
        <div class="stat-item">Звезд: {{ game.starCount }}</div>
        <div class="stat-item">Радиус: {{ game.minRadius }} - {{ game.maxRadius }}</div>
    </div>
</template>
