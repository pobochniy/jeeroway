import { ChangeDetectionStrategy, Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { RoboApiService } from '../../../shared/api/robo-api.service';
import { RoboModel } from '../../../shared/models/robo.model';

@Component({
  selector: 'app-robo-details',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './robo-details.component.html',
  styleUrls: ['./robo-details.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RoboDetailsComponent implements OnInit {
  robot = signal<RoboModel | null>(null);

  constructor(private service: RoboApiService
    , private router: Router
    , private route: ActivatedRoute) {
  }

  async ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.robot.set(await this.service.Details(id));
    }
  }
}
