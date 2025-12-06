'use strict';

const signalR = require('@microsoft/signalr');

class SignalRClient {
    constructor(hubUrl, roboId, reconnectInterval = 10000) {
        this.hubUrl = hubUrl;
        this.roboId = roboId;
        this.reconnectInterval = reconnectInterval;
        this.connection = null;
        this.reconnectTimer = null;
        this.isConnecting = false;
        this.handlers = new Map();
        
        this._initConnection();
    }

    _initConnection() {
        const url = `${this.hubUrl}?roboId=${this.roboId}`;
        
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(url)
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: () => this.reconnectInterval
            })
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this._setupEventHandlers();
    }

    _setupEventHandlers() {
        this.connection.onclose(async (error) => {
            console.error(`[SignalR] Connection closed. Error: ${error?.message || 'Unknown'}`);
            this._scheduleReconnect();
        });

        this.connection.onreconnecting((error) => {
            console.warn(`[SignalR] Reconnecting... Error: ${error?.message || 'Unknown'}`);
        });

        this.connection.onreconnected((connectionId) => {
            console.log(`[SignalR] Reconnected successfully. ConnectionId: ${connectionId}`);
            this._clearReconnectTimer();
        });
    }

    async start() {
        if (this.isConnecting || this.connection.state === signalR.HubConnectionState.Connected) {
            console.log('[SignalR] Already connected or connecting');
            return;
        }

        this.isConnecting = true;
        
        try {
            await this.connection.start();
            console.log(`[SignalR] Connected to hub. RoboId: ${this.roboId}, ConnectionId: ${this.connection.connectionId}`);
            this._clearReconnectTimer();
        } catch (error) {
            console.error(`[SignalR] Failed to connect: ${error.message}`);
            this._scheduleReconnect();
        } finally {
            this.isConnecting = false;
        }
    }

    _scheduleReconnect() {
        this._clearReconnectTimer();
        
        console.log(`[SignalR] Scheduling reconnect in ${this.reconnectInterval}ms...`);
        this.reconnectTimer = setTimeout(async () => {
            console.log('[SignalR] Attempting to reconnect...');
            await this.start();
        }, this.reconnectInterval);
    }

    _clearReconnectTimer() {
        if (this.reconnectTimer) {
            clearTimeout(this.reconnectTimer);
            this.reconnectTimer = null;
        }
    }

    on(methodName, handler) {
        if (!this.connection) {
            throw new Error('Connection not initialized');
        }
        
        this.connection.on(methodName, handler);
        this.handlers.set(methodName, handler);
        console.log(`[SignalR] Registered handler for method: ${methodName}`);
    }

    off(methodName) {
        if (!this.connection) {
            return;
        }
        
        this.connection.off(methodName);
        this.handlers.delete(methodName);
        console.log(`[SignalR] Unregistered handler for method: ${methodName}`);
    }

    async invoke(methodName, ...args) {
        if (this.connection.state !== signalR.HubConnectionState.Connected) {
            throw new Error('Connection is not in Connected state');
        }
        
        return await this.connection.invoke(methodName, ...args);
    }

    async stop() {
        this._clearReconnectTimer();
        
        if (this.connection) {
            await this.connection.stop();
            console.log('[SignalR] Connection stopped');
        }
    }

    getState() {
        return this.connection?.state || 'Not initialized';
    }

    isConnected() {
        return this.connection?.state === signalR.HubConnectionState.Connected;
    }
}

module.exports = SignalRClient;
