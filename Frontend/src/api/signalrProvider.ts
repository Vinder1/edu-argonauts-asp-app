import * as signalR from '@microsoft/signalr';
import { auth } from '../auth/auth';

class SignalRProvider {
    private connection: signalR.HubConnection | null = null;

    getConnection(): signalR.HubConnection {
        if (!this.connection) {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl('/events', {
                    accessTokenFactory: () => auth.getToken() || ''
                })
                .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
                .build();
        }
        return this.connection;
    }

    async start(): Promise<void> {
        try {
            const conn = this.getConnection();
            if (conn.state === signalR.HubConnectionState.Disconnected) {
                await conn.start();
            }
        } catch (err) {
            console.warn('SignalR connection failed, will retry:', err);
        }
    }

    async stop(): Promise<void> {
        if (this.connection) {
            await this.connection.stop();
        }
    }
}

export const signalrProvider = new SignalRProvider();
