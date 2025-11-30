import {ChangeDetectionStrategy, Component} from '@angular/core';
import {AlertsService} from "./alerts.service";

@Component({
  selector: 'shared-alerts',
  standalone: true,
  templateUrl: './alerts.component.html',
  styleUrls: ['./alerts.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AlertsComponent {
  constructor(public alertsService: AlertsService) {
  }

  public removeAlert(id: number){
    this.alertsService.remove(id);
  }
}
