import { Injectable, signal } from '@angular/core';
import { AlertModel } from "../models/alert.model";

@Injectable({ providedIn: 'root' })
export class AlertsService {
  private nextId = 0;
  // Signal-based store of alerts for zoneless CD
  public readonly alerts = signal<AlertModel[]>([]);

  public push(
    alertClass: "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark",
    title?: string,
    error?: any,
    timeToCloseMs: number = 0
  ) {
    const details = this.formatApiError(error);
    const content = [title, details].filter(Boolean).join('\n');

    const id = ++this.nextId;
    const item = new AlertModel({ id, alertClass, content });
    this.alerts.update(list => [...list, item]);

    if (timeToCloseMs > 0) setTimeout(() => this.remove(id), timeToCloseMs);
  }

  public remove(id: number) {
    this.alerts.update(list => list.filter(x => x.id !== id));
  }

  /**
   * Formats API error payloads into a human-readable string.
   * Accepts ModelState dictionaries (422), strings, Error-like objects.
   */
  private formatApiError(err: any): string {
    if (!err) return '';

    // If caller passes the entire HttpErrorResponse, use .error
    const body = err?.error !== undefined ? err.error : err;
    const status = err?.status ?? 0;

    // ModelState: { Field: [messages] }
    const looksLikeModelState = body && typeof body === 'object' && !Array.isArray(body)
      && Object.values(body as Record<string, unknown>).some(v => Array.isArray(v) || typeof v === 'string');

    if ((status === 422 || (status === 0 && looksLikeModelState)) && looksLikeModelState) {
      const lines: string[] = [];
      for (const [field, msgs] of Object.entries(body as Record<string, unknown>)) {
        const arr = Array.isArray(msgs) ? msgs : [msgs];
        for (const m of arr) lines.push(`${field}: ${String(m)}`);
      }
      return lines.join('\n');
    }

    if (typeof body === 'string') return body;
    if (typeof body?.message === 'string') return body.message;
    if (typeof err?.message === 'string') return err.message;
    return '';
  }
}
