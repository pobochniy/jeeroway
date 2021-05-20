import { Component, OnInit } from '@angular/core';
import { RoboApiService } from '../../shared/api/robo-api.service';
import { RoboModel } from '../../shared/models/robo.model';

@Component({
  selector: 'app-robo-list',
  templateUrl: './robo-list.component.html',
  styleUrls: ['./robo-list.component.css']
})
export class RoboListComponent implements OnInit {

  public dataSource: RoboModel[];

  constructor(private service: RoboApiService) {
  }

  async ngOnInit() {
    this.dataSource = await this.service.List();
  }

  async Delete(id: string) {
    if (confirm("Вы - Администратор?")) {
      await this.service.Delete(id);
      this.dataSource = await this.service.List();
    }
  }
}
