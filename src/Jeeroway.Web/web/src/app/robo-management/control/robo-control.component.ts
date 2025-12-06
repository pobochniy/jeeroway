import { ChangeDetectionStrategy, Component, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as signalR from '@aspnet/signalr';
import { HubConnection } from '@aspnet/signalr';
import { RoboControlModel } from '../../../shared/models/robo-control.model';

@Component({
  selector: 'app-robo-control',
  standalone: true,
  templateUrl: './robo-control.component.html',
  styleUrls: ['./robo-control.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RoboControlComponent implements OnInit, OnDestroy {
  public connection!: HubConnection;
  public buttonState = signal<RoboControlModel>(new RoboControlModel());
  public roboId: string = '';
  public isConnected = signal<boolean>(false);

  constructor(private route: ActivatedRoute) {
    this.keyButtons = this.keyButtons.bind(this);
    this.pushTheButton = this.pushTheButton.bind(this);
    this.sendButtonState = this.sendButtonState.bind(this);
  }

  async ngOnInit() {
    this.roboId = this.route.snapshot.paramMap.get('id') ?? '';

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`/hub/robocontrol?roboId=${this.roboId}`)
      .build();

    try {
      await this.connection.start();
      this.isConnected.set(true);
      console.log('SignalR Connected');
    } catch (err) {
      console.error('SignalR Connection Error:', err);
      this.isConnected.set(false);
    }

    document.addEventListener('keyup', this.keyButtons);
    document.addEventListener('keydown', this.keyButtons);
  }

  ngOnDestroy(): void {
    document.removeEventListener('keyup', this.keyButtons);
    document.removeEventListener('keydown', this.keyButtons);
    if (this.connection) {
      this.connection.stop();
    }
  }

  keyButtons(e: KeyboardEvent) {
    const key = e.key.toLowerCase();
    
    // Маппинг русской раскладки на английскую
    const keyMap: Record<string, string> = {
      'ц': 'w', // W
      'ы': 's', // S
      'ф': 'a', // A
      'в': 'd'  // D
    };
    
    const mappedKey = keyMap[key] || key;
    
    if (['w', 'a', 's', 'd'].includes(mappedKey)) {
      if (e.type === 'keyup') {
        this.pushTheButton(mappedKey, false);
      } else if (e.type === 'keydown') {
        this.pushTheButton(mappedKey, true);
      }
    }
  }

  public pushTheButton(code: string, isPressed: boolean = true) {
    this.buttonState.update(current => {
      const s = new RoboControlModel();
      // Копируем текущее состояние
      s.w = current.w;
      s.s = current.s;
      s.a = current.a;
      s.d = current.d;

      // Обновляем только конкретную кнопку
      switch (code) {
        case 'w': s.w = isPressed; break;
        case 's': s.s = isPressed; break;
        case 'a': s.a = isPressed; break;
        case 'd': s.d = isPressed; break;
      }

      s.roboId = this.roboId;
      s.timeJs = Date.now();
      return s;
    });

    this.sendButtonState();
  }

  sendButtonState() {
    if (!this.isConnected()) {
      console.warn('SignalR not connected, skipping send');
      return;
    }

    const state = this.buttonState();

    // Проверяем, что roboId установлен
    if (!state.roboId) {
      console.warn('RoboId is empty, skipping send');
      return;
    }

    this.connection.invoke("PushControl", state)
      .catch(err => console.error('Error invoking PushControl:', err));

    // Отправка происходит только при изменении состояния кнопок
  }
}
