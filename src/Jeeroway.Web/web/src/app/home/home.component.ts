import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterLink} from '@angular/router';
import {UserService} from '../../shared/services/user.service';
import {AuthApiService} from '../../shared/api/auth-api.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [AuthApiService]
})
export class HomeComponent {
  constructor(public service: UserService, private authServ: AuthApiService) {
  }

  async logOut() {
    this.service.User = undefined;
    await this.authServ.logOut();
    //this.router.navigateByUrl('/');
  }
}
