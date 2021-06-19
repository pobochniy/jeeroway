import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as signalR from '@aspnet/signalr';
import { HubConnection } from '@aspnet/signalr';
import { UsersApiService } from '../../shared/api/users-api.service';
import { RoboControlModel } from '../../shared/models/robo-control.model';

@Component({
  selector: 'app-robo-control',
  templateUrl: './robo-control.component.html',
  styleUrls: ['./robo-control.component.css']
})
export class RoboControlComponent implements OnInit {
  public connection: HubConnection;
  public timerId: any;
  public buttonState: RoboControlModel = new RoboControlModel();
  public roboId: string;

  constructor(private service: UsersApiService
    , private route: ActivatedRoute) {
    this.keyButtons = this.keyButtons.bind(this);
    this.pushTheButton = this.pushTheButton.bind(this);
    //this.buttonState = this.buttonState.bind(this);
    this.sendButtonState = this.sendButtonState.bind(this);
  }

  async ngOnInit() {
    this.roboId = this.route.snapshot.paramMap.get('id');

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/robocontrol?roboId=xxx')
      .build();
    this.connection.start();
    document.addEventListener('keyup', this.keyButtons);
    document.addEventListener('keydown', this.keyButtons);
  }

  keyButtons(e) {
    if (e.type == 'keyup') this.pushTheButton('');
    else if (e.type == 'keydown') this.pushTheButton(e.key);
  }

  public pushTheButton(code: string) {
    switch (code) {
      case '':
        this.buttonState.clearControls();
        break;
      case 'w':
        this.buttonState.w = true;
        if (this.buttonState.w) this.buttonState.s = false;
        break;
      case 's':
        this.buttonState.s = true;
        if (this.buttonState.s) this.buttonState.w = false;
        break;
      case 'a':
        this.buttonState.a = true;
        if (this.buttonState.a) this.buttonState.d = false;
        break;
      case 'd':
        this.buttonState.d = true;
        if (this.buttonState.d) this.buttonState.a = false;
        break;
    }
    this.buttonState.roboId = this.roboId;
    this.buttonState.timeJs = new Date().getTime();
    clearTimeout(this.timerId);
    this.sendButtonState();
  }

  sendButtonState() {
    this.connection.invoke("PushControl", this.buttonState);
    //debugger;
    //console.log(this.buttonState)
    this.timerId = setTimeout(() => {
      this.buttonState.clearControls();
      this.sendButtonState()
    }, 20000);
  }
}
