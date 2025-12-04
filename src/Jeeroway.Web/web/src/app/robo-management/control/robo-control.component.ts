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
  public timerId: any;
  public buttonState = signal<RoboControlModel>(new RoboControlModel());
  public roboId: string = '';

  constructor(private route: ActivatedRoute) {
    this.keyButtons = this.keyButtons.bind(this);
    this.pushTheButton = this.pushTheButton.bind(this);
    this.sendButtonState = this.sendButtonState.bind(this);
  }

  async ngOnInit() {
    this.roboId = this.route.snapshot.paramMap.get('id') ?? '';

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`/robocontrol?roboId=${this.roboId}`)
      .build();
    this.connection.start();
    document.addEventListener('keyup', this.keyButtons);
    document.addEventListener('keydown', this.keyButtons);
  }

  ngOnDestroy(): void {
    document.removeEventListener('keyup', this.keyButtons);
    document.removeEventListener('keydown', this.keyButtons);
    clearTimeout(this.timerId);
    if (this.connection) {
      this.connection.stop();
    }
  }

  keyButtons(e: KeyboardEvent) {
    if (e.type == 'keyup') this.pushTheButton('');
    else if (e.type == 'keydown') this.pushTheButton(e.key);
  }

  public pushTheButton(code: string) {
    this.buttonState.update(_ => {
      const s = new RoboControlModel();
      switch (code) {
        case 'w': s.w = true; break;
        case 's': s.s = true; break;
        case 'a': s.a = true; break;
        case 'd': s.d = true; break;
        default: /* all false */ break;
      }
      s.roboId = this.roboId;
      s.timeJs = Date.now();
      return s;
    });
    clearTimeout(this.timerId);
    this.sendButtonState();
  }

  sendButtonState() {
    this.connection.invoke("PushControl", this.buttonState());
    this.timerId = setTimeout(() => {
      this.buttonState.set(new RoboControlModel());
      this.sendButtonState()
    }, 20000);
  }
}
