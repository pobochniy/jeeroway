import { ChangeDetectionStrategy, Component, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { RoboApiService } from '../../../shared/api/robo-api.service';
import { RoboModel } from '../../../shared/models/robo.model';

@Component({
  selector: 'app-robo-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './robo-list.component.html',
  styleUrls: ['./robo-list.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RoboListComponent implements OnInit {
  public dataSource = signal<RoboModel[]>([]);

  constructor(private service: RoboApiService) {
  }

  async ngOnInit() {
    this.dataSource.set(await this.service.List());
  }

  async Delete(id: string) {
    if (confirm("Вы - Администратор?")) {
      await this.service.Delete(id);
      this.dataSource.set(await this.service.List());
    }
  }
}
