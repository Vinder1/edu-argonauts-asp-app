export interface GuideEntry {
    key: string;
    text: string;
}

export async function loadGuideKeys(): Promise<string[]> {
    try {
        let response = await fetch('/api/guides/');
        if (!response.ok) return [];
        const data = await response.json();
        return data as string[];
    } catch (error: unknown) {
        console.error('Ошибка загрузки списка гайдов:', error);
        return [];
    }
}

export async function loadGuideContent(key: string): Promise<GuideEntry | null> {
    try {
        let response = await fetch(`/api/guides/${encodeURIComponent(key)}`);
        if (!response.ok) return null;
        const data = await response.json();
        return data as GuideEntry;
    } catch (error: unknown) {
        console.error('Ошибка загрузки гайда:', error);
        return null;
    }
}
