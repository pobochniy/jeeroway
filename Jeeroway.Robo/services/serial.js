'use strict';

const { SerialPort } = require('serialport');

class SerialService {
  constructor({ path = '/dev/ttyACM0', baudRate = 115200, reconnectDelay = 2000 } = {}) {
    this.path = process.env.SERIAL_PATH || path;
    this.baudRate = Number(process.env.SERIAL_BAUD) || baudRate;
    this.reconnectDelay = reconnectDelay;

    this.port = null;
    this.opening = false;
    this.inFlight = false;
    this.queue = [];
    this.reconnectTimer = null;

    this.stats = {
      openCount: 0,
      bytesWritten: 0,
      messagesWritten: 0,
      lastOpenAt: null,
      lastCloseAt: null,
      lastError: null,
    };
  }

  isOpen() {
    return !!(this.port && this.port.isOpen);
  }

  getStatus() {
    return {
      path: this.path,
      baudRate: this.baudRate,
      isOpen: this.isOpen(),
      opening: this.opening,
      queueLength: this.queue.length,
      stats: this.stats,
    };
  }

  configure({ path, baudRate } = {}) {
    if (typeof path === 'string') this.path = path;
    if (typeof baudRate === 'number') this.baudRate = baudRate;

    // Reopen with new settings if already open
    if (this.isOpen()) {
      try { this.port.close(); } catch {}
    }
    this._ensureOpen();
  }

  async send(data) {
    const buffer = Buffer.isBuffer(data) ? data : Buffer.from(String(data));

    return new Promise((resolve, reject) => {
      this.queue.push({ buffer, resolve, reject });
      this._ensureOpen();
      this._processQueue();
    });
  }

  _ensureOpen() {
    if (this.isOpen() || this.opening) return;
    this._openPort();
  }

  _openPort() {
    this.opening = true;

    const port = new SerialPort({
      path: this.path,
      baudRate: this.baudRate,
      autoOpen: false,
    });

    port.once('open', () => {
      this.port = port;
      this.opening = false;
      this.stats.openCount++;
      this.stats.lastOpenAt = new Date().toISOString();
      this._clearReconnect();
      this._processQueue();
    });

    port.on('error', (err) => {
      this.stats.lastError = err?.message || String(err);
      // If open failed or runtime error, ensure reconnect
      if (!this.isOpen()) {
        this.opening = false;
        this._scheduleReconnect();
      }
    });

    port.on('close', () => {
      this.stats.lastCloseAt = new Date().toISOString();
      this.port = null;
      this.opening = false;
      this._scheduleReconnect();
    });

    // Raw data passthrough (optional subscribers can attach later if needed)
    port.on('data', (chunk) => {
      // no-op; could be extended to emit
    });

    port.open((err) => {
      if (err) {
        this.stats.lastError = err?.message || String(err);
        this.opening = false;
        this._scheduleReconnect();
      }
    });
  }

  _processQueue() {
    if (!this.isOpen() || this.inFlight) return;

    const item = this.queue.shift();
    if (!item) return;

    this.inFlight = true;
    this.port.write(item.buffer, (err) => {
      if (err) {
        this.inFlight = false;
        item.reject(err);
        // try next later
        setTimeout(() => this._processQueue(), 10);
        return;
      }

      this.port.drain((drainErr) => {
        this.inFlight = false;
        if (drainErr) {
          item.reject(drainErr);
        } else {
          this.stats.messagesWritten += 1;
          this.stats.bytesWritten += item.buffer.length;
          item.resolve();
        }
        // Proceed to next
        setImmediate(() => this._processQueue());
      });
    });
  }

  _scheduleReconnect() {
    if (this.reconnectTimer) return;
    this.reconnectTimer = setTimeout(() => {
      this.reconnectTimer = null;
      // only try to open if we still have pending messages or to keep link alive
      this._openPort();
    }, this.reconnectDelay);
  }

  _clearReconnect() {
    if (this.reconnectTimer) {
      clearTimeout(this.reconnectTimer);
      this.reconnectTimer = null;
    }
  }
}

const serial = new SerialService({});
module.exports = serial;
